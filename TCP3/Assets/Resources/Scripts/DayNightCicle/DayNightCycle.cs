using UnityEngine;
using Unity.Netcode;

public class DayNightCycle : NetworkBehaviour
{
    private void Start()
    {
       GameManager.Instance.timeOfDay.OnValueChanged += UpdateDayNightCycleServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateDayNightCycleServerRpc(short prev, short timeOfDay)
    {
        if (GameManager.Instance.dayDuration <= 0)
        {
            Debug.LogError("dayDuration deve ser maior que zero.");
            return;
        }

        UpdateLighting(timeOfDay);

        // Propaga a atualização de iluminação para todos os clientes
        UpdateLightingClientRpc(timeOfDay);
    }

    private void UpdateLighting(short newTime)
    {
        float normalizedTimeOfDay = newTime / (float)GameManager.Instance.dayDuration;

        Color newColor = GameManager.Instance.lightColorGradient.Evaluate(normalizedTimeOfDay);
        GameManager.Instance.directionalLight.color = newColor;
    }

    [ClientRpc]
    private void UpdateLightingClientRpc(short newTime)
    {
        UpdateLighting(newTime);
    }
}
