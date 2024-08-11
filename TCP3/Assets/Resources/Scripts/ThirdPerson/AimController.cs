using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private float maxAimDistance = 999f;
    [SerializeField] private Color rayColor = Color.red;

    public Vector3 GetAimPoint()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        // Desenha o raio de debug
        Debug.DrawRay(ray.origin, ray.direction * maxAimDistance, rayColor);

        if (Physics.Raycast(ray, out RaycastHit raycasthit, maxAimDistance, aimColliderLayerMask))
        {
            if (debugTransform != null) { debugTransform.position = raycasthit.point; }
            return raycasthit.point;
        }
        else
        {
            // Retornar um ponto longe na direção do ray caso não haja colisão
            return ray.GetPoint(maxAimDistance);
        }
    }

    public Vector3 GetLookDirection(Vector3 aimPoint, Transform characterTransform)
    {
        Vector3 worldAimTarget = aimPoint;
        worldAimTarget.y = characterTransform.position.y;
        return (worldAimTarget - characterTransform.position).normalized;
    }

    public void RotateTowards(Vector3 aimDirection, Transform characterTransform, float speed)
    {
        characterTransform.forward = Vector3.Lerp(characterTransform.forward, aimDirection, Time.deltaTime * speed);
    }
}
