using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerButton
{
    protected Player player;
    protected Transform target;
    protected Vector2 basePos;

    public PlayerButton(Player player, Transform target)
    {
        this.player = player;
        this.target = target;
        basePos = player.transform.position;
    }
    
}
