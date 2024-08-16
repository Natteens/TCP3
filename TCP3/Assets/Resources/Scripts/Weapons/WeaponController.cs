using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Unity.Collections;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private WeaponInfo currentWeapon;
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private MultiAimConstraint torsoAimConstraint;
    [SerializeField] private TPSController tpsController;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask layer;

    private StarterAssetsInputs input;
    private float aimWeightChangeSpeed = 5f;
    private float fireRateCounter;

    public NetworkVariable<FixedString64Bytes> currentWeaponName = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentAmmo = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> fireRateCounterNet = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isShootingNet = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server);

    public event Action OnWeaponChanged;
    public event Action OnShoot;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        if (IsOwner)
        {
            EquipWeapon(currentWeapon);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleInput();
        var (success, position) = MouseController.GetMousePosition(Camera.main, layer);
        if (success)
        {
            HandleAiming(position);
        }
        AdjustTorsoAimWeight();

        if (CanShoot())
        {
            HandleShooting(position);
        }
        else
        {
            StopShooting();
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            FireCounterTimer();
        }
    }

    private void FireCounterTimer()
    {
        if (currentWeapon != null && fireRateCounterNet.Value < 2f)
        {
            fireRateCounterNet.Value += Time.fixedDeltaTime;
        }
    }

    private void HandleShooting(Vector3 aimPoint)
    {
        tpsController.RotateTowardsMouseSmooth(aimPoint);

        if (fireRateCounterNet.Value >= currentWeapon.cadence)
        {
            if (currentAmmo.Value > 0)
            {
                if (!isShootingNet.Value)
                {
                    isShootingNet.Value = true;
                    ShootServerRpc(aimPoint);
                }
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    private void StopShooting()
    {
        if (isShootingNet.Value)
        {
            anim.SetInteger("WeaponState", input.aim ? 3 : 2);
            isShootingNet.Value = false;
        }
    }

    private void HandleAiming(Vector3 aimPoint)
    {
        if (input.aim)
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            anim.SetInteger("WeaponState", 3);
            tpsController.RotateTowardsMouseSmooth(aimPoint);
        }
        else
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            anim.SetInteger("WeaponState", 2);
        }
    }

    public void EquipWeapon(WeaponInfo newWeapon)
    {
        if (IsServer)
        {
            currentWeaponName.Value = newWeapon != null ? new FixedString64Bytes(newWeapon.itemName) : default;
            currentAmmo.Value = newWeapon != null ? newWeapon.maxMunition : 0;
            fireRateCounterNet.Value = 0f;
            isShootingNet.Value = false;
        }

        DeactivateCurrentWeapon();

        currentWeapon = newWeapon;

        if (currentWeapon != null)
        {
            ActivateNewWeapon();
            anim.SetLayerWeight(1, 1f);
            anim.SetBool("withoutWeapon", false);
            anim.SetInteger("WeaponState", 1);
            anim.SetBool(currentWeapon.animatorParameter, true);
        }
        else
        {
            anim.SetLayerWeight(1, 0f);
            anim.SetBool("withoutWeapon", true);
        }

        OnWeaponChanged?.Invoke();
    }

    private void DeactivateCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            Transform weaponTransform = FindWeaponTransform(currentWeapon.itemName);
            if (weaponTransform != null)
            {
                weaponTransform.gameObject.SetActive(false);
            }
        }
    }

    private void ActivateNewWeapon()
    {
        if (currentWeapon != null)
        {
            Transform weaponTransform = FindWeaponTransform(currentWeapon.itemName);
            if (weaponTransform != null)
            {
                weaponTransform.gameObject.SetActive(true);
            }
        }
    }

    private Transform FindWeaponTransform(string weaponModelName)
    {
        return weaponsContainer.Find(weaponModelName);
    }

    private void HandleInput()
    {
        if (currentWeapon == null) return;

        if (input.reload && currentAmmo.Value < currentWeapon.maxMunition)
        {
            StartCoroutine(Reload());
        }
    }

    private bool CanShoot()
    {
        return input.shoot && fireRateCounterNet.Value >= currentWeapon.cadence && currentWeapon != null;
    }

    private Vector3 GetShootDirection(Vector3 aimPoint)
    {
        Vector3 shootDirection = (aimPoint - bulletSpawner.position).normalized;
        shootDirection.y = 0;
        shootDirection += new Vector3(UnityEngine.Random.Range(-currentWeapon.spread, currentWeapon.spread), 0, 0);
        return shootDirection.normalized;
    }

    private IEnumerator Reload()
    {
        anim.SetInteger("WeaponState", 5);
        yield return new WaitForSeconds(currentWeapon.reloadSpeed);
        currentAmmo.Value = currentWeapon.maxMunition;
        anim.SetInteger("WeaponState", input.aim ? 3 : 2);
    }

    private void AdjustTorsoAimWeight()
    {
        float targetWeight = input.aim ? 1f : 0f;
        float currentWeight = torsoAimConstraint.weight;
        torsoAimConstraint.weight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * aimWeightChangeSpeed);
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 aimPoint)
    {
        // Execute the shooting logic on the server
        HandleShooting(aimPoint);
        NotifyShootClientRpc();
    }

    [ClientRpc]
    private void NotifyShootClientRpc()
    {
        // Update all clients with the shooting action
        OnShoot?.Invoke();
    }
}
