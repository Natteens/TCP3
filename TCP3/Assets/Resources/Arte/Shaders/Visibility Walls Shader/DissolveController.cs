using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class DissolveController : MonoBehaviour
{
    public Transform player; // Referência ao transform do jogador
    public LayerMask constructionLayer;
    public float raycastDistance = 10f;
    public float dissolveSpeed = 2f;
    public Color raycastColor = Color.red;
    public float dissolveWidth = 0.1f;
    public Color dissolveColor = Color.red;
    public float noiseScale = 50f;
    public float glowIntensity = 1f;

    private Camera cam;
    private Material dissolveMaterial;
    private List<Renderer> affectedRenderers = new List<Renderer>();
    private float currentDissolveHeight;

    private void Start()
    {
        cam = GetComponent<Camera>();
        dissolveMaterial = new Material(Shader.Find("Custom/ProceduralDissolve"));
        dissolveMaterial.SetFloat("_DissolveWidth", dissolveWidth);
        dissolveMaterial.SetColor("_DissolveColor", dissolveColor);
        dissolveMaterial.SetFloat("_NoiseScale", noiseScale);
        dissolveMaterial.SetFloat("_GlowIntensity", glowIntensity);

        if (player == null)
        {
            Debug.LogError("Player reference not set in DissolveController!");
        }
    }

    private void Update()
    {
        if (player == null) return;

        bool isInConstruction = CheckIfInConstruction();
        UpdateDissolveEffect(isInConstruction);
    }

    private bool CheckIfInConstruction()
    {
        Vector3 direction = cam.transform.position - player.position;
        Ray ray = new Ray(player.position, direction);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance, constructionLayer);
        Debug.DrawRay(ray.origin, ray.direction * (hit ? hitInfo.distance : raycastDistance), raycastColor);
        return hit;
    }

    private void UpdateDissolveEffect(bool isInConstruction)
    {
        float targetHeight = isInConstruction ? player.position.y + 2f : 1000f; // Ajuste conforme necessário
        currentDissolveHeight = Mathf.MoveTowards(currentDissolveHeight, targetHeight, dissolveSpeed * Time.deltaTime);
        dissolveMaterial.SetFloat("_DissolveHeight", currentDissolveHeight);

        // Encontre todos os renderizadores afetados
        Collider[] colliders = Physics.OverlapSphere(player.position, raycastDistance, constructionLayer);
        affectedRenderers.Clear();
        foreach (Collider col in colliders)
        {
            Renderer renderer = col.GetComponent<Renderer>();
            if (renderer != null && !affectedRenderers.Contains(renderer))
            {
                affectedRenderers.Add(renderer);
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (affectedRenderers.Count == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(source, temp);

        foreach (Renderer renderer in affectedRenderers)
        {
            Bounds bounds = renderer.bounds;
            Vector3 min = cam.WorldToViewportPoint(bounds.min);
            Vector3 max = cam.WorldToViewportPoint(bounds.max);

            dissolveMaterial.SetVector("_ObjectBounds", new Vector4(min.x, min.y, max.x, max.y));
            Graphics.Blit(temp, destination, dissolveMaterial);
            Graphics.Blit(destination, temp);
        }

        Graphics.Blit(temp, destination);
        RenderTexture.ReleaseTemporary(temp);
    }
}