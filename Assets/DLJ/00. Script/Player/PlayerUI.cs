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
    private readonly HashSet<PlayerTag> defeatedPlayers = new();

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
        if (TurnManager.Instance.CurrentTurnState == TurnState.PlusyTurn)
        {
            CurrentPlayerUI = mainUIs[0];
        }
        else
        {
            CurrentPlayerUI = mainUIs[1];
        }

        PlayerTag tag = TurnManager.Instance.CurrentTurnState == TurnState.PlusyTurn
            ? PlayerTag.Plusy : PlayerTag.Minusy;
        if (defeatedPlayers.Contains(tag)) CurrentPlayerUI = null;

        if (CurrentPlayerUI == null) return;
        
        CurrentPlayerUI.transform.DOLocalMoveY(CurrentPlayerUI.transform.position.y - 170f, 0.18f).SetEase(easeCurve);
    }

    private void DownUI()
    {
        if (CurrentPlayerUI == null) return;
        
        CurrentPlayerUI.transform.DOLocalMoveY(CurrentPlayerUI.transform.position.y - 255f, 0.18f).SetEase(easeCurve);
    }
    
    public void SetSlider(PlayerTag tag,int health)
    {
        GameObject ui = GetPlayerUI(tag);
        if (ui == null) return;
        Slider slider = ui.GetComponentInChildren<Slider>(true);
        if (slider != null) slider.value = health;
    }

    public void SetPlayerDefeated(PlayerTag tag)
    {
        if (!defeatedPlayers.Add(tag)) return;
        GameObject ui = GetPlayerUI(tag);
        if (ui == null) return;

        ui.transform.DOKill();
        CanvasGroup group = ui.GetComponent<CanvasGroup>();
        if (group == null) group = ui.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
        foreach (Selectable selectable in ui.GetComponentsInChildren<Selectable>(true))
            selectable.interactable = false;
        foreach (Indicator indicator in ui.GetComponentsInChildren<Indicator>(true))
            indicator.gameObject.SetActive(false);

        if (CurrentPlayerUI == ui) CurrentPlayerUI = null;
    }

    private GameObject GetPlayerUI(PlayerTag tag)
    {
        int index = (int)tag;
        return index >= 0 && index < mainUIs.Count ? mainUIs[index] : null;
    }
}
