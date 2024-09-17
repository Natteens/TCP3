using Unity.Netcode;
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

    public float detectionRange = 5f;
    private bool isPlayerNear = false;

    // VFX para os pianos
    private string[] pianoVFX = { "p1", "p2", "p3", "p4" };

    // Controla o range de spawn para os VFX no eixo X e Z (Y fixo)
    public float spawnRangeX = 2f;
    public float spawnRangeZ = 2f;

    void Start()
    {
        // Inscreve o método PianoKey para cada tecla
        onKey1.AddListener(() => PianoKey("piano1"));
        onKey2.AddListener(() => PianoKey("piano2"));
        onKey3.AddListener(() => PianoKey("piano3"));
        onKey4.AddListener(() => PianoKey("piano4"));
        onKey5.AddListener(() => PianoKey("piano5"));
        onKey6.AddListener(() => PianoKey("piano6"));
        onKey7.AddListener(() => PianoKey("piano7"));
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
        }
    }

    public void PianoKey(string s)
    {
        // Aciona o som de piano
        Spawner.Instance.SpawnInWorldServerRpc(transform.position, s);

        // Chama uma VFX aleatória em uma posição próxima
        PlayRandomVFX();
    }

    private void PlayRandomVFX()
    {
        // Seleciona uma VFX aleatória entre p1, p2, p3, p4
        string randomVFX = pianoVFX[Random.Range(0, pianoVFX.Length)];

        // Define uma posição aleatória próxima ao transform atual (Y permanece o mesmo)
        Vector3 randomPosition = new Vector3(
            transform.position.x + Random.Range(-spawnRangeX, spawnRangeX),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRangeZ, spawnRangeZ)
        );

        // Aciona a VFX na posição aleatória
        Spawner.Instance.SpawnInWorldServerRpc(randomPosition, randomVFX);
    }

    // Depuração visual para mostrar a área de spawn
    private void OnDrawGizmosSelected()
    {
        // Define a cor para o gizmo de depuração
        Gizmos.color = Color.yellow;

        // Desenha uma área retangular ao redor do transform mostrando o range de spawn
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRangeX * 2, 0.1f, spawnRangeZ * 2));
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
