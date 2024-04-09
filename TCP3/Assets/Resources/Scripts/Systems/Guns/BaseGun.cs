using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

enum GunType
{ 
    Shotgun,
    Submachine,
    Rifle,
    Pistol,
    Sniper
}

//common uncommon legendary epic master elite
enum Rarity
{
    Commom,
    Uncommon,
    Rare,
    Legendary,
    Epic,
    Master,
    Elite
}

[CreateAssetMenu(menuName = "Arma")]
public class BaseGun : ScriptableObject
{

    [SerializeField][TitleGroup("Details")] private string gunName;
    [SerializeField][TitleGroup("Details")] private GunType gunType;
    [SerializeField][TitleGroup("Details")] private Rarity rarity;

    
    [SerializeField][TitleGroup("Weapon Config")][Range(0.1f, 10f)] private float fireRate;
    [SerializeField][TitleGroup("Weapon Config")][Range(1f, 300f)] private float radiusBullets;
    [SerializeField][TitleGroup("Weapon Config")][Range(10f, 1000f)] private float gunRange;
    [SerializeField][TitleGroup("Weapon Config")] [Range(1,250)]private int maxAmmo;
    private int ammo;
    //Status que a arma vai dar

    [SerializeField][TitleGroup("Bullet Config")][Range(1, 20)] private int bulletPerShoot;
    [SerializeField][TitleGroup("Bullet Config")][Range(1f, 200f)] private float bulletSpeed = 50f;
    [SerializeField][TitleGroup("Bullet Config")][Range(1f, 20f)] private float timeToDestroy = 3f;
    [SerializeField][TitleGroup("Bullet Config")] private GameObject bulletPrefab;
    [SerializeField][TitleGroup("Bullet Config")] private GameObject bulletDecal;
    
    //Propriedades
    public float FireRate { get ; set; }
    public float RadiusBullets { get; set; }
    public float GunRange { get { return gunRange; } set { gunRange = value; } }
    public int MaxAmmo { get; set; }
    public int Ammo { get; set; }
    public int BulletPerShoot { get; set; }
    public float BulletSpeed { get; set; }
    public float TimeToDestroy { get; set; }
    public GameObject BulletPrefab { get { return bulletPrefab; } }
    public GameObject BulletDecal { get { return bulletDecal; } }
}
