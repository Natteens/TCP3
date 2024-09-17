using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateParticleTransform : MonoBehaviour
{
    public float height = 2f;         // Altura que a partícula vai subir
    public float duration = 1f;       // Tempo que levará para atingir a altura
    private Vector3 startPosition;    // Posição inicial da partícula
    private Vector3 targetPosition;   // Posição final da partícula
    private float elapsedTime = 0f;   // Tempo decorrido

    void Start()
    {
        // Define a posição inicial da partícula
        startPosition = transform.position;

        // Define a posição final, subindo ao longo do eixo Y
        targetPosition = new Vector3(startPosition.x, startPosition.y + height, startPosition.z);
    }

    void Update()
    {
        // Atualiza o tempo decorrido
        elapsedTime += Time.deltaTime;

        // Move a partícula da posição inicial para a posição final ao longo do tempo
        if (elapsedTime < duration)
        {
            // Interpolação linear (Lerp) para fazer a partícula subir suavemente
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
        }
    }
}
