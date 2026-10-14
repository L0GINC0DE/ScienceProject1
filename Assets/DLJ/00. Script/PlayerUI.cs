using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image playerMainUI;
    [SerializeField] private Ease easeCurve;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        TurnManager.Instance.OnPlayerTurnStart += ShowUI;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnPlayerTurnStart -= ShowUI;
    }

    public void ShowUI()
    {
        playerMainUI.transform.DOLocalMoveY(playerMainUI.rectTransform.anchoredPosition.y + 75f, 0.18f).SetEase(easeCurve);
    }
    
    public void SetSlider(int health)
    {
        slider.value = health;
    }
}
