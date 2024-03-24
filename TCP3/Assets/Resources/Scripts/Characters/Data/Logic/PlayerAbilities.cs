using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "PlayerAbilities")]
public class PlayerAbilities : ScriptableObject
{
    #region Constituicao
    [BoxGroup("Status"), InlineEditor]
    public VidaMax VidaMax;

    [BoxGroup("Status"), InlineEditor]
    public VigorMax VigorMax;

    [BoxGroup("Status"), InlineEditor]
    public FomeMax FomeMax;

    [BoxGroup("Status"), InlineEditor]
    public RegenHP RegenHP;

    [BoxGroup("Status"), InlineEditor]
    public RegenStamina RegenStamina;
    #endregion

    #region Agilidade
    [BoxGroup("Status"), InlineEditor]
    public JumpHeight JumpHeight;
    [BoxGroup("Status"), InlineEditor]
    public MoveSpeed MoveSpeed;
    [BoxGroup("Status"), InlineEditor]
    public MoveSpeed RunSpeed;

    #endregion

    
    public void IncrementStatsByLevel(Status status)
    {
        // Constituição
        VidaMax.GetValue(status);
        VigorMax.GetValue(status);

        // Agilidade
        MoveSpeed.GetValue(status);
        RunSpeed.GetValue(status);
        JumpHeight.GetValue(status);

        // Precisao
        // Força
        // Sorte
    }
}
