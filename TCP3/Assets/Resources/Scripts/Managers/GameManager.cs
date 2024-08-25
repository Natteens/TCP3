using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Dynamic;

public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;
    public UI_Inventory uiInventory;
    public UI_Craft uiCraft;
    public GameObject interactMSG;
    public GameObject HarvestHolder;

    public Image health, stamina, hunger, thirsty;

    [BoxGroup("Day/Night Cycle")]
    [ReadOnly] public NetworkVariable<short> timeOfDay { get; set; } = new NetworkVariable<short>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [BoxGroup("Day/Night Cycle")] public short dayDuration = 120;
    [BoxGroup("Day/Night Cycle")] public Light directionalLight;
    [BoxGroup("Day/Night Cycle")] public Gradient lightColorGradient;
    [BoxGroup("Day/Night Cycle")] public AnimationCurve lightIntensityCurve;

    public bool isNight;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateDayTime), 0f, .5f);
    }

    public void UpdateDayTime()
    {
        timeOfDay.Value++;
        bool CheckIfisNight = timeOfDay.Value >= dayDuration * .3f && timeOfDay.Value <= dayDuration * .65f;
        isNight = CheckIfisNight;

        if (timeOfDay.Value >= dayDuration) timeOfDay.Value = 0;
    }
}
