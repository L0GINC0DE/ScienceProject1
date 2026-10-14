using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Attack,
    Act
}

public class PlayerMachine
{
    private Dictionary<PlayerState, PlayerButton> playerDict  = new();

    public PlayerState CurrentState { get; private set; }

    public void AddPlayer(PlayerState state, PlayerButton button)
    {
        playerDict.Add(state, button);
    }
}
