using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target Detector", menuName = "AI/Detector/Target Detector")]
public class TargetDetector : Detector
{
    [SerializeField]
    private LayerMask obstaclesLayerMask, playerLayerMask;

    private List<Transform> colliders;

    public override void Detect(AIData aiData)
    {
        Collider2D playerCollider = 
            Physics2D.OverlapCircle(aiData.transform.position, targetDetectionRange, playerLayerMask);

        if (playerCollider != null)
        {
            Vector2 direction = (playerCollider.transform.position - aiData.transform.position).normalized;
            RaycastHit2D hit = 
                Physics2D.Raycast(aiData.transform.position, direction, targetDetectionRange, obstaclesLayerMask);

            if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
            {
                Debug.DrawRay(aiData.transform.position, direction * targetDetectionRange, Color.magenta);
                colliders = new List<Transform>() { playerCollider.transform };
            }
            else
            {
                colliders = null;
            }
        }
        else
        {
            colliders = null;
        }
        aiData.targets = colliders;
    }

    public override void DrawGizmos(AIData aiData)
    {
        Gizmos.DrawWireSphere(aiData.transform.position, targetDetectionRange);

        if (colliders == null)
            return;
        Gizmos.color = Color.magenta;
        foreach (var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }
}
