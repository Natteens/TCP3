using System.Collections;
using UnityEngine;

public class ConeRay : MonoBehaviour
{
    public Material VisionConeMaterial;
    public Mesh CustomMesh; // A mesh personalizada para o cone de visão
    public float VisionRange;
    public LayerMask VisionObstructingLayer; // Layer com objetos que obstruem a visão do inimigo
    public bool IsVisionConeActive = false; // Variável para ativar/desativar o cone de visão
    public Transform Target; // Posição específica para onde o cone de visão apontará

    private MeshFilter MeshFilter_;

    void Start()
    {
        // Adiciona MeshRenderer e MeshFilter
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = VisionConeMaterial;
        MeshFilter_ = gameObject.AddComponent<MeshFilter>();

        // Define a mesh personalizada
        if (CustomMesh != null)
        {
            MeshFilter_.mesh = CustomMesh;
        }
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

            // Atualiza a mesh de visão caso seja necessário
            UpdateVisionMesh();
        }
        else
        {
            // Se o cone não estiver ativo, limpa o mesh
            MeshFilter_.mesh.Clear();
        }
    }

    void UpdateVisionMesh()
    {
        if (CustomMesh == null)
        {
            Debug.LogWarning("Nenhuma mesh customizada foi atribuída.");
            return;
        }

        // Raycast para detectar obstruções na visão
        Vector3[] vertices = CustomMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertexWorldPos = transform.TransformPoint(vertices[i]);
            Vector3 rayDirection = (vertexWorldPos - transform.position).normalized;

            // Se houver obstrução na visão
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                vertices[i] = transform.InverseTransformPoint(hit.point); // Ajusta o vértice da mesh
            }
            else
            {
                // Se não houver obstrução, mantém o vértice na posição original com base no VisionRange
                vertices[i] = vertices[i].normalized * VisionRange;
            }
        }

        // Atualiza a mesh de visão com os novos vértices
        CustomMesh.vertices = vertices;
        CustomMesh.RecalculateBounds();
        MeshFilter_.mesh = CustomMesh;
    }
}
