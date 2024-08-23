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
        // Use Physics.OverlapSphere instead of Physics2D.OverlapCircle for 3D detection
        Collider[] playerColliders = Physics.OverlapSphere(aiData.transform.position, targetDetectionRange, playerLayerMask);

        if (playerColliders.Length > 0)
        {
            Transform playerTransform = playerColliders[0].transform;
            Vector3 direction = (playerTransform.position - aiData.transform.position).normalized;

            // Use Physics.Raycast for 3D raycasting
            if (Physics.Raycast(aiData.transform.position, direction, out RaycastHit hit, targetDetectionRange, obstaclesLayerMask))
            {
                if ((playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    Debug.DrawRay(aiData.transform.position, direction * targetDetectionRange, Color.magenta);
                    colliders = new List<Transform>() { playerTransform };
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
