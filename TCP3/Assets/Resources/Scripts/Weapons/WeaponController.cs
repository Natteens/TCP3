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

    public GameObject fuzilWeapon;
    public GameObject fuzilRajadaWeapon;
    public GameObject revolverWeapon;
    public GameObject subMetralhadoraWeapon;
    public GameObject rifleWeapon;
    public GameObject escopetaWeapon;

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
        if (currentWeapon == null || !IsOwner) return;
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
                ulong shooterId = GetComponent<NetworkObject>().NetworkObjectId;
                string projectileName = Projectile(); 
                for (int i = 0; i < currentWeapon.bulletPerShoot; i++)
                {
                    Vector3 shootDirection = GetShootDirection(aimPoint, i, currentWeapon.bulletPerShoot, currentWeapon.spread);
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
                if (currentWeapon.itemName == "Supernova")
                {
                  return"projectile2";
                }   
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
        currentWeapon = null;
        DeactivateAllWeapons();
    }

    private void ActivateNewWeapon()
    {
        if (currentWeapon != null)
        {
            DeactivateAllWeapons();

            switch (currentWeapon.weaponType)
            {
                case WeaponType.Fuzil:
                    fuzilWeapon.SetActive(true);
                    break;
                case WeaponType.FuzilRajada:
                    fuzilRajadaWeapon.SetActive(true);
                    break;
                case WeaponType.Revolver:
                    revolverWeapon.SetActive(true);
                    break;
                case WeaponType.SubMetralhadora:
                    subMetralhadoraWeapon.SetActive(true);
                    break;
                case WeaponType.Rifle:
                    rifleWeapon.SetActive(true);
                    break;
                case WeaponType.Escopeta:
                    escopetaWeapon.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    private void DeactivateAllWeapons()
    {
        fuzilWeapon.SetActive(false);
        fuzilRajadaWeapon.SetActive(false);
        revolverWeapon.SetActive(false);
        subMetralhadoraWeapon.SetActive(false);
        rifleWeapon.SetActive(false);
        escopetaWeapon.SetActive(false);
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
        if (input.shoot && input.aim && fireRateCounter >= currentWeapon.cadence && currentWeapon != null && canShoot)
        {
             return true;
        }
        else
        {
            return false;
        }
    }

    private Vector3 GetShootDirection(Vector3 aimPoint, int bulletIndex, int totalBullets, float spread)
    {
        // Dire��o padr�o do tiro
        Vector3 shootDirection = (aimPoint - bulletSpawner.position).normalized;

        // Aplica spread apenas para armas com mais de um tiro por disparo
        if (totalBullets > 1)
        {
            float spreadFactor = spread * ((float)bulletIndex / (totalBullets - 1)) - spread / 2;
            shootDirection = Quaternion.Euler(0, spreadFactor * 15, 0) * shootDirection; // Spread moderado
        }

        // Garante que o Y seja sempre 0 para que o tiro permane�a no plano XZ
        shootDirection.y = 0;
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
            Vector3 shootDirection = GetShootDirection(Vector3.zero, 0, currentWeapon.bulletPerShoot, currentWeapon.spread);
            Gizmos.DrawLine(bulletSpawner.position, bulletSpawner.position + shootDirection * 10f);
            Gizmos.DrawSphere(bulletSpawner.position + shootDirection * 10f, 0.1f);
        }
    }
}