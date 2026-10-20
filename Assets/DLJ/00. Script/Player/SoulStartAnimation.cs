using DG.Tweening;
using UnityEngine;

public class SoulStartAnimation : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform ingame;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D soulBody;
    private Sequence soulSequence;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        soulBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        TurnManager.Instance.OnEnemyTurn += SoulAnimation;
        TurnManager.Instance.OnPlayerTurnStarted += ReturnToPlayer;
    }

    private void SoulAnimation()
    {
        soulSequence?.Kill();
        soulBody.simulated = false;

        soulSequence = DOTween.Sequence();
        soulSequence.Append(transform.DOMove(ingame.position, 1f).SetEase(Ease.OutQuad));
        soulSequence.Join(transform.DOScale(Vector3.one * 0.5f, 0.5f));
        soulSequence.Join(spriteRenderer.DOFade(1f, 1f));
        soulSequence.OnComplete(() =>
        {
            soulBody.position = transform.position;
            soulBody.simulated = true;
        });
    }

    private void ReturnToPlayer()
    {
        soulSequence?.Kill();
        soulBody.simulated = false;

        soulSequence = DOTween.Sequence();
        soulSequence.Append(transform.DOMove(player.position, 0.8f).SetEase(Ease.InOutQuad));
        soulSequence.Join(spriteRenderer.DOFade(0f, 0.8f));
    }

    private void OnDisable()
    {
        soulSequence?.Kill();
        soulBody.simulated = false;

        if (TurnManager.Instance == null) return;
        TurnManager.Instance.OnEnemyTurn -= SoulAnimation;
        TurnManager.Instance.OnPlayerTurnStarted -= ReturnToPlayer;
    }
}
