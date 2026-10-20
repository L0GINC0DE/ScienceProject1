using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Plusy : Player
{
    public PlayerMachine playerMachine;
    public Transform targetPos;
    

    public override void Awake()
    {
        base.Awake();
        
        playerMachine = new PlayerMachine();
        
        playerMachine.AddPlayer(PlayerState.Attack, new PlayerAttack(this, targetPos));
        playerMachine.AddPlayer(PlayerState.Act, new PlayerAct(this, targetPos));
    }
}
