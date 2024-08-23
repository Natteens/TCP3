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
            Vector3 directionToObstacle
                = obstacleCollider.ClosestPoint(aiData.transform.position) - aiData.transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            //calculate weight based on the distance Enemy<--->Obstacle
            float weight
                = distanceToObstacle <= agentColliderSize
                ? 1
                : (radius - distanceToObstacle) / radius;

            Vector3 directionToObstacleNormalized = directionToObstacle.normalized;

            //Add obstacle parameters to the danger array
            for (int i = 0; i < Directions.allDirections.Count; i++)
            {
                float result = Vector3.Dot(directionToObstacleNormalized, Directions.allDirections[i]);

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
                        Directions.allDirections[i] * dangersResultTemp[i]*2
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
    // Direções ajustadas para vetores 3D, incluindo inclinações para cima
    public static List<Vector3> allDirections = new List<Vector3>
    {
        new Vector3(0, 0, 1).normalized,      // Para frente
        new Vector3(0, 0, -1).normalized,     // Para trás
        new Vector3(1, 0, 0).normalized,      // Direita
        new Vector3(-1, 0, 0).normalized,     // Esquerda
        new Vector3(-1, 0, 1).normalized,     // Frente-Esquerda
        new Vector3(-1, 0, -1).normalized,    // Trás-Esquerda
        new Vector3(1, 0, -1).normalized,     // Trás-Direita
        new Vector3(1, 0, 1).normalized,      // Frente-Direita
    };
}