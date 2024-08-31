using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Vector3 offset; 

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        healthComponent.OnTakeDamage += UpdateHealthBar;
        healthComponent.OnHeal += UpdateHealthBar;
        //healthComponent.OnRevive += UpdateHealthBar;
        UpdateHealthBar(0);
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        transform.position = healthComponent.transform.position + offset;
    }

    public void UpdateHealthBar(float amount)
    {
        float normalizedHealth = Mathf.Clamp01(healthComponent.CurrentHealth / healthComponent.MaxHealth);

        // Atualiza a barra de vida usando o valor normalizado
        healthBarImage.fillAmount = normalizedHealth;
    }

    private void OnDestroy()
    {
        // Remove os eventos ao destruir o objeto para evitar erros
        healthComponent.OnTakeDamage -= UpdateHealthBar;
        healthComponent.OnHeal -= UpdateHealthBar;
        //healthComponent.OnRevive -= UpdateHealthBar;
    }
}
