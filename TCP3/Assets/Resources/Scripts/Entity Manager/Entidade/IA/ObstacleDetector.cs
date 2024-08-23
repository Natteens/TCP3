using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle Detector", menuName = "AI/Detector/Obstacle Detector")]
public class ObstacleDetector : Detector
{
    [SerializeField]
    private float detectionRadius = 2;

    [SerializeField]
    private LayerMask layerMask;

    private Collider[] colliders;

    public override void Detect(AIData aiData)
    {
        colliders = Physics.OverlapSphere(aiData.transform.position, detectionRadius, layerMask);
        aiData.obstacles = colliders;
    }

    public override void DrawGizmos(AIData aiData)
    {
        if (colliders != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider obstacleCollider in colliders)
            {
                Gizmos.DrawSphere(obstacleCollider.transform.position, 0.2f);
            }
        }
    }
}
