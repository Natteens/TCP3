using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private WeaponInfo currentWeapon;
    [SerializeField] private Transform weaponsContainer; // Objeto pai que contém todas as armas
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private MultiAimConstraint torsoAimConstraint;
    [SerializeField] private TPSController tpsController;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask layer;
    private StarterAssetsInputs input;
    private float aimWeightChangeSpeed = 5f;
    private int currentAmmo;
    private bool isShooting;
    private float fireRateCounter;

    public event Action OnWeaponChanged;
    public event Action OnShoot;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        EquipWeapon(currentWeapon);
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
        FireCounterTimer();
    }

    private void FireCounterTimer()
    {
        if (currentWeapon != null && fireRateCounter < 2f)
            fireRateCounter += Time.fixedDeltaTime;
    }

    private void HandleShooting(Vector3 aimPoint)
    {
        tpsController.RotateTowardsMouseSmooth(aimPoint);

        if (fireRateCounter >= currentWeapon.cadence)
        {
            if (currentAmmo > 0)
            {
                if (!isShooting) isShooting = true;
                Vector3 shootDirection = GetShootDirection(aimPoint);
                Instantiate(currentWeapon.bulletPrefab, bulletSpawner.position, Quaternion.LookRotation(shootDirection, Vector3.up));
                currentAmmo -= currentWeapon.bulletPerShoot;
                fireRateCounter = 0f;
                OnShoot?.Invoke();
                Debug.DrawLine(bulletSpawner.position, bulletSpawner.position + shootDirection * 10f, Color.yellow, 2f);
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
        DeactivateCurrentWeapon();

        currentWeapon = newWeapon;
        currentAmmo = newWeapon != null ? newWeapon.maxMunition : 0;
        fireRateCounter = 0f;
        isShooting = false;

        if (currentWeapon != null)
        {
            ActivateNewWeapon();

            anim.SetLayerWeight(1, 1f);
            anim.SetBool("withoutWeapon", false);
            anim.SetInteger("WeaponState", 1); // Pegar a arma
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
        // espero q não de bug na hora de fazer o network disso
        // mas se a arma de um jogador estiver sendo afetada 
        // por outro jogador pode ser isso aq por causa do Find 
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
        return input.shoot && fireRateCounter >= currentWeapon.cadence && currentWeapon != null;
    }

    private Vector3 GetShootDirection(Vector3 aimPoint)
    {
        Vector3 shootDirection = (aimPoint - bulletSpawner.position).normalized;
        shootDirection.y = 0;
        shootDirection += new Vector3(UnityEngine.Random.Range(-currentWeapon.spread, currentWeapon.spread),0,0);
        return shootDirection.normalized;
    }

    private IEnumerator Reload()
    {
       // anim.SetInteger("WeaponState", 5); // Atualizar animação para "recarregando"
        yield return new WaitForSeconds(currentWeapon.reloadSpeed);
        currentAmmo = currentWeapon.maxMunition;
        anim.SetInteger("WeaponState", input.aim ? 3 : 2); // Voltar ao estado de segurar a arma após recarregar
    }

    private void AdjustTorsoAimWeight()
    {
        float targetWeight = input.aim ? 1f : 0f; 
        float currentWeight = torsoAimConstraint.weight;
        torsoAimConstraint.weight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * aimWeightChangeSpeed);
    }
}
