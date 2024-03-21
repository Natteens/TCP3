using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Entity Stats")]
public class StatusBase : SerializedScriptableObject
{
    [TabGroup("Character Details")]
    public CharacterInfo characterInfo;

    [TabGroup("Status")]
    public Status status;

}

[System.Serializable]
public class CharacterInfo
{
    [PreviewField(50, ObjectFieldAlignment.Left)]
    [VerticalGroup("IconGroup")]
    public Sprite icon;

    [VerticalGroup("IconGroup")]
    public string characterName;
    // Outros detalhes do personagem podem ser adicionados aqui
}

[System.Serializable]
public class Status
{
    [ProgressBar(1, 6, r: 1, g: 0, b: 0)]
    public int constitution; // Vermelho

    [ProgressBar(1, 6, r: 0, g: 0.5f, b: 1)]
    public int strength; // Azul claro

    [ProgressBar(1, 6, r: 1, g: 0.5f, b: 0)]
    public int agility; // Laranja

    [ProgressBar(1, 6, r: 1, g: 1, b: 0)]
    public int precision; // Amarelo claro

    [ProgressBar(1, 6, r: 0, g: 1, b: 0)]
    public int luck; // Verde claro
}
