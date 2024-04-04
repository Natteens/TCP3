using System.Collections;
using UnityEngine;
using Unity.Netcode;


public class VFXManager : Singleton<VFXManager>
{
    // Método principal para reproduzir o VFX
    public void PlayVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation, float duration)
    {
        InstantiateAndDestroyVFX(vfxPrefab, position, rotation, duration);
        VFXClientRPC(vfxPrefab.name, position, rotation, duration);
    }

    // RPC para reproduzir o VFX em todos os clientes
    [ClientRpc]
    private void VFXClientRPC(string vfxName, Vector3 position, Quaternion rotation, float duration)
    {
        // Encontra o objeto do efeito visual com base no nome e reproduz o efeito localmente
        GameObject vfx = GameObject.Find(vfxName)?.GetComponent<GameObject>();
        if (vfx != null)
        {
            InstantiateAndDestroyVFX(vfx, position, rotation, duration);
        }
    }

    public void InstantiateAndDestroyVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation, float duration)
    {
        // Instancia o objeto VFX na posição e rotação especificadas
        GameObject instantiatedVFX = Instantiate(vfxPrefab, position, rotation);

        // Inicia a corrotina para destruir o objeto VFX após a duração especificada
        StartCoroutine(DestroyVFXAfterDuration(instantiatedVFX, duration));
    }

    private IEnumerator DestroyVFXAfterDuration(GameObject vfxObject, float duration)
    {
        // Aguarda a duração especificada antes de destruir o objeto VFX
        yield return new WaitForSeconds(duration);
        Destroy(vfxObject);
    }

}
