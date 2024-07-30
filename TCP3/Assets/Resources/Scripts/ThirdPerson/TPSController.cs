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
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform pfBulletProjectile;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    [SerializeField] private float speedRotate;
    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }
    private void Update()
    {
        Vector3 mouseWorldPosition = Raycast();
        Aim(mouseWorldPosition);
        Shoot(mouseWorldPosition);
    }

    private void Shoot(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.shoot)
        {
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Rotation(LookDir(mouseWorldPosition), speedRotate);
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }
    }

    private void Aim(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            Vector3 aimDirection = LookDir(mouseWorldPosition);
            Rotation(aimDirection, 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetSensitivity(normalSensitivity);
        }
    }

    private Vector3 Raycast()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycasthit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycasthit.point;
            debugTransform.position = raycasthit.point;
        }

        return mouseWorldPosition;
    }

    private Vector3 LookDir(Vector3 mouseWorldPosition)
    {
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        return aimDirection;
    }

    private void Rotation(Vector3 aimDirection, float speed)
    {
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * speed);
    }
}
