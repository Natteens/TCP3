using System.Collections;
using UnityEngine;

public class ConeRay : MonoBehaviour
{
    public Material VisionConeMaterial;
    public Mesh CustomMesh; // A mesh personalizada para o cone de vis�o
    public float VisionRange;
    public LayerMask VisionObstructingLayer; // Layer com objetos que obstruem a vis�o do inimigo
    public bool IsVisionConeActive = false; // Vari�vel para ativar/desativar o cone de vis�o
    public Transform Target; // Posi��o espec�fica para onde o cone de vis�o apontar�

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
        // Verifica se o cone de vis�o est� ativo
        if (IsVisionConeActive)
        {
            // Se houver um alvo definido, rotaciona o cone para a dire��o do alvo
            if (Target != null)
            {
                Vector3 directionToTarget = (Target.position - transform.position).normalized;
                directionToTarget.y = 0; // Garante que a rota��o ocorra apenas no plano XZ (no eixo Y)
                transform.forward = directionToTarget; // Rotaciona o cone na dire��o do alvo
            }

            // Atualiza a mesh de vis�o caso seja necess�rio
            UpdateVisionMesh();
        }
        else
        {
            // Se o cone n�o estiver ativo, limpa o mesh
            MeshFilter_.mesh.Clear();
        }
    }

    void UpdateVisionMesh()
    {
        if (CustomMesh == null)
        {
            Debug.LogWarning("Nenhuma mesh customizada foi atribu�da.");
            return;
        }

        // Raycast para detectar obstru��es na vis�o
        Vector3[] vertices = CustomMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertexWorldPos = transform.TransformPoint(vertices[i]);
            Vector3 rayDirection = (vertexWorldPos - transform.position).normalized;

            // Se houver obstru��o na vis�o
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                vertices[i] = transform.InverseTransformPoint(hit.point); // Ajusta o v�rtice da mesh
            }
            else
            {
                // Se n�o houver obstru��o, mant�m o v�rtice na posi��o original com base no VisionRange
                vertices[i] = vertices[i].normalized * VisionRange;
            }
        }

        // Atualiza a mesh de vis�o com os novos v�rtices
        CustomMesh.vertices = vertices;
        CustomMesh.RecalculateBounds();
        MeshFilter_.mesh = CustomMesh;
    }
}
