using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SeekBehaviour", menuName = "AI/Steering Behaviours/Seek")]
public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float targetRechedThreshold = 0.5f;

    private bool reachedLastTarget = true;

    // Gizmo parameters
    private Vector3 targetPositionCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        // Se n�o tivermos um alvo, pare de procurar
        if (reachedLastTarget)
        {
            if (aiData.targets == null || aiData.targets.Count <= 0)
            {
                aiData.currentTarget = null;
                return (danger, interest);
            }
            else
            {
                reachedLastTarget = false;
                aiData.currentTarget = aiData.targets.OrderBy
                    (target => Vector3.Distance(target.position, aiData.transform.position)).FirstOrDefault();
            }
        }

        // Cache a �ltima posi��o apenas se ainda virmos o alvo (se a cole��o de alvos n�o estiver vazia)
        if (aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
        {
            targetPositionCached = aiData.currentTarget.position;
        }

        // Primeiro, verifique se alcan�amos o alvo
        if (Vector3.Distance(aiData.transform.position, targetPositionCached) < targetRechedThreshold)
        {
            reachedLastTarget = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }

        // Se ainda n�o alcan�amos o alvo, fa�a a l�gica principal de encontrar as dire��es de interesse
        Vector3 directionToTarget = (targetPositionCached - aiData.transform.position);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector3.Dot(directionToTarget.normalized, Directions.allDirections[i]);

            // Aceitar apenas dire��es a menos de 90 graus da dire��o do alvo
            if (result > 0)
            {
                float valueToPutIn = result;
                if (valueToPutIn > interest[i])
                {
                    interest[i] = valueToPutIn;
                }
            }
        }
        interestsTemp = interest;
        return (danger, interest);
    }

    public override void DrawGizmos(AIData aiData)
    {
        // Desenhar uma esfera no cache da posi��o alvo
        Gizmos.DrawSphere(targetPositionCached, 0.2f);

        if (interestsTemp != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < interestsTemp.Length; i++)
            {
                Gizmos.DrawRay(aiData.transform.position, Directions.allDirections[i] * interestsTemp[i] * 2);
            }

            if (!reachedLastTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPositionCached, 0.1f);
            }
        }
    }
}