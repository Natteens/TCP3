using Cinemachine;
using UnityEngine;
using Unity.Netcode;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
    public ThirdPersonController player;
    public CinemachineVirtualCamera aimCamera;
    public GameObject reticle;
    private StarterAssetsInputs starterAssets;

    [SerializeField] private float normalSensitivity = 5f;
    [SerializeField] private float aimSensitivity = 2f;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private float maxRaycastDistance = 100f;
    [SerializeField] private Transform debugTransform;

    private void Awake()
    {
        player = GetComponent<ThirdPersonController>();
        starterAssets = GetComponent<StarterAssetsInputs>();
        aimCamera = player._AimVirtualCamera;     

        reticle.SetActive(false);   
    }

    private void Update()
    {
        UpdateAim();
    }

    private void UpdateAim()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxRaycastDistance, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        bool isAiming = starterAssets.aim;
        aimCamera.gameObject.SetActive(isAiming);
        player.SetSensitivity(isAiming ? aimSensitivity : normalSensitivity);
        if (isAiming)
            player._animator.SetLayerWeight(1, Mathf.Lerp(player._animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        else
            player._animator.SetLayerWeight(1, Mathf.Lerp(player._animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

        if (reticle != null)
        {
            reticle.SetActive(isAiming);
            if (isAiming)
            {
                reticle.transform.position = mouseWorldPosition;
            }
        }
    }
}
