using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Vector3 offset; // Deslocamento em relação ao inimigo

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Assina os eventos de dano e cura do HealthComponent
        healthComponent.OnTakeDamage += UpdateHealthBar;
        healthComponent.OnHeal += UpdateHealthBar;
        //healthComponent.OnRevive += UpdateHealthBar;

        // Inicializa a barra de vida com o valor inicial de saúde
        UpdateHealthBar(0);
    }

    private void LateUpdate()
    {
        // Faz a barra de vida olhar para a câmera
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);

        // Posiciona a barra de vida acima do inimigo
        transform.position = healthComponent.transform.position + offset;
    }

    public void UpdateHealthBar(float amount)
    {
        // Calcula o valor normalizado da vida atual entre 0 e 1
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
