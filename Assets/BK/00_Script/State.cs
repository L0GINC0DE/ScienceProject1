using UnityEngine;



public abstract class State
{
  public StateMachine _stateMachine;

  public State(StateMachine stateMachine)
  {
    _stateMachine = stateMachine;
  }

  public virtual void EnterState()
  {
  }

  public virtual void ExitState()
  {
  }

  public virtual void UpdateState()
  {
 
  }
}
