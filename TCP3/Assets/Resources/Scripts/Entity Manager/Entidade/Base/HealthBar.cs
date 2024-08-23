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

    private void UpdateHealthBar(float amount)
    {
        // Atualiza a barra de vida com base na vida atual do HealthComponent
        healthBarImage.fillAmount = healthComponent.CurrentHealth / healthComponent.MaxHealth;
    }

    private void OnDestroy()
    {
        // Remove os eventos ao destruir o objeto para evitar erros
        healthComponent.OnTakeDamage -= UpdateHealthBar;
        healthComponent.OnHeal -= UpdateHealthBar;
        //healthComponent.OnRevive -= UpdateHealthBar;
    }
}
