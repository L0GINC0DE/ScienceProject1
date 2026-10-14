using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum PlayerState
{
    Attack,
    Act
}

public class PlayerMachine
{
    private Dictionary<PlayerState, IButtonAction> playerDict  = new();

    public static PlayerState CurrentState { get; private set; }

    public void AddPlayer(PlayerState state, IButtonAction button)
    {
        playerDict.Add(state, button);
    }

    public static void ChangeState(PlayerState state)
    {
        CurrentState = state;
    }

    public void Activate(PlayerState state)
    {
        IButtonAction buttonAction = playerDict[state];
        buttonAction.OnClick();
    }
}
