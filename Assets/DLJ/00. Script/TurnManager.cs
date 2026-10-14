using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

[DefaultExecutionOrder(-100)]
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnState CurrentTurnState { get; private set; } = TurnState.EnemyTurn;

    public Action OnPlayerTurnStart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
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
        CurrentTurnState = TurnState.PlayerTurn;
        OnPlayerTurnStart?.Invoke();
    }
}
