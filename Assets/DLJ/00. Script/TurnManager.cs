using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TurnState
{
    PlusyTurn,
    MinusyTurn,
    EnemyTurn,
    Waiting
}



[DefaultExecutionOrder(-100)]
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnState CurrentTurnState { get; private set; }
    
    public Action OnPlusyTurn;
    public Action OnMinusyTurn;

    private event Action OnExecuteAction;
    private int selectedAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        BeginPlayerTurn();
    }

    private void Update()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            BeginPlayerTurn();
        }
    }

    private void BeginPlayerTurn()
    {
        CurrentTurnState = TurnState.PlusyTurn;
        PlusyTurn();
        
    }

    private void BeginEnemyTurn()
    {
        CurrentTurnState = TurnState.EnemyTurn;
    }

    private void PlusyTurn()
    {
        OnPlusyTurn?.Invoke();
    }
    
    public void MinusyTurn()
    {
        CurrentTurnState = TurnState.MinusyTurn;
        OnMinusyTurn?.Invoke();
    }

    public void ReserveAction(PlayerMachine machine, PlayerState state)
    {
        OnExecuteAction += () => machine.Activate(state);
        ++selectedAction;
    }

    public void ExecuteAction()
    {
        if (selectedAction != 2) return;
        CurrentTurnState = TurnState.Waiting;
        OnExecuteAction?.Invoke();
        
        OnExecuteAction = null;
        selectedAction = 0;
    }
    
    public void CompleteAction()
    {
        if (CurrentTurnState != TurnState.Waiting) return;
        
        BeginEnemyTurn();
    }
}
