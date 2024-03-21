using Cinemachine;
using UnityEngine;
using Unity.Netcode;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
    public GameObject RigtargetPosition;
    private ThirdPersonController player;
    private StarterAssetsInputs starterAssets;
    private CinemachineVirtualCamera aimCamera;
    private CinemachineVirtualCamera thirdPersonCamera;
    private Canvas aimCanvas;
    private Canvas thirdPersonCanvas;

    [SerializeField] private float normalSensitivity = 5f;
    [SerializeField] private float aimSensitivity = 2f;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private float maxRaycastDistance = 100f;

    private void Awake()
    {
        player = GetComponent<ThirdPersonController>();
        starterAssets = GetComponent<StarterAssetsInputs>();
        aimCamera = player._AimVirtualCamera;
        thirdPersonCamera = player._PlayerFollowVirtualCamera; 
        aimCanvas = aimCamera.GetComponentInChildren<Canvas>();
        thirdPersonCanvas = thirdPersonCamera.GetComponentInChildren<Canvas>();
    }

    private void Update()
    {
        UpdateAim();
    }

    private void UpdateAim()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * maxRaycastDistance, Color.red);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxRaycastDistance, aimColliderLayerMask))
        {
            RigtargetPosition.transform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
        bool isAiming = starterAssets.aim;
        aimCamera.gameObject.SetActive(isAiming);
        player.SetSensitivity(isAiming ? aimSensitivity : normalSensitivity);
        
        aimCanvas.gameObject.SetActive(isAiming);
        thirdPersonCanvas.gameObject.SetActive(!isAiming);

        if (isAiming)
        {
            player._animator.SetLayerWeight(1, Mathf.Lerp(player._animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //Vector3 aimDirection = (mouseWorldPosition - transform.position).normalized;
            //aimDirection.y = 0f; // Restringe a direção apenas ao plano horizontal

            //Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 2000f);
        }
        else
        {
            player._animator.SetLayerWeight(1, Mathf.Lerp(player._animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }
}
// rotação esta zuada