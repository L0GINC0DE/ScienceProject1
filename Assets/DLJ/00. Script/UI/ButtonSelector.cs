using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonSelector : MonoBehaviour
{
    [SerializeField] private List<Indicator> indicator;
    [SerializeField] private List<GameObject> plusyButtons;
    [SerializeField] private List<GameObject> minusyButtons;
    [SerializeField] private Plusy plusy;
    [SerializeField] private Minusy minusy;
    
    public List<Vector2> BtnPositions { get; private set; } = new();
    private int currentIndex = 0;

    public static Action OnChoose;

    private void Update()
    {
        if (TurnManager.Instance.CurrentTurnState != TurnState.PlusyTurn &&
            TurnManager.Instance.CurrentTurnState != TurnState.MinusyTurn) return;
        
        currentIndex = TurnManager.Instance.CurrentTurnState == TurnState.PlusyTurn ? 0 : 1;

        PlayerTag currentTag = currentIndex == 0 ? PlayerTag.Plusy : PlayerTag.Minusy;
        if (!TurnManager.Instance.IsPlayerAlive(currentTag)) return;
        
        List<GameObject> currentButtons = currentIndex == 0 ? plusyButtons : minusyButtons;
        
        BtnPositions.Clear();
        
        for (int i = 0; i < 2; i++)
        {
            BtnPositions.Add(currentButtons[i].transform.position);
        }
        
        indicator[currentIndex].ChangeButton();

        PlayerMachine machine = TurnManager.Instance.CurrentTurnState switch
        {
            TurnState.PlusyTurn => plusy.playerMachine,
            TurnState.MinusyTurn => minusy.playerMachine,
            _ => null
        };
        
        if (machine == null) return;

        int actionIndex = indicator[currentIndex].GetIndex();
        
        machine.ChangeState(actionIndex == 0 ? PlayerState.Attack : PlayerState.Act);
        
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TurnManager.Instance.ReserveAction(machine, machine.CurrentState);
            
            OnChoose?.Invoke();
            
            if (TurnManager.Instance.CurrentTurnState == TurnState.PlusyTurn) 
                TurnManager.Instance.MinusyTurn();
            else if (TurnManager.Instance.CurrentTurnState == TurnState.MinusyTurn)
                TurnManager.Instance.ExecuteAction();
        }
    }
}
