using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Indicator : MonoBehaviour
{
    private ButtonSelector btnSelector;
    private int index;

    private void Awake()
    {
        btnSelector = GetComponentInParent<ButtonSelector>();
    }

    public void ChangeButton()
    {
        if (TurnManager.Instance.CurrentTurnState != TurnState.PlusyTurn && TurnManager.Instance.CurrentTurnState != TurnState.MinusyTurn) return;
        
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            index++;
            if (index >= btnSelector.BtnPositions.Count) index = 0;
            Debug.Log(btnSelector);
            transform.position = btnSelector.BtnPositions[index];
        }
        
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            index--;
            if (index < 0) index = 1;
            transform.position = btnSelector.BtnPositions[index];
        }
    }

    public int GetIndex()
    {
        return index;
    }
}
