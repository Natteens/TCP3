using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DayNightCycle : NetworkBehaviour
{
    public Light directionalLight;
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;
    public short dayDuration = 120; // Ajustado para byte
    public float transitionDuration = 5f; // Tempo de transição suave entre dia e noite

    private short timeOfDay;
    private float transitionProgress = 1f; // Inicia com 1 para aplicar transição imediatamente
    private Color targetColor;
    private float targetIntensity;
    private Color startColor;
    private float startIntensity;

    private void Start()
    {
         timeOfDay = 0;
         InvokeRepeating(nameof(UpdateDayNightCycle), 0f, 1f);
    }

    private void UpdateDayNightCycle()
    {
        if (dayDuration <= 0)
        {
            Debug.LogError("dayDuration deve ser maior que zero.");
            return;
        }

        // Atualiza o tempo do dia
        timeOfDay = (short)((timeOfDay + 1) % (dayDuration + 1));

        float normalizedTimeOfDay = timeOfDay / (float)dayDuration;

        // Calcula as cores e intensidades alvo com base no tempo do dia
        Color newColor = lightColorGradient.Evaluate(normalizedTimeOfDay);
        float newIntensity = lightIntensityCurve.Evaluate(normalizedTimeOfDay);

        // Atualiza a luz com a nova cor e intensidade
        if (directionalLight != null)
        {
            // Se a transição estiver em andamento, apenas atualiza os valores alvo
            if (transitionProgress >= 1f)
            {
                targetColor = newColor;
                targetIntensity = newIntensity;
                StartCoroutine(TransitionLighting(newColor, newIntensity));
            }
            else
            {
                // Atualiza as variáveis alvo para a próxima transição
                targetColor = newColor;
                targetIntensity = newIntensity;
            }
        }
    }

    private IEnumerator TransitionLighting(Color newColor, float newIntensity)
    {
        // Se a transição já estiver em andamento, não inicia uma nova
        if (transitionProgress < 1f)
        {
            yield break;
        }

        // Configura os valores iniciais para a transição
        startColor = directionalLight.color;
        startIntensity = directionalLight.intensity;

        transitionProgress = 0f;

        // Transição suave
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

        // Finaliza a transição
        transitionProgress = 1f;
    }
}
