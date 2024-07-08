using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private GameEvent gameEvent;

    public void TriggerEvent()
    {
        gameEvent.Raise();
    }
}
