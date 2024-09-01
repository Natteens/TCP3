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

    private StarterAssetsInputs input;
    private int currentAmmo;
    private bool isShooting;
    private float fireRateCounter;
    private bool canShoot;
    private float currentAimOffsetY;

    private const float ANIM_STATE_EQUIP = 0f;
    private const float ANIM_STATE_HOLD = 1f;
    private const float ANIM_STATE_AIM = 2f;

    public event Action OnWeaponChanged;
    public event Action OnShoot;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
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
        if (currentWeapon != null && fireRateCounter < currentWeapon.cadence)
            fireRateCounter += Time.fixedDeltaTime;
    }

    private void HandleShooting(Vector3 aimPoint)
    {
        if (input.shoot && input.aim && fireRateCounter >= currentWeapon.cadence && canShoot)
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
            anim.SetFloat("WeaponState", input.aim ? ANIM_STATE_AIM : ANIM_STATE_HOLD);
            isShooting = false;
        }
    }

    private void HandleAiming(Vector3 aimPoint)
    {
        if (input.aim)
        {
            anim.SetLayerWeight(1, 1f);
            anim.SetFloat("WeaponState", ANIM_STATE_AIM);
            AdjustCharacterRotation(aimPoint);
            AdjustBulletSpawnerRotation(aimPoint);
            EnableShooting();
        }
        else
        {
            anim.SetLayerWeight(1, input.move != Vector2.zero ? 0f : 1f);
            anim.SetFloat("WeaponState", input.move == Vector2.zero ? ANIM_STATE_HOLD : ANIM_STATE_EQUIP);
            DisableShooting();
        }
    }

    private void AdjustCharacterRotation(Vector3 aimPoint)
    {
        Vector3 directionToAim = (aimPoint - transform.position).normalized;
        directionToAim.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToAim, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void AdjustBulletSpawnerRotation(Vector3 aimPoint)
    {
        // Certificar que o bulletSpawner esteja sempre apontando para o ponto de mira
        Vector3 directionToAim = (aimPoint - bulletSpawner.position).normalized;
        directionToAim.y = 0; // Garantir que o tiro esteja sempre na horizontal
        bulletSpawner.forward = directionToAim; // Alinhar o bulletSpawner na direção de tiro
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
            anim.SetFloat("WeaponState", ANIM_STATE_EQUIP, 0.1f, Time.deltaTime);
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

        if (input.reload && currentAmmo < currentWeapon.maxMunition)
        {
            StartCoroutine(Reload());
        }
    }

    private bool CanShoot()
    {
        return input.shoot && input.aim && fireRateCounter >= currentWeapon.cadence && currentWeapon != null && canShoot;
    }

    private Vector3 GetShootDirection(Vector3 aimPoint)
    {
        // Calcular a direção do tiro como uma linha reta entre o bulletSpawner e o ponto de mira
        Vector3 shootDirection = (aimPoint - bulletSpawner.position).normalized;
        shootDirection.y = 0;
        // Aplicar o spread, se necessário
        float spreadFactor = UnityEngine.Random.Range(-currentWeapon.spread, currentWeapon.spread);
        shootDirection += new Vector3(spreadFactor, 0, spreadFactor);

        return shootDirection.normalized;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(currentWeapon.reloadSpeed);
        currentAmmo = currentWeapon.maxMunition;
        anim.SetFloat("WeaponState", ANIM_STATE_HOLD, 0.1f, Time.deltaTime);
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
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.magenta); // Linha rosa (direção do personagem)

        Vector3 bulletSpawnerDirection = (aimPoint - bulletSpawner.position).normalized;
        bulletSpawnerDirection.y = 0;
        Debug.DrawRay(bulletSpawner.position, bulletSpawnerDirection * 10f, Color.yellow); // Linha amarela (direção do tiro)
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Desenhar a direção do tiro
        if (bulletSpawner != null)
        {
            Gizmos.color = Color.red;
            Vector3 shootDirection = GetShootDirection(Vector3.zero); // Usar o ponto de mira real
            Gizmos.DrawLine(bulletSpawner.position, bulletSpawner.position + shootDirection * 10f);
            Gizmos.DrawSphere(bulletSpawner.position + shootDirection * 10f, 0.1f);
        }

        // Desenhar o ponto de mira
        if (torsoAimConstraint != null)
        {
            Gizmos.color = Color.blue;
            Vector3 aimPoint = torsoAimConstraint.data.offset;
            Gizmos.DrawSphere(transform.position + aimPoint, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position + aimPoint);
        }

        // Desenhar o ponto de mira calculado pelo MouseController
        var (success, position) = MouseController.GetMousePosition(Camera.main, layer);
        if (success)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(position, 0.2f);
            Gizmos.DrawLine(transform.position, position);
        }

        // Desenhar direção para o ponto de mira
        Gizmos.color = Color.cyan;
        Vector3 directionToAim = (position - transform.position).normalized;
        Gizmos.DrawLine(transform.position, transform.position + directionToAim * 10f);

        // Desenhar o raio da posição de origem do personagem
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * 10f);

        // Desenhar direção de mira do bulletSpawner
        if (bulletSpawner != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 bulletSpawnerDirection = (position - bulletSpawner.position).normalized;
            bulletSpawnerDirection.y = 0;
            Debug.DrawRay(bulletSpawner.position, bulletSpawnerDirection * 10f);
        }

        // Debug para o vetor de offset da mira
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, Vector3.up * currentAimOffsetY);
    }
}