using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleAvoidanceBehaviour", menuName = "AI/Steering Behaviours/Obstacle Avoidance")]
public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float radius = 2f, agentColliderSize = 0.6f;

    //gizmo parameters
    float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider obstacleCollider in aiData.obstacles)
        {
            Vector2 directionToObstacle
                = obstacleCollider.ClosestPoint(aiData.transform.position) - aiData.transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            //calculate weight based on the distance Enemy<--->Obstacle
            float weight
                = distanceToObstacle <= agentColliderSize
                ? 1
                : (radius - distanceToObstacle) / radius;

            Vector2 directionToObstacleNormalized = directionToObstacle.normalized;

            //Add obstacle parameters to the danger array
            for (int i = 0; i < Directions.eightDirections.Count; i++)
            {
                float result = Vector2.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                float valueToPutIn = result * weight;

                //override value only if it is higher than the current one stored in the danger array
                if (valueToPutIn > danger[i])
                {
                    danger[i] = valueToPutIn;
                }
            }
        }
        dangersResultTemp = danger;
        return (danger, interest);
    }

    public override void DrawGizmos(AIData aiData)
    {
        if (dangersResultTemp != null)
        {
            if (dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(
                        aiData.transform.position,
                        Directions.eightDirections[i] * dangersResultTemp[i]*2
                        );
                }
            }
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(aiData.transform.position, agentColliderSize);
    }
}

public static class Directions
{
    public static List<Vector3> eightDirections = new List<Vector3>{
            new Vector3(0,0,1).normalized,
            new Vector3(0,0,-1).normalized,
            new Vector3(1,0,0).normalized,
            new Vector3(-1,0,0).normalized,
            new Vector3(-1,0,1).normalized,
            new Vector3(-1,0,-1).normalized,
            new Vector3(1,0,-1).normalized,
            new Vector3(1,0,1).normalized,
        };
}
