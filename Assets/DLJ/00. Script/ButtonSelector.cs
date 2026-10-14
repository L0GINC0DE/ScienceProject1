using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonSelector : MonoBehaviour
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private Player player;
    
    
    private List<Vector2> btnPositions = new();
    [SerializeField] private int currentIndex = 0;
    

    private void Update()
    {
        if (TurnManager.Instance.CurrentTurnState != TurnState.PlayerTurn) return;
        
        btnPositions.Clear();
        
        for (int i = 0; i < 2; i++)
        {
            btnPositions.Add(buttons[i].transform.position);
        }
        
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            currentIndex++;
            if (currentIndex >= btnPositions.Count) currentIndex = 0;
            indicator.transform.position = btnPositions[currentIndex];
        }
        
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = 1;
            indicator.transform.position = btnPositions[currentIndex];
        }
        
        PlayerMachine.ChangeState(currentIndex == 0 ? PlayerState.Attack : PlayerState.Act);
        
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Plusy plusy = player as Plusy;
            plusy?.playerMachine.Activate(PlayerMachine.CurrentState);
        }
    }
}
