using DG.Tweening;
using UnityEngine;

public class IngameOpening : MonoBehaviour
{
    [SerializeField] private GameObject ingame;

    private void OnEnable()
    {
        TurnManager.Instance.OnEnemyTurn += ShowIngame;
        TurnManager.Instance.OnPlusyTurn += DisableIngame;
    }

    private void ShowIngame()
    {
        ingame.SetActive(true);
        ingame.transform.DOScaleX(1, 0.8f);
    }

    private void DisableIngame()
    {
        ingame.transform.DOScaleX(0, 0.8f);
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnEnemyTurn -= ShowIngame;
        TurnManager.Instance.OnPlusyTurn -= DisableIngame;
    }
}
