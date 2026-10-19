using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class PlayerAttack : PlayerButton, IButtonAction
{
    private Player player;
    private Transform targetPos;
    private Vector2 basePos;

    private Sequence attackSequence;
    
    public PlayerAttack(Player player, Transform target) : base(player, target)
    {
        this.player = player;
        this.target = target;
        basePos = player.transform.position;
    }

    public void OnClick()
    {
        Attack();
    }

    private void Attack()
    {
        PlayerAnimation anim = player.GetComponentInChildren<PlayerAnimation>();
        anim.SetBoolAnimation("Attack", true);

        attackSequence.Kill();
        
        attackSequence = DOTween.Sequence();
        attackSequence.Append(player.transform.DOJump(target.transform.position, 2.3f, 1, 1.2f)).SetEase(Ease.OutSine)
            .AppendInterval(0.1f)
            .Append(player.transform.DOJump(basePos, 1.3f, 1, 0.9f)).SetEase(Ease.OutCirc)
            .JoinCallback(() => anim.SetBoolAnimation("Attack", false))
            .AppendCallback(() => TurnManager.Instance.CompleteAction());
    }
}
