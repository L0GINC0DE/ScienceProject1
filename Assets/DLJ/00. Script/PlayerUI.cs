using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> mainUIs = new();
    public GameObject CurrentPlayerUI { get; private set; }
    [SerializeField] private Ease easeCurve;
    private Slider currentSlider;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        TurnManager.Instance.OnPlusyTurn += ShowUI;
        TurnManager.Instance.OnMinusyTurn += ShowUI;
        
        ButtonSelector.OnChoose += DownUI;
    }


    private void OnDisable()
    {
        TurnManager.Instance.OnPlusyTurn -= ShowUI;
        TurnManager.Instance.OnMinusyTurn -= ShowUI;
        
        ButtonSelector.OnChoose -= DownUI;
        
    }

    private void ShowUI()
    {
        RefreshCurrentUI();

        if (CurrentPlayerUI == null) return;
        
        CurrentPlayerUI.transform.DOLocalMoveY(CurrentPlayerUI.transform.position.y - 65f, 0.18f).SetEase(easeCurve);
    }

    private void DownUI()
    {
        if (CurrentPlayerUI == null) return;
        
        CurrentPlayerUI.transform.DOLocalMoveY(CurrentPlayerUI.transform.position.y - 120f, 0.18f).SetEase(easeCurve);
    }
    
    private void RefreshCurrentUI()
    {
        if (TurnManager.Instance.CurrentTurnState == TurnState.PlusyTurn)
        {
            CurrentPlayerUI = mainUIs[0];
        }
        else
        {
            CurrentPlayerUI = mainUIs[1];
        }
    }
    
    
    public void SetSlider(PlayerTag tag,int health)
    {
        currentSlider = tag switch
        {
            PlayerTag.Plusy => mainUIs[0].GetComponentInChildren<Slider>(),
            PlayerTag.Minusy => mainUIs[1].GetComponentInChildren<Slider>()
        };
        currentSlider.value = health;
    }
}
