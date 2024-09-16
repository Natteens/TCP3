using UnityEngine;
using UnityEngine.Events;

public class PlayingPiano : MonoBehaviour
{
    // Eventos para cada tecla
    public UnityEvent onKey1;
    public UnityEvent onKey2;
    public UnityEvent onKey3;
    public UnityEvent onKey4;
    public UnityEvent onKey5;
    public UnityEvent onKey6;
    public UnityEvent onKey7;
    public UnityEvent onKey8;
    public UnityEvent onKey9;
    public UnityEvent onKey0;

    public float detectionRange = 5f;
    private bool isPlayerNear = false;

    void Start()
    {
        onKey1.AddListener(() => PianoKey("piano1"));
        onKey2.AddListener(() => PianoKey("piano2"));
        onKey3.AddListener(() => PianoKey("piano3"));
        onKey4.AddListener(() => PianoKey("piano4"));
        onKey5.AddListener(() => PianoKey("piano5"));
        onKey6.AddListener(() => PianoKey("piano6"));
        onKey7.AddListener(() => PianoKey("piano7"));
        onKey8.AddListener(() => PianoKey("piano8"));
        onKey9.AddListener(() => PianoKey("piano9"));
        onKey0.AddListener(() => PianoKey("piano0"));
    }

    void Update()
    {
        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) onKey1?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha2)) onKey2?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha3)) onKey3?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha4)) onKey4?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha5)) onKey5?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha6)) onKey6?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha7)) onKey7?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha8)) onKey8?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha9)) onKey9?.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha0)) onKey0?.Invoke();
        }
    }

    public void PianoKey(string s)
    {
        Spawner.Instance.SpawnInWorldServerRpc(transform.position, s);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
