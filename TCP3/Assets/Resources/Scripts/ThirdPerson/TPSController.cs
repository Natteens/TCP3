using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class TPSController : MonoBehaviour
{
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private float rotationSpeed = 100f;

    private StarterAssetsInputs starterAssetsInputs;
    private IsometricAiming aimController;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        aimController = GetComponent<IsometricAiming>();
    }

    private void Update()
    {
        var (success, position) = aimController.GetMousePosition();
        if (success)
        {
            HandleAiming(position);
            HandleShooting(position);
        }
    }

    private void RotateTowardsMouseSmooth(Vector3 aimPoint)
    {
        Vector3 aimDirection = (aimPoint - transform.position).normalized;
        aimDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void RotateTowardsMouseInstant(Vector3 aimPoint)
    {
        Vector3 aimDirection = (aimPoint - transform.position).normalized;
        aimDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
        transform.rotation = targetRotation;
    }

    private void HandleShooting(Vector3 aimPoint)
    {
        if (starterAssetsInputs.shoot)
        {
            // Rotaciona instantaneamente para o ponto de tiro
            RotateTowardsMouseInstant(aimPoint);

            // Dispara o projétil na direção para onde o personagem está virado
            Vector3 shootDirection = transform.forward;
            Instantiate(bulletProjectilePrefab, spawnBulletPosition.position, Quaternion.LookRotation(shootDirection, Vector3.up));
            starterAssetsInputs.shoot = false;

            Debug.DrawLine(spawnBulletPosition.position, spawnBulletPosition.position + shootDirection * 10f, Color.yellow, 2f);
        }
    }

    private void HandleAiming(Vector3 aimPoint)
    {
        if (starterAssetsInputs.aim)
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            RotateTowardsMouseSmooth(aimPoint);
        }
        else
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }
}
