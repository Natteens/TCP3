using UnityEngine;

public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.DoEnterLogic();
    }

    public void ChangeState(IState newState)
    {
        CurrentState.DoExitLogic();
        CurrentState = newState;
        newState.DoEnterLogic();
    }
}

public interface IState
{
    void Initialize(GameObject gameObject);
    void DoEnterLogic();
    void DoExitLogic();
    void DoFrameUpdateLogic();
    void DoPhysicsLogic();
    void ResetValues();
}
