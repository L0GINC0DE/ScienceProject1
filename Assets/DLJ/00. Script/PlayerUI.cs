using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image PlayerMainUI;

    private void OnEnable()
    {
        TurnManager.Instance.OnPlayerTurnStart += ShowUI;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnPlayerTurnStart -= ShowUI;
    }

    public void ShowUI()
    {
        //PlayerMainUI
    }
}
