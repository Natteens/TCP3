using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContextSolver", menuName = "AI/Context Solver")]
public class ContextSolver : ScriptableObject
{
    // Gizmo parameters
    float[] interestGizmo = new float[0];
    Vector3 resultDirection = Vector3.zero;
    private float rayLength = 2;

    public void Init()
    {
        interestGizmo = new float[8];
    }

    public Vector3 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        // Loop through each behaviour
        foreach (SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
        }

        // Subtract danger values from interest array
        for (int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        interestGizmo = interest;

        // Get the average direction
        Vector3 outputDirection = Vector3.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += Directions.allDirections[i] * interest[i];
        }

        outputDirection.Normalize();

        resultDirection = outputDirection;

        // Return the selected movement direction
        return resultDirection;
    }

    public void DrawGizmos(AIData aiData)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(aiData.transform.position, resultDirection * rayLength);
    }
}
