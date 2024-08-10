using UnityEngine;

public abstract class EnemyAttackStateSOBase : ScriptableObject, IEnemyState
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected Transform playerTransform;
  //  protected StatusEffect damageEffect;

    public virtual void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;
     //   playerTransform = Player.Instance.transform;
     //   damageEffect = enemy.EnemyEffect;
    }

    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() { }
    public virtual void DoPhysicsLogic() { }
    public virtual void ResetValues() { }
    public virtual void EventOnAttackAnimationIn() { }
    public virtual void EventOnAttackAnimationOut() { }
}
