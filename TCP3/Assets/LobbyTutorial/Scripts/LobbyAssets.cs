using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite marineSprite;
    [SerializeField] private Sprite ninjaSprite;
    [SerializeField] private Sprite zombieSprite;


    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(LobbyManagerCodeMonkey_.PlayerCharacter playerCharacter) {
        switch (playerCharacter) {
            default:
            case LobbyManagerCodeMonkey_.PlayerCharacter.Marine:   return marineSprite;
            case LobbyManagerCodeMonkey_.PlayerCharacter.Ninja:    return ninjaSprite;
            case LobbyManagerCodeMonkey_.PlayerCharacter.Zombie:   return zombieSprite;
        }
    }

}