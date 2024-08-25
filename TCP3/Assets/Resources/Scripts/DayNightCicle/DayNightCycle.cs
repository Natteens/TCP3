using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DayNightCycle : NetworkBehaviour
{
    public Light directionalLight;
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;
    public float dayDuration = 120f;
    public float transitionDuration = 5f; // Tempo de transi��o suave entre dia e noite

    [SerializeField] private float timeOfDay;
    private float transitionProgress = 0f;
    private Color targetColor;
    private float targetIntensity;
    private Color startColor;
    private float startIntensity;

    private void Start()
    {
        if (IsServer)
        {
            timeOfDay = 0f;
            InvokeRepeating(nameof(UpdateDayNightCycle), 0f, 1f);
        }
    }

    private void UpdateDayNightCycle()
    {
        if (dayDuration <= 0f)
        {
            Debug.LogError("dayDuration deve ser maior que zero.");
            return;
        }

        if (timeOfDay >= dayDuration * 0.7) GameManager.Instance.isNight = true;

        timeOfDay += Time.deltaTime;
        if (timeOfDay > dayDuration)
        {
            timeOfDay = 0f; // Reinicia o ciclo
            if (timeOfDay >= dayDuration * 0.7) GameManager.Instance.isNight = false;
        }

        float normalizedTimeOfDay = timeOfDay / dayDuration;

        if (normalizedTimeOfDay < 0f || normalizedTimeOfDay > 1f)
        {
            Debug.LogError("normalizedTimeOfDay est� fora do intervalo esperado [0, 1].");
            return;
        }

        // Calcula as cores e intensidades alvo com base no tempo do dia
        Color newColor = lightColorGradient.Evaluate(normalizedTimeOfDay);
        float newIntensity = lightIntensityCurve.Evaluate(normalizedTimeOfDay);

        // Atualiza a luz com a nova cor e intensidade
        if (directionalLight != null)
        {
            if (transitionProgress >= 1f)
            {
                // Se j� terminou a transi��o, s� atualiza as vari�veis alvo
                targetColor = newColor;
                targetIntensity = newIntensity;
            }
            else
            {
                // Inicia a transi��o
                StartCoroutine(TransitionLighting(newColor, newIntensity));
            }
        }
    }

    [ClientRpc]
    private void UpdateLightingClientRpc(Color newColor, float newIntensity)
    {
        if (directionalLight != null)
        {
            // Atualiza as vari�veis alvo e inicia a transi��o
            targetColor = newColor;
            targetIntensity = newIntensity;
            StartCoroutine(TransitionLighting(newColor, newIntensity));
        }
    }

    private IEnumerator TransitionLighting(Color newColor, float newIntensity)
    {
        // Se a transi��o j� estiver em andamento, interrompe
        if (transitionProgress < 1f)
        {
            yield return null;
        }

        // Configura os valores iniciais para a transi��o
        startColor = directionalLight.color;
        startIntensity = directionalLight.intensity;

        transitionProgress = 0f;

        // Transi��o suave
        while (transitionProgress < 1f)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            directionalLight.color = Color.Lerp(startColor, targetColor, transitionProgress);
            directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, transitionProgress);
            yield return null;
        }

        // Garante que o valor final seja exatamente o alvo
        directionalLight.color = targetColor;
        directionalLight.intensity = targetIntensity;
    }
}
