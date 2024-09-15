using UnityEngine;

public abstract class IdleStateSOBase : ScriptableObject, IState
{
    protected NPC npc;
    protected Transform transform;
    protected GameObject gameObject;
    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.npc = gameObject.GetComponent<NPC>();
    }

    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() { }
    public virtual void DoPhysicsLogic() { }
    public virtual void ResetValues() { }
}
