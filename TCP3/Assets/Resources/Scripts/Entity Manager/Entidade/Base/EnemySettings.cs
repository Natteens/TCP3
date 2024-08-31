using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnemySettings : NetworkBehaviour
{
    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("Nome do Inimigo")]
    [SerializeField, Required] private NetworkVariable<string> enemyName = new NetworkVariable<string>("Inimigo", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); 

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("N�vel do Inimigo")]
    [SerializeField, Required] private NetworkVariable<int> level = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("Xp drop Inimigo")]
    [SerializeField, Range(1, 10000)] private int giveXp; 

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("UI de Nome e N�vel")]
    [SerializeField, Required] private TextMeshProUGUI nameAndLevelText; // Refer�ncia ao texto da UI

    [BoxGroup("Multiplicadores de Status")]
    [LabelText("Multiplicador de Vida")]
    [SerializeField, MinValue(1)] private float healthMultiplier = 10f; // Multiplicador de vida por n�vel

    [BoxGroup("Multiplicadores de Status")]
    [LabelText("Multiplicador de Defesa")]
    [SerializeField, MinValue(1)] private float defenseMultiplier = 1.5f; // Multiplicador de defesa por n�vel

    [BoxGroup("UI Offset")]
    [LabelText("Deslocamento da UI")]
    [SerializeField] private Vector3 uiOffset; // Deslocamento da UI em rela��o ao inimigo

    [SerializeField] private StatusComponent statusComponent;
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private HealthBar healthBar;
    private Camera mainCamera;

    private void Start()
    {
        //statusComponent = GetComponent<StatusComponent>();
        mainCamera = Camera.main;

        ApplyLevelScaling();
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

    [Button("Aplicar N�vel e Atualizar UI", ButtonSizes.Large)]
    private void ApplyLevelScaling()
    {
        Dictionary<StatusType, float> currentStats = statusComponent.currentStatus;

        bool condition = GameManager.Instance.isNight.Value;
        float healthModifier = condition ? level.Value * (healthMultiplier * 1.5f) : level.Value * healthMultiplier;
        float defenseModifier = condition ? level.Value * (defenseMultiplier * 1.5f) : level.Value * defenseMultiplier;

        if (currentStats.ContainsKey(StatusType.Health))
        {
            currentStats[StatusType.Health] = currentStats[StatusType.Health] + healthModifier; 
        }

        if (currentStats.ContainsKey(StatusType.Defense))
        {
            currentStats[StatusType.Defense] = currentStats[StatusType.Defense] + defenseModifier;
        }

        statusComponent.UpdateStatus(currentStats);
        healthComponent.InitializeHealth();
        healthBar.UpdateHealthBar(0);
        UpdateNameAndLevelUI();
    }

    private void UpdateNameAndLevelUI()
    {
        if (nameAndLevelText != null)
        {
            nameAndLevelText.text = $"{enemyName} - Nv. {level}";
        }
    }
    public void Setup(byte minLevel, byte maxLevel)
    {
        SetLevel(Random.Range(minLevel, maxLevel + 1));
    }

    [Button("Definir N�vel")]
    public void SetLevel(int newLevel)
    {
        level.Value = newLevel;
        ApplyLevelScaling();
        UpdateNameAndLevelUI();
    }

    [Button("Definir Nome")]
    public void SetName(string newName)
    {
        enemyName.Value = newName;
        UpdateNameAndLevelUI();
    }

    public int GetLevel()
    { 
        return level.Value;
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
