using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;

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
    //[SerializeField] private float maxAimAngle = 180f;
    //[SerializeField] private float defaultAimOffsetY = -30f;
    //[SerializeField] private float movingAimOffsetYLeft = -55f;
    //[SerializeField] private float movingAimOffsetYRight = -45f;
    //[SerializeField] private float aimOffsetTransitionSpeed = 5f;

    [SerializeField]
    private float rotationOffset = 0f; // Adicione um offset para ajuste fino da rotação


    private StarterAssetsInputs input;
    private int currentAmmo;
    private bool isShooting;
    private float fireRateCounter;
    private bool canShoot;
    private float targetAimOffsetY;
    private float currentAimOffsetY;

    private const float ANIM_STATE_EQUIP = 0f;
    private const float ANIM_STATE_HOLD = 1f;
    private const float ANIM_STATE_AIM = 2f;

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
            // Transição suave para o estado de "segurando a arma" ao parar de atirar
            anim.SetFloat("WeaponState", input.aim ? ANIM_STATE_AIM : ANIM_STATE_HOLD);
            isShooting = false;
        }
    }

    private void HandleAiming(Vector3 aimPoint)
    {
        if (input.aim)
        {
            // Sempre ativa a camada de arma durante a mira
            anim.SetLayerWeight(1, 1f);
            // Transição suave para o estado de "mirar com a arma"
            anim.SetFloat("WeaponState", ANIM_STATE_AIM);

            // Ajuste a rotação do personagem com base no ponto de mira
            AdjustCharacterRotation(aimPoint);
        }
        else
        {
            // Reduz o peso da camada de arma ao não mirar
            anim.SetLayerWeight(1, input.move != Vector2.zero ? 0f : 1f);
            anim.SetFloat("WeaponState", input.move == Vector2.zero ? ANIM_STATE_HOLD : ANIM_STATE_EQUIP);
            DisableShooting();
        }

    }

    private void AdjustCharacterRotation(Vector3 aimPoint)
    {
        // Calcula a direção do ponto de mira em relação à posição atual do personagem
        Vector3 dir = (aimPoint - transform.position).normalized;
        dir.y = 0; // Ignora a diferença de altura para manter a rotação no plano horizontal
        // Calcula a rotação alvo com base na direção corrigida
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        
        // Aplica a rotação diretamente ao personagem
        transform.rotation = targetRotation;
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
        return input.shoot && input.aim && fireRateCounter >= currentWeapon.cadence && currentWeapon != null && canShoot;
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
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.magenta);

        Vector3 bulletSpawnerDirection = (aimPoint - bulletSpawner.position).normalized;
        bulletSpawnerDirection.y = 0;
        Debug.DrawRay(bulletSpawner.position, bulletSpawnerDirection * 10f, Color.yellow);
    }
}
