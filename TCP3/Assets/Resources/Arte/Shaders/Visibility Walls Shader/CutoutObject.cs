using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private LayerMask wallMask;

    [SerializeField]
    private Camera mainCamera;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(transform.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = transform.position - mainCamera.transform.position;

        RaycastHit[] hitObjects = Physics.RaycastAll(mainCamera.transform.position, offset, offset.magnitude, wallMask);

        Debug.DrawRay(mainCamera.transform.position, offset, Color.red);

        List<Renderer> hitRenderers = new List<Renderer>();

        for (int i = 0; i < hitObjects.Length; ++i)
        {
            Renderer renderer = hitObjects[i].transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                hitRenderers.Add(renderer);

                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.materials;
                }

                Material[] materials = renderer.materials;

                for (int m = 0; m < materials.Length; ++m)
                {
                    materials[m].SetVector("_CutoutPos", cutoutPos);
                    materials[m].SetFloat("_CutoutSize", 0.1f);
                    materials[m].SetFloat("_FalloffSize", 0.05f);
                }
            }
        }

        List<Renderer> renderersToRestore = new List<Renderer>(originalMaterials.Keys);
        renderersToRestore.RemoveAll(hitRenderers.Contains);

        foreach (Renderer renderer in renderersToRestore)
        {
            if (renderer != null && originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = originalMaterials[renderer];
                originalMaterials.Remove(renderer); 
            }
        }
    }
}
