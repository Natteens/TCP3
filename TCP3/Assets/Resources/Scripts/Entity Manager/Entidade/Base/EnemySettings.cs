using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[GenerateSerializationForType(typeof(string))]
public class EnemySettings : NetworkBehaviour
{
    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("Nome do Inimigo")]
    private NetworkVariable<string> networkEnemyName = new NetworkVariable<string>("Inimigo", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("Nome do Inimigo")]
    [SerializeField] private string enemyName;

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("N�vel do Inimigo")]
    private NetworkVariable<int> networkLevel = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("N�vel do Inimigo")]
    [SerializeField] private int level;

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

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("Nome do Inimigo")]
    public string EnemyName
    {
        get => networkEnemyName.Value;
        set
        {
            enemyName = value;
            networkEnemyName.Value = value;
            
        }
    }

    [BoxGroup("Configura��es do Inimigo")]
    [LabelText("N�vel do Inimigo")]
    public int Level
    {
        get => networkLevel.Value;
        set
        {
            level = value;
            networkLevel.Value = value; 
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        EnemyName = enemyName;
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
        float healthModifier = condition ? level * (healthMultiplier * 1.5f) : level * healthMultiplier;
        float defenseModifier = condition ? level * (defenseMultiplier * 1.5f) : level * defenseMultiplier;

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
            nameAndLevelText.text = $"{EnemyName} - Nv. {Level}";
        }
    }

    public void Setup(byte minLevel, byte maxLevel)
    {
        Level = Random.Range(minLevel, maxLevel + 1);
    }

    [Button("Definir N�vel")]
    public void SetLevel(int newLevel)
    {
        Level = newLevel;
    }

    [Button("Definir Nome")]
    public void SetName(string newName)
    {
        EnemyName = newName;
    }

    public int GetLevel()
    {
        return level;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyLevelScaling();
            UpdateNameAndLevelUI();
        }
    }

    /*
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        var name = networkEnemyName.Value;
        var lvl = networkLevel.Value;

        // Serializa o valor do nome do inimigo
        serializer.SerializeValue(ref name);
        // Serializa o valor do n�vel do inimigo
        serializer.SerializeValue(ref lvl);

        // Atualiza os NetworkVariables com os valores serializados/deserializados
        networkEnemyName.Value = name;
        networkLevel.Value = lvl;
    }
    */
}
