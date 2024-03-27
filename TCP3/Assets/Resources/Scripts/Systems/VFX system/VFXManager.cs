using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;

public class VFXManager : NetworkBehaviour
{
    public void PlayVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation, float duration)
    {
        GameObject instantiatedVFX = Instantiate(vfxPrefab, position, rotation);
        instantiatedVFX.GetComponent<NetworkObject>().Spawn();
        StartCoroutine(DestroyVFXAfterDuration(instantiatedVFX, duration));
    }

    private IEnumerator DestroyVFXAfterDuration(GameObject vfxObject, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(vfxObject);
    }
}
