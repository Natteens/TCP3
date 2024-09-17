using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateParticleTransform : MonoBehaviour
{
    public float height = 2f;         // Altura que a part�cula vai subir
    public float duration = 1f;       // Tempo que levar� para atingir a altura
    private Vector3 startPosition;    // Posi��o inicial da part�cula
    private Vector3 targetPosition;   // Posi��o final da part�cula
    private float elapsedTime = 0f;   // Tempo decorrido

    void Start()
    {
        // Define a posi��o inicial da part�cula
        startPosition = transform.position;

        // Define a posi��o final, subindo ao longo do eixo Y
        targetPosition = new Vector3(startPosition.x, startPosition.y + height, startPosition.z);
    }

    void Update()
    {
        // Atualiza o tempo decorrido
        elapsedTime += Time.deltaTime;

        // Move a part�cula da posi��o inicial para a posi��o final ao longo do tempo
        if (elapsedTime < duration)
        {
            // Interpola��o linear (Lerp) para fazer a part�cula subir suavemente
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
        }
    }
}
