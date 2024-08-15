using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponInfo currentWeapon;
    [SerializeField] private Transform weaponsContainer; // Objeto pai que contém todas as armas
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private MultiAimConstraint torsoAimConstraint;
    [SerializeField] private TPSController tpsController;
    [SerializeField] private Animator anim;
    private StarterAssetsInputs input;
    private IsometricAiming aimController;
    private float aimWeightChangeSpeed = 5f;
    private int currentAmmo;
    private bool isShooting;
    private float fireRateCounter;

    public event Action OnWeaponChanged;
    public event Action OnShoot;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        aimController = GetComponent<IsometricAiming>();
    }

    private void Start()
    {
        EquipWeapon(currentWeapon);
    }

    private void Update()
    {
        HandleInput();
        var (success, position) = aimController.GetMousePosition();
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
        if (fireRateCounter >= currentWeapon.cadence)
        {
            if (currentAmmo > 0)
            {
                if (!isShooting)
                {
                    isShooting = true;
                    anim.SetFloat("WeaponState", 4f); // Atualizar animação para "atirando"
                }

                tpsController.RotateTowardsMouseInstant(aimPoint);
                Vector3 shootDirection = GetShootDirection();
                Instantiate(currentWeapon.bulletPrefab, bulletSpawner.position, Quaternion.LookRotation(shootDirection, Vector3.up));
                currentAmmo -= currentWeapon.bulletPerShoot;
                fireRateCounter = 0f;

                OnShoot?.Invoke();

                Debug.DrawLine(bulletSpawner.position, bulletSpawner.position + shootDirection * 10f, Color.yellow, 2f);
            }
            else
            {
                StartCoroutine(Reload()); // Recarregar se a munição acabar
            }
        }
    }

    private void StopShooting()
    {
        if (isShooting)
        {
            isShooting = false;
            anim.SetFloat("WeaponState", input.aim ? 3f : 2f); // Voltar ao estado de segurar a arma (não atirando)
        }
    }

    private void HandleAiming(Vector3 aimPoint)
    {
        if (input.aim)
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            anim.SetFloat("WeaponState", 3f); // Aiming com a arma
            tpsController.RotateTowardsMouseSmooth(aimPoint);
        }
        else
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            anim.SetFloat("WeaponState", 2f); // Segurando a arma, mas sem mirar
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
            anim.SetFloat("WeaponState", 1f); // Pegar a arma
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
            Transform weaponTransform = FindWeaponTransform(currentWeapon.weaponModel.name);
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
            Transform weaponTransform = FindWeaponTransform(currentWeapon.weaponModel.name);
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

    private Vector3 GetShootDirection()
    {
        Vector3 shootDirection = tpsController.transform.forward;

        shootDirection += new Vector3(
            UnityEngine.Random.Range(-currentWeapon.spread, currentWeapon.spread), 0, 0);

        return shootDirection.normalized;
    }

    private IEnumerator Reload()
    {
        anim.SetFloat("WeaponState", 5f); // Atualizar animação para "recarregando"
        yield return new WaitForSeconds(currentWeapon.reloadSpeed);
        currentAmmo = currentWeapon.maxMunition;
        anim.SetFloat("WeaponState", input.aim ? 3f : 2f); // Voltar ao estado de segurar a arma após recarregar
    }

    private void AdjustTorsoAimWeight()
    {
        float targetWeight = input.aim ? 1f : 0f;
        float currentWeight = torsoAimConstraint.weight;
        torsoAimConstraint.weight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * aimWeightChangeSpeed);
    }
}
