using System;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnState CurrentTurnState { get; private set; }

    public Action OnPlayerTurnStart;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void BeginPlayerTurn()
    {
        CurrentTurnState = TurnState.PlayerTurn;
    }
}
