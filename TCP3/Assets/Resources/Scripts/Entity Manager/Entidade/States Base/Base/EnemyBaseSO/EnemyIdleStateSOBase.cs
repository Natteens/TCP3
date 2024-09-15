using UnityEngine;

public abstract class EnemyIdleStateSOBase : ScriptableObject, IState
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = gameObject.GetComponent<Enemy>();
    }

    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() { }
    public virtual void DoPhysicsLogic() { }
    public virtual void ResetValues() { }
}
