using System;
using DG.Tweening;
using UnityEngine;

public class PlayerAct : PlayerButton, IButtonAction
{
    private Player player;

    public PlayerAct(Player player, Transform target) : base(player, target)
    {
        this.player = player;
    }

    public void OnClick()
    {
        Act();
    }

    private void Act()
    {
        PlayerAnimation anim = player.GetComponentInChildren<PlayerAnimation>();
        if (player.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.Heal(20);
        }
        anim.SetTriggerAnimation("Act");
    }
}
