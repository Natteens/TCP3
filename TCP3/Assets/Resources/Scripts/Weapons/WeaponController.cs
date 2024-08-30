using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private WeaponInfo currentWeapon;
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private MultiAimConstraint torsoAimConstraint;
    [SerializeField] private TPSController tpsController;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask layer;
    [SerializeField] private EventComponent events;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxAimAngle = 180f;
    [SerializeField] private float defaultAimOffsetY = -30f;
    [SerializeField] private float movingAimOffsetYLeft = -55f;
    [SerializeField] private float movingAimOffsetYRight = -45f;
    [SerializeField] private float aimOffsetTransitionSpeed = 5f;

    private StarterAssetsInputs input;
    private int currentAmmo;
    private bool isShooting;
    private float fireRateCounter;
    private bool canShoot;
    private float targetAimOffsetY;
    private float currentAimOffsetY;

    public event Action OnWeaponChanged;
    public event Action OnShoot;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        events.OnShootingWithWeapon += EnableShooting;
    }

    private void Start()
    {
        StartingWeaponServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartingWeaponServerRpc()
    {
        EquipWeapon(currentWeapon);
    }

    private void Update()
    {
        WeaponUpdateServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void WeaponUpdateServerRpc()
    {
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

        DisplayDebugRays(position);
    }

    private void FixedUpdate()
    {
        FireCounterTimer();
    }

    private void FireCounterTimer()
    {
        if (currentWeapon != null && fireRateCounter < 2f)
            fireRateCounter += Time.fixedDeltaTime;
    }

    private void HandleShooting(Vector3 aimPoint)
    {
        if (fireRateCounter >= currentWeapon.cadence && canShoot)
        {
            if (currentAmmo > 0)
            {
                if (!isShooting) isShooting = true;
                Vector3 shootDirection = GetShootDirection(aimPoint);
                Spawner.Instance.SpawnProjectilesServerRpc(bulletSpawner.position, shootDirection, "projectileBasic", currentWeapon.damage);
                currentAmmo -= currentWeapon.bulletPerShoot;
                fireRateCounter = 0f;
                OnShoot?.Invoke();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    private void StopShooting()
    {
        if (isShooting)
        {
            anim.SetInteger("WeaponState", input.aim ? 3 : 2);
            isShooting = false;
        }
    }

    private void HandleAiming(Vector3 aimPoint)
    {
        if (input.aim)
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            anim.SetInteger("WeaponState", 3);

            // Ajustar a rotação apenas quando mirando
            AdjustCharacterRotation(aimPoint);
        }
        else
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            anim.SetInteger("WeaponState", 2);
            DisableShooting();
        }

        AdjustAimOffset();
    }

    private void AdjustCharacterRotation(Vector3 aimPoint)
    {
        Vector3 directionToAim = (aimPoint - transform.position).normalized;
        directionToAim.y = 0;

        float angle = Vector3.SignedAngle(transform.forward, directionToAim, Vector3.up);
        angle = Mathf.Clamp(angle, -maxAimAngle, maxAimAngle);

        Vector3 targetDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void AdjustAimOffset()
    {
        // Ajustar o offset da mira (Y) com base no movimento
        if (input.move != Vector2.zero)
        {
            if (input.move.x < 0)
            {
                targetAimOffsetY = movingAimOffsetYLeft;
            }
            else if (input.move.x > 0)
            {
                targetAimOffsetY = movingAimOffsetYRight;
            }
            else
            {
                targetAimOffsetY = (movingAimOffsetYLeft + movingAimOffsetYRight) / 2f;
            }
        }
        else
        {
            targetAimOffsetY = defaultAimOffsetY;
        }

        currentAimOffsetY = Mathf.Lerp(currentAimOffsetY, targetAimOffsetY, Time.deltaTime * aimOffsetTransitionSpeed);
        torsoAimConstraint.data.offset = new Vector3(0f, currentAimOffsetY, 0f);
    }

    public void EquipWeapon(WeaponInfo newWeapon)
    {
        DeactivateCurrentWeapon();

        currentWeapon = newWeapon;
        currentAmmo = newWeapon != null ? newWeapon.maxMunition : 0;
        fireRateCounter = 0f;
        isShooting = false;
        canShoot = false;

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
        Transform weaponTransform = weaponsContainer.Find(weaponModelName);
        return weaponTransform;
    }

    private void HandleInput()
    {
        if (currentWeapon == null) return;

        if (input.reload && currentAmmo < currentWeapon.maxMunition)
        {
            StartCoroutine(Reload());
        }
    }

    private bool CanShoot()
    {
        return input.shoot && fireRateCounter >= currentWeapon.cadence && currentWeapon != null && canShoot;
    }

    private Vector3 GetShootDirection(Vector3 aimPoint)
    {
        Vector3 shootDirection = (aimPoint - bulletSpawner.position).normalized;
        shootDirection.y = 0; // Mantém o projétil na mesma altura

        float spreadFactor = UnityEngine.Random.Range(-currentWeapon.spread, currentWeapon.spread);
        Vector3 spreadOffset = new Vector3(spreadFactor, 0, spreadFactor);
        shootDirection += spreadOffset;

        return shootDirection.normalized;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(currentWeapon.reloadSpeed);
        currentAmmo = currentWeapon.maxMunition;
        anim.SetInteger("WeaponState", input.aim ? 3 : 2);
    }

    private void AdjustTorsoAimWeight()
    {
        torsoAimConstraint.weight = input.aim ? 1f : 0f;
    }

    public void EnableShooting()
    {
        canShoot = true;
    }

    public void DisableShooting()
    {
        canShoot = false;
    }

    private void DisplayDebugRays(Vector3 aimPoint)
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.magenta);

        Vector3 bulletSpawnerDirection = (aimPoint - bulletSpawner.position).normalized;
        bulletSpawnerDirection.y = 0;
        Debug.DrawRay(bulletSpawner.position, bulletSpawnerDirection * 10f, Color.yellow);
    }
}
