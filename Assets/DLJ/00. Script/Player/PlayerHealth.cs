using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerTag
{
    Plusy,
    Minusy
}

public class PlayerHealth : AbstractHealthSystem
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private PlayerTag playerTag;
    public PlayerTag Tag => playerTag;
    private bool isDefeated;
    
    private void Start()
    {
        SetHealth(100);
    }

    private void Update()
    {
        playerUI?.SetSlider(playerTag, Health);

        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            OnDamaged?.Invoke(10);
        }
    }

    public override void Damage(int damage)
    {
        if (isDefeated || damage <= 0) return;
        base.Damage(damage);
        playerUI?.SetSlider(playerTag, Health);
    }

    public override void Dead()
    {
        if (isDefeated) return;
        isDefeated = true;
        playerUI?.SetSlider(playerTag, 0);
        playerUI?.SetPlayerDefeated(playerTag);

        foreach (Transform child in GetComponentsInChildren<Transform>(true)) child.DOKill();
        gameObject.SetActive(false);
        TurnManager.Instance?.NotifyPlayerDefeated(playerTag);
    }

    private void OnDisable()
    {
        OnDamaged -= Damage;
        OnHeal -= Heal;
        OnDead -= Dead;
    }
}
