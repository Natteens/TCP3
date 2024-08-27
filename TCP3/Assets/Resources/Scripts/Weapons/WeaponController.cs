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
    [SerializeField] private EventComponent events;
    private StarterAssetsInputs input;
    private int currentAmmo;
    private bool isShooting;
    private float fireRateCounter;
    private bool canShoot; // Variável para controle do disparo baseado em animação

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
        if (IsAimBeyondThreshold(aimPoint))
        {
            tpsController.RotateTowardsMouseSmooth(aimPoint);
        }

        if (fireRateCounter >= currentWeapon.cadence && canShoot)
        {
            if (currentAmmo > 0)
            {
                if (!isShooting) isShooting = true;
                Vector3 shootDirection = GetShootDirection(aimPoint);
                var projectile = Instantiate(currentWeapon.bulletPrefab, bulletSpawner.position, Quaternion.LookRotation(shootDirection, Vector3.up));
                projectile.GetComponent<ProjectileMover>().InitializeProjectile((int)currentWeapon.damage);
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

            if (!IsAimBeyondThreshold(aimPoint))
            {
                AdjustTorsoOnlyAim(aimPoint);
            }
            else
            {
                tpsController.RotateTowardsMouseSmooth(aimPoint);
            }
        }
        else
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            anim.SetInteger("WeaponState", 2);
            DisableShooting();
        }
    }

    private void AdjustTorsoOnlyAim(Vector3 aimPoint)
    {
        Vector3 aimDirection = (aimPoint - transform.position).normalized;
        aimDirection.y = 0; // Ignora a rotação no eixo Y
        transform.rotation = Quaternion.LookRotation(aimDirection);
    }

    private bool IsAimBeyondThreshold(Vector3 aimPoint)
    {
        Vector3 aimDirection = (aimPoint - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, aimDirection);
        return angle > 90f; // Checa se o ângulo de rotação é maior que 90 graus
    }

    public void EquipWeapon(WeaponInfo newWeapon)
    {
        DeactivateCurrentWeapon();

        currentWeapon = newWeapon;
        currentAmmo = newWeapon != null ? newWeapon.maxMunition : 0;
        fireRateCounter = 0f;
        isShooting = false;
        canShoot = false; // Define que o jogador não pode atirar inicialmente

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
        shootDirection.y = 0;

        float spreadFactor = UnityEngine.Random.Range(-currentWeapon.spread, currentWeapon.spread);
        Vector3 spreadOffset = new Vector3(spreadFactor, 0, spreadFactor);
        shootDirection += spreadOffset;

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
        torsoAimConstraint.weight = input.aim ? 1f : 0f;
    }

    // Método para ser chamado pelo evento de animação
    public void EnableShooting()
    {
        canShoot = true;
    }

    public void DisableShooting()
    {
        canShoot = false;
    }

    // Novo método para ajustar a rotação do personagem com base na posição do mouse
    private void AdjustCharacterRotation(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        directionToTarget.y = 0; // Ignora a rotação no eixo Y para manter a rotação plana
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), Time.deltaTime * 10f);
    }

    // Exibição do Raycast da linha de mira
    private void DisplayAimingRay(Vector3 aimPoint)
    {
        Vector3 directionToAim = (aimPoint - bulletSpawner.position).normalized;
        Debug.DrawRay(bulletSpawner.position, directionToAim * 10f, Color.red);
    }
}
