using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] public WeaponInfo currentWeapon;
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private MultiAimConstraint torsoAimConstraint;
    [SerializeField] private TPSController tpsController;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask layer;
    [SerializeField] private EventComponent events;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float defaultAimOffsetY = -30f;

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

    [ServerRpc(RequireOwnership = false)]
    public void StartingWeaponServerRpc()
    {
        if(currentWeapon != null)
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
                ulong shooterId = GetComponent<NetworkObject>().NetworkObjectId;
                string projectileName = Projectile(); 
                for (int i = 0; i < currentWeapon.bulletPerShoot; i++)
                {
                    Spawner.Instance.SpawnProjectilesServerRpc(bulletSpawner.position, shootDirection, projectileName, currentWeapon.damage, shooterId); 
                }
                currentAmmo--;
                fireRateCounter = 0f;
                OnShoot?.Invoke();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    private string Projectile()
    {
        switch (currentWeapon.weaponType)
        {
            case WeaponType.Fuzil:
                return "projectileBasic";
            case WeaponType.FuzilRajada:
                return "projectileBasic";
            case WeaponType.Revolver:
                return "projectileBasic";
            case WeaponType.SubMetralhadora:
                return "projectile5";
            case WeaponType.Rifle:
                return "projectile4";
            case WeaponType.Escopeta:
                return "projectile5";
            default:
                return "projectileBasic"; 
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
        AdjustAimOffset();
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
        Vector3 directionToAim = (aimPoint - bulletSpawner.position).normalized;
        directionToAim.y = 0; 
        bulletSpawner.forward = directionToAim; 
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

    public void DeactivateCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            Transform weaponTransform = FindWeaponTransform(currentWeapon.weaponType.ToString());
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
        foreach (Transform weapon in weaponsContainer.GetComponentsInChildren<Transform>(true))
        {
            if (weapon.name == weaponModelName)
            {
                return weapon;
            }
        }
        return null;
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
        Vector3 shootDirection = (aimPoint - bulletSpawner.position).normalized;
        shootDirection.y = 0;
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

    private void AdjustAimOffset()
    {
        if (input.move != Vector2.zero)
        {
            currentAimOffsetY = 0;
        }
        else
        {
            currentAimOffsetY = defaultAimOffsetY;
        }
        torsoAimConstraint.data.offset = new Vector3(0f, currentAimOffsetY, 0f);
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (bulletSpawner != null)
        {
            Gizmos.color = Color.red;
            Vector3 shootDirection = GetShootDirection(Vector3.zero); 
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

        var (success, position) = MouseController.GetMousePosition(Camera.main, layer);
        if (success)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(position, 0.2f);
            Gizmos.DrawLine(transform.position, position);
        }

        Gizmos.color = Color.cyan;
        Vector3 directionToAim = (position - transform.position).normalized;
        Gizmos.DrawLine(transform.position, transform.position + directionToAim * 10f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * 10f);

        if (bulletSpawner != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 bulletSpawnerDirection = (position - bulletSpawner.position).normalized;
            bulletSpawnerDirection.y = 0;
            Debug.DrawRay(bulletSpawner.position, bulletSpawnerDirection * 10f);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, Vector3.up * currentAimOffsetY);
    }
}