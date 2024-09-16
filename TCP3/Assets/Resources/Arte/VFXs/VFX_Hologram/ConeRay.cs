using System.Collections;
using UnityEngine;

public class ConeRay : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer; // layer com objetos que obstruem a visão do inimigo, como paredes
    public int VisionConeResolution = 120; // resolução do cone de visão
    public bool IsVisionConeActive = false; // variável para ativar/desativar o cone de visão
    public Transform Target; // posição específica para onde o cone de visão apontará

    private Mesh VisionConeMesh;
    private MeshFilter MeshFilter_;

    void Start()
    {
        // Adiciona MeshRenderer e MeshFilter
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = VisionConeMaterial;
        MeshFilter_ = gameObject.AddComponent<MeshFilter>();

        VisionConeMesh = new Mesh();
        VisionAngle *= Mathf.Deg2Rad; // Converte o ângulo de visão para radianos
    }

    void Update()
    {
        // Verifica se o cone de visão está ativo
        if (IsVisionConeActive)
        {
            // Se houver um alvo definido, rotaciona o cone para a direção do alvo
            if (Target != null)
            {
                Vector3 directionToTarget = (Target.position - transform.position).normalized;
                directionToTarget.y = 0; // Garante que a rotação ocorra apenas no plano XZ (no eixo Y)
                transform.forward = directionToTarget; // Rotaciona o cone na direção do alvo
            }

            DrawVisionCone(); // Atualiza o cone de visão a cada frame
        }
        else
        {
            // Se o cone não estiver ativo, limpa o mesh
            VisionConeMesh.Clear();
        }
    }

    void DrawVisionCone()
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] vertices = new Vector3[VisionConeResolution + 1];
        vertices[0] = Vector3.zero;

        float currentAngle = -VisionAngle / 2;
        float angleIncrement = VisionAngle / (VisionConeResolution - 1);

        for (int i = 0; i < VisionConeResolution; i++)
        {
            float sine = Mathf.Sin(currentAngle);
            float cosine = Mathf.Cos(currentAngle);

            Vector3 raycastDirection = (transform.forward * cosine) + (transform.right * sine);
            Vector3 vertexForward = (Vector3.forward * cosine) + (Vector3.right * sine);

            // Realiza o raycast para detectar obstruções
            if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                vertices[i + 1] = vertexForward * hit.distance;
            }
            else
            {
                vertices[i + 1] = vertexForward * VisionRange;
            }

            currentAngle += angleIncrement;
        }

        // Define os triângulos para a malha
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        // Atualiza o mesh do cone de visão
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }
}
