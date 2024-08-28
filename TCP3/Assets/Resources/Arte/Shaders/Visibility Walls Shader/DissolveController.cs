using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class DissolveController : MonoBehaviour
{
    public Transform player;
    public LayerMask constructionLayer;
    public float dissolveSpeed = 0.5f;
    public Vector3 dissolveBoxSize = new Vector3(2f, 2f, 2f);
    public Vector3 dissolveBoxOffset = Vector3.zero;
    public Material dissolveMaterial;
    public bool showDissolveBox = true;

    private Camera cam;
    private List<Renderer> affectedRenderers = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private float currentDissolveAmount = 0f;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (player == null) return;

        UpdateAffectedRenderers();
        UpdateDissolveEffect();
    }

    private void UpdateAffectedRenderers()
    {
        affectedRenderers.Clear();
        Vector3 boxCenter = player.position + dissolveBoxOffset;
        Collider[] colliders = Physics.OverlapBox(boxCenter, dissolveBoxSize / 2, player.rotation, constructionLayer);

        foreach (Collider collider in colliders)
        {
            Renderer renderer = collider.GetComponent<Renderer>();
            if (renderer != null && !affectedRenderers.Contains(renderer))
            {
                affectedRenderers.Add(renderer);
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.sharedMaterials;
                }
            }
        }
    }

    private void UpdateDissolveEffect()
    {
        bool shouldDissolve = affectedRenderers.Count > 0;
        float targetDissolveAmount = shouldDissolve ? 1f : 0f;
        currentDissolveAmount = Mathf.MoveTowards(currentDissolveAmount, targetDissolveAmount, dissolveSpeed * Time.deltaTime);

        foreach (Renderer renderer in affectedRenderers)
        {
            ApplyDissolveMaterial(renderer);
        }

        // Restore original materials for renderers no longer affected
        foreach (var kvp in originalMaterials.ToArray())
        {
            if (!affectedRenderers.Contains(kvp.Key))
            {
                if (kvp.Key != null)
                {
                    kvp.Key.sharedMaterials = kvp.Value;
                }
                originalMaterials.Remove(kvp.Key);
            }
        }
    }

    private void ApplyDissolveMaterial(Renderer renderer)
    {
        Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = new Material(dissolveMaterial);
            newMaterials[i].CopyPropertiesFromMaterial(renderer.sharedMaterials[i]);
            newMaterials[i].SetFloat("_DissolveAmount", currentDissolveAmount);
            newMaterials[i].SetVector("_DissolveOrigin", player.position + dissolveBoxOffset);
            newMaterials[i].SetVector("_DissolveSize", dissolveBoxSize);
        }
        renderer.sharedMaterials = newMaterials;
    }

    private void OnDisable()
    {
        // Restore all original materials when the script is disabled
        foreach (var kvp in originalMaterials)
        {
            if (kvp.Key != null)
            {
                kvp.Key.sharedMaterials = kvp.Value;
            }
        }
        originalMaterials.Clear();
    }

    private void OnDrawGizmos()
    {
        if (showDissolveBox && player != null)
        {
            Gizmos.color = Color.yellow;

            // Criar uma matriz de transformação para o cubo de dissolução
            Matrix4x4 dissolveMatrix = Matrix4x4.TRS(player.position + dissolveBoxOffset, player.rotation, Vector3.one);
            Gizmos.matrix = dissolveMatrix;

            // Desenhar o cubo de dissolução
            Gizmos.DrawWireCube(Vector3.zero, dissolveBoxSize);
        }
    }
}
