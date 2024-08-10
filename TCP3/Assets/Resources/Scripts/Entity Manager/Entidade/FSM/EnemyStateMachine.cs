using UnityEngine;

public class EnemyStateMachine
{
    public IEnemyState CurrentEnemyState { get; private set; }

    public void Initialize(IEnemyState startingState)
    {
        CurrentEnemyState = startingState;
        startingState.DoEnterLogic();
    }

    public void ChangeState(IEnemyState newState)
    {
        CurrentEnemyState.DoExitLogic();
        CurrentEnemyState = newState;
        newState.DoEnterLogic();
    }
}

public interface IEnemyState
{
    void Initialize(GameObject gameObject, Enemy enemy);
    void DoEnterLogic();
    void DoExitLogic();
    void DoFrameUpdateLogic();
    void DoPhysicsLogic();
    void ResetValues();
}
