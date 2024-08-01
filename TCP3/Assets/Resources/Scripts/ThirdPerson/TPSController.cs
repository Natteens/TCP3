using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class TPSController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform pfBulletProjectile;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private AimController aimController;
    [SerializeField] private float speedRotate;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        aimController = GetComponent<AimController>();
    }

    private void Update()
    {
        Vector3 aimPoint = aimController.GetAimPoint();
        HandleAiming(aimPoint);
        HandleShooting(aimPoint);
    }

    private void HandleShooting(Vector3 aimPoint)
    {
        if (starterAssetsInputs.shoot)
        {
            Vector3 aimDir = (aimPoint - spawnBulletPosition.position).normalized;
            aimController.RotateTowards(aimController.GetLookDirection(aimPoint, transform), transform, speedRotate);
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;

            // Desenhar linha de depuração para visualizar o trajeto do projétil
            Debug.DrawLine(spawnBulletPosition.position, spawnBulletPosition.position + aimDir * 10f, Color.yellow, 2f);
        }
    }

    private void HandleAiming(Vector3 aimPoint)
    {
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            Vector3 aimDirection = aimController.GetLookDirection(aimPoint, transform);
            aimController.RotateTowards(aimDirection, transform, 20f);
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetSensitivity(normalSensitivity);
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }
}
