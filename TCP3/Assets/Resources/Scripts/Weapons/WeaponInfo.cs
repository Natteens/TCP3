using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Create Weapon")]
public class WeaponInfo : Item
{
    public enum WeaponType
    {
        Fuzil,
        FuzilRajada,
        Revolver,
        SubMetralhadora,
        Rifle,
        Escopeta
    }
    [Header("Weapon Configs")]
    public WeaponType weaponType;
    public int maxMunition;
    public float cadence;
    public float reloadSpeed;
   [Range(0.01f, 1f)] public float spread;
    public int bulletPerShoot;
    public int damage;
    public int defensePenetration;
    public bool isAutomatic;
    public Vector3 spawnPosition;
    public Transform bulletPrefab;
    public List<StatusEffect> effects;

    [Header("Animation Configs")]
    public string animatorParameter; // Nome do parâmetro no Animator
    public float weaponState; // Estado da arma na blend tree (1 a 4)

    private void OnValidate()
    {
        amount = 1;
        itemType = Itemtype.Arma;

        string newInfos =     "<color=red>Dano: " + damage.ToString() +
                              "\nPenetração de Armadura: " + defensePenetration.ToString() +
                              "\nMunição máxima: " + maxMunition.ToString() +
                              "\nVelocidade de Recarga: " + reloadSpeed.ToString() +
                              "\nCadência: " + cadence.ToString() + "</color>";

        itemDescription = newInfos;
    }

}
