using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [SerializeField] private string defeatSceneName = "Dead";

    public TurnState CurrentTurnState { get; private set; }
    
    public Action OnPlusyTurn;
    public Action OnMinusyTurn;
    public Action OnEnemyTurn;
    public Action OnPlayerTurnStarted;

    private event Action OnExecuteAction;
    private int selectedAction;
    private PlayerHealth[] players;
    private bool isChangingScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

        players = FindObjectsByType<PlayerHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    private void Start()
    {
        BeginPlayerTurn();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            //BeginPlayerTurn();
        }
    }

    private void BeginPlayerTurn()
    {
        OnExecuteAction = null;
        selectedAction = 0;
        CurrentTurnState = IsPlayerAlive(PlayerTag.Plusy) ? TurnState.PlusyTurn
            : IsPlayerAlive(PlayerTag.Minusy) ? TurnState.MinusyTurn : TurnState.Waiting;
        OnPlayerTurnStarted?.Invoke();
        if (CurrentTurnState == TurnState.PlusyTurn) OnPlusyTurn?.Invoke();
        else if (CurrentTurnState == TurnState.MinusyTurn) OnMinusyTurn?.Invoke();
    }

    public void CompleteEnemyTurn()
    {
        if (CurrentTurnState != TurnState.EnemyTurn) return;
        BeginPlayerTurn();
    }

    public void MinusyTurn()
    {
        if (!IsPlayerAlive(PlayerTag.Minusy))
        {
            ExecuteAction();
            return;
        }
        CurrentTurnState = TurnState.MinusyTurn;
        OnMinusyTurn?.Invoke();
    }

    public void ReserveAction(PlayerMachine machine, PlayerState state)
    {
        if (machine == null || (CurrentTurnState != TurnState.PlusyTurn &&
            CurrentTurnState != TurnState.MinusyTurn)) return;
        PlayerTag tag = CurrentTurnState == TurnState.PlusyTurn ? PlayerTag.Plusy : PlayerTag.Minusy;
        if (!IsPlayerAlive(tag)) return;
        OnExecuteAction += () =>
        {
            if (IsPlayerAlive(tag)) machine.Activate(state);
        };
        ++selectedAction;
    }

    public void ExecuteAction()
    {
        int livingPlayers = (IsPlayerAlive(PlayerTag.Plusy) ? 1 : 0) + (IsPlayerAlive(PlayerTag.Minusy) ? 1 : 0);
        if (livingPlayers == 0 || selectedAction < livingPlayers) return;
        CurrentTurnState = TurnState.Waiting;
        Action actions = OnExecuteAction;
        OnExecuteAction = null;
        selectedAction = 0;
        actions?.Invoke();
    }
    
    public void CompleteAction()
    {
        if (CurrentTurnState != TurnState.Waiting) return;
        if (!IsPlayerAlive(PlayerTag.Plusy) && !IsPlayerAlive(PlayerTag.Minusy)) return;
        CurrentTurnState = TurnState.EnemyTurn;
        OnEnemyTurn?.Invoke();
    }

    public bool IsPlayerAlive(PlayerTag tag)
    {
        if (players == null) return false;
        foreach (PlayerHealth player in players)
            if (player != null && player.Tag == tag && player.isActiveAndEnabled && player.Health > 0)
                return true;
        return false;
    }

    public void NotifyPlayerDefeated(PlayerTag tag)
    {
        if (isChangingScene) return;

        if (!IsPlayerAlive(PlayerTag.Plusy) && !IsPlayerAlive(PlayerTag.Minusy))
        {
            isChangingScene = true;
            CurrentTurnState = TurnState.Waiting;
            OnExecuteAction = null;
            selectedAction = 0;
            Time.timeScale = 1f;
            SceneManager.LoadScene(defeatSceneName);
            return;
        }

        if (CurrentTurnState == TurnState.PlusyTurn && tag == PlayerTag.Plusy) MinusyTurn();
        else if (CurrentTurnState == TurnState.MinusyTurn && tag == PlayerTag.Minusy) ExecuteAction();
    }
}
