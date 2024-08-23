using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySettings : MonoBehaviour
{
    [BoxGroup("Configurações do Inimigo")]
    [LabelText("Nome do Inimigo")]
    [SerializeField, Required] private string enemyName = "Inimigo"; // Nome do inimigo

    [BoxGroup("Configurações do Inimigo")]
    [LabelText("Nível do Inimigo")]
    [SerializeField, Range(1, 100)] private int level = 1; // Nível do inimigo

    [BoxGroup("Configurações do Inimigo")]
    [LabelText("UI de Nome e Nível")]
    [SerializeField, Required] private TextMeshProUGUI nameAndLevelText; // Referência ao texto da UI

    [BoxGroup("Multiplicadores de Status")]
    [LabelText("Multiplicador de Vida")]
    [SerializeField, MinValue(1)] private float healthMultiplier = 10f; // Multiplicador de vida por nível

    [BoxGroup("Multiplicadores de Status")]
    [LabelText("Multiplicador de Defesa")]
    [SerializeField, MinValue(1)] private float defenseMultiplier = 1.5f; // Multiplicador de defesa por nível

    [BoxGroup("UI Offset")]
    [LabelText("Deslocamento da UI")]
    [SerializeField] private Vector3 uiOffset; // Deslocamento da UI em relação ao inimigo

    [SerializeField] private StatusComponent statusComponent;
    private Camera mainCamera;

    private void Start()
    {
        //statusComponent = GetComponent<StatusComponent>();
        mainCamera = Camera.main;

      //  ApplyLevelScaling();
        UpdateNameAndLevelUI();
    }

    private void LateUpdate()
    {
        if (nameAndLevelText != null)
        {
            nameAndLevelText.transform.rotation = Quaternion.LookRotation(nameAndLevelText.transform.position - mainCamera.transform.position);
            nameAndLevelText.transform.position = statusComponent.transform.position + uiOffset;
        }
    }

    [Button("Aplicar Nível e Atualizar UI", ButtonSizes.Large)]
    private void ApplyLevelScaling()
    {
        // Modifica os status com base no nível e nos multiplicadores configurados
        Dictionary<StatusType, float> newStatus = new Dictionary<StatusType, float>();

        // Exemplo de aplicação de modificadores: Vida, Ataque, Defesa
        float health = level * healthMultiplier;
        float defense = level * defenseMultiplier;

        newStatus[StatusType.Health] = health;
        newStatus[StatusType.Defense] = defense;

        // Atualiza os status usando o método público do StatusComponent
        statusComponent.UpdateStatus(newStatus);
    }

    private void UpdateNameAndLevelUI()
    {
        if (nameAndLevelText != null)
        {
            nameAndLevelText.text = $"{enemyName} - Nv. {level}";
        }
    }

    [Button("Definir Nível")]
    public void SetLevel(int newLevel)
    {
        level = newLevel;
        ApplyLevelScaling();
        UpdateNameAndLevelUI();
    }

    [Button("Definir Nome")]
    public void SetName(string newName)
    {
        enemyName = newName;
        UpdateNameAndLevelUI();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyLevelScaling();
            UpdateNameAndLevelUI();
        }
    }
}
