using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Unity.Netcode;

public class TPSController : NetworkBehaviour
{
    [SerializeField] private Transform aimTransform;
    [SerializeField] private CinemachineVirtualCamera virtualCamera { get; set; }
    [SerializeField] private Camera Mcam { get; set; }
    [SerializeField] private MultiAimConstraint torsoAimConstraint;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private bool showGizmos = true; // Exibe os Gizmos no modo de jogo
    [SerializeField] private float cameraOffsetSpeed = 5f; // Velocidade de transi��o do offset
    [SerializeField] private float cameraDistanceAiming = 23f; // Dist�ncia da c�mera ao mirar
    [SerializeField] private float minZoomDistance = 15f; // Dist�ncia m�nima do zoom
    [SerializeField] private float maxZoomDistance = 19.5f; // Dist�ncia m�xima do zoom
    [SerializeField] private float zoomSpeed = 2f; // Velocidade do zoom
    [SerializeField] private float cameraRadius = 5f; // Raio m�ximo para o movimento da c�mera ao mirar
    [SerializeField] private float rotationMultiplier = 2f; // Multiplicador de velocidade da rota��o
    [SerializeField] private float aimWeightChangeSpeed = 5f; // Velocidade de mudan�a de peso do Multi-Aim Constraint

    [SerializeField] private LayerMask layer;

    [SerializeField] private StarterAssetsInputs input;
    private CinemachineFramingTransposer framingTransposer;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        Mcam = GameManager.Instance.mainCamera;
        virtualCamera = GameManager.Instance.virtualCamera;
        framingTransposer = GameManager.Instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        var (success, position) = MouseController.GetMousePosition(Mcam, layer);
        if (success)
        {
            AimMousePosition(position);
        }
        RotateCameraY();
        AdjustCamera();
    }

    private void AimMousePosition(Vector3 position)
    {
        aimTransform.position = position;
    }

    public void RotateTowardsMouseSmooth(Vector3 aimPoint)
    {
        Vector3 aimDirection = (aimPoint - transform.position).normalized;
        aimDirection.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Ajuste do peso do Multi-Aim Constraint
        torsoAimConstraint.weight = Mathf.Lerp(torsoAimConstraint.weight, 1f, Time.deltaTime * aimWeightChangeSpeed);
    }

    private void AdjustCamera()
    {
        Vector3 targetOffset = Vector3.zero;
        float zoomInput = input.zoom.y / 100f;
        float currentZoomHeight = framingTransposer.m_CameraDistance;

        // Limitar o valor do zoom para dentro dos limites definidos
        currentZoomHeight = Mathf.Clamp(currentZoomHeight, minZoomDistance, maxZoomDistance);

        if (input.aim)
        {
            // Calcula o deslocamento desejado da c�mera ao mirar
            Vector3 offsetDirection = (aimTransform.position - transform.position).normalized;
            float distanceFromPlayer = Mathf.Clamp(Vector3.Distance(aimTransform.position, transform.position), 0, cameraRadius);
            targetOffset = new Vector3(offsetDirection.x * distanceFromPlayer, framingTransposer.m_TrackedObjectOffset.y, offsetDirection.z * distanceFromPlayer);

            // Ajusta a dist�ncia da c�mera ao valor de mira
            framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, cameraDistanceAiming, Time.deltaTime * cameraOffsetSpeed);
        }
        else
        {
            // Quando o jogador n�o est� mirando, ajusta o zoom com base no input
            if (Mathf.Abs(zoomInput) > 0.1f)
            {
                float zoomAmount = zoomInput * zoomSpeed;
                float targetZoom = Mathf.Clamp(currentZoomHeight - zoomAmount, minZoomDistance, maxZoomDistance);
                framingTransposer.m_CameraDistance = targetZoom;
            }

            // Retorna o offset da c�mera ao valor padr�o se n�o estiver mirando
            targetOffset = new Vector3(0, framingTransposer.m_TrackedObjectOffset.y, 0);
        }

        // Aplica o offset da c�mera suavemente
        framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(framingTransposer.m_TrackedObjectOffset, targetOffset, Time.deltaTime * cameraOffsetSpeed);
    }

    private void RotateCameraY()
    {
        float rotateInput = input.rotate.x;

        if (rotateInput != 0)
        {
            float rotationAmount = Mathf.Sign(rotateInput) * rotationMultiplier * Time.deltaTime;
            Vector3 currentRotation = virtualCamera.transform.rotation.eulerAngles;
            float newYRotation = currentRotation.y + rotationAmount;
            virtualCamera.transform.rotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos && Application.isPlaying)
        {
            // Desenha o c�rculo ao redor do jogador que representa o limite m�ximo da c�mera
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, cameraRadius);

            // Desenha uma linha entre o jogador e a posi��o de mira para visualizar a dire��o do offset da c�mera
            if (input.aim)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, aimTransform.position);
            }
        }
    }
}
