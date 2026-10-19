using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SoulStartAnimation : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform ingame;
    
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        TurnManager.Instance.OnEnemyTurn += SoulAnimation;
    }

    private void SoulAnimation()
    {
        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
        spriteRenderer.DOFade(1, 1f);
        transform.DOMove(ingame.position, 1f).SetEase(Ease.OutQuad);
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnEnemyTurn -= SoulAnimation;
    }
}
