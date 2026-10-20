using DG.Tweening;
using UnityEngine;

public class IngameOpening : MonoBehaviour
{
    [SerializeField] private GameObject ingame;

    public Transform IngameTransform => ingame != null ? ingame.transform : null;

    private Transform ingameTransform;
    private RectTransform ingameRectTransform;
    private TurnManager turnManager;
    private Tween scaleTween;
    private Vector3 openedScale;
    private bool isOpen;

    private void Awake()
    {
        ingameTransform = ingame.transform;
        ingameRectTransform = ingame.GetComponent<RectTransform>();
        openedScale = ingameTransform.localScale;

        if (Mathf.Approximately(openedScale.x, 0f)) openedScale.x = 1f;
    }

    private void OnEnable()
    {
        turnManager = TurnManager.Instance;
        if (turnManager == null) return;

        turnManager.OnEnemyTurn += ShowIngame;
        turnManager.OnPlayerTurnStarted += DisableIngame;
    }

    private void ShowIngame()
    {
        isOpen = true;
        ingame.SetActive(true);
        AnimateOpen();
    }

    public void SetIngameSize(Vector2 size)
    {
        if (size.x <= 0f || size.y <= 0f) return;

        if (ingameRectTransform != null)
        {
            ingameRectTransform.sizeDelta = size;
            return;
        }

        openedScale.x = size.x;
        openedScale.y = size.y;
        if (isOpen) AnimateOpen();
    }

    private void AnimateOpen()
    {
        scaleTween?.Kill();
        scaleTween = ingameTransform.DOScale(openedScale, 0.8f);
    }

    private void DisableIngame()
    {
        isOpen = false;
        scaleTween?.Kill();
        scaleTween = ingameTransform.DOScaleX(0f, 0.8f);
    }

    private void OnDisable()
    {
        scaleTween?.Kill();

        if (turnManager == null) return;
        turnManager.OnEnemyTurn -= ShowIngame;
        turnManager.OnPlayerTurnStarted -= DisableIngame;
    }
}
