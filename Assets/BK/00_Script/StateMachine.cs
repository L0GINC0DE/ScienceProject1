using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Attacking,
    Defending
}

public class StateMachine
{
    public Dictionary<EnemyState, State> states = new ();

    public State CurrentState {get; private set; }
    public Action<State> OnStateChange { get; private set; }
    public BossHealthSystem BossHealthSystem { get; private set; }

    public void Awake()
    {
        BossHealthSystem = new BossHealthSystem();
        states.Add(EnemyState.Idle,new IdleState(this));
    }

    private void OnEnable()
    {
        OnStateChange += ChangeState;
    }

    public void ChangeState(State newState)
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }

    public void Update()
    {
        CurrentState.UpdateState();
    }
}
