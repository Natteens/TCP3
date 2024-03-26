using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;

public class VFXManager : NetworkBehaviour
{
    // Método principal para reproduzir o VFX
    public void PlayVFX(VisualEffect vfx, float duration)
    {
        StartCoroutine(PlayVFXCoroutine(vfx, duration));
        VFXClientRPC(vfx.name, duration);
    }

    // RPC para reproduzir o VFX em todos os clientes
    [ClientRpc]
    private void VFXClientRPC(string vfxName, float duration)
    {
        // Encontra o objeto do efeito visual com base no nome e reproduz o efeito localmente
        VisualEffect vfx = GameObject.Find(vfxName)?.GetComponent<VisualEffect>();
        if (vfx != null)
        {
            StartCoroutine(PlayVFXCoroutine(vfx, duration));
        }
    }

    // Corrotina para reproduzir o efeito visual
    private IEnumerator PlayVFXCoroutine(VisualEffect vfx, float duration)
    {
        // Reproduz o efeito visual
        vfx.Play();

        // Aguarda o tempo especificado
        yield return new WaitForSeconds(duration);

        // Para o efeito visual após o tempo especificado
        vfx.Stop();
    }
}
