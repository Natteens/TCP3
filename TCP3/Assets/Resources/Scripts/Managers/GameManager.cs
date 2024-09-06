using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Dynamic;

public class GameManager : Singleton<GameManager>
{
    [BoxGroup("Camera")] public Camera mainCamera;
    [BoxGroup("Camera")] public CinemachineVirtualCamera virtualCamera;
    [BoxGroup("Player References")] public UI_Inventory uiInventory;
    [BoxGroup("Player References")] public UI_Craft uiCraft;
    [BoxGroup("UI References")] public Image health, stamina, hunger, thirsty;
    [BoxGroup("UI References")] public GameObject interactMSG;
    [BoxGroup("UI References")] public GameObject HarvestHolder;
    [BoxGroup("RespawnPoint")] public Transform spawnPoint;


    [BoxGroup("Day/Night Cycle")]
    [ReadOnly] public NetworkVariable<float> timeOfDay { get; set; } = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [BoxGroup("Day/Night Cycle")] public short dayDuration;
    [BoxGroup("Day/Night Cycle")] public Light directionalLight;
    [BoxGroup("Day/Night Cycle")] public Gradient lightColorGradient;
    [BoxGroup("Day/Night Cycle")] public AnimationCurve lightIntensityCurve;
    [BoxGroup("Day/Night Cycle")]
    public NetworkVariable<bool> isNight = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        if (IsServer)
        {
            InvokeRepeating(nameof(UpdateDayTime), 0f, .05f);
        }
    }

    private void UpdateDayTime()
    {
        if (timeOfDay.Value >= dayDuration)
        {
            timeOfDay.Value = 0;
            return;
        }

        timeOfDay.Value += Time.deltaTime;
        bool CheckIfisNight = timeOfDay.Value >= dayDuration * .3f && timeOfDay.Value <= dayDuration * .65f;
        isNight.Value = CheckIfisNight;
        float newXvalue = ((timeOfDay.Value / (float)dayDuration) * 100f) + 40f;

        Vector3 newRotation = new(Mathf.Lerp(newXvalue, newXvalue + 1f, .3f), directionalLight.gameObject.transform.rotation.y, directionalLight.gameObject.transform.rotation.z);
        directionalLight.gameObject.transform.rotation = Quaternion.Euler(newRotation);
        //Debug.Log(newRotation);
    }

    public bool CheckIsNight()
    {
        return isNight.Value;
    }

    public void GoToRespawnPoint(Transform transform)
    {
        transform.position = spawnPoint.position;
    }

}
