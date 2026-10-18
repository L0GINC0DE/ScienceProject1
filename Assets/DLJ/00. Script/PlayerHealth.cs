using System;
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
    
    private void Start()
    {
        SetHealth(100);
    }

    private void Update()
    {
        playerUI.SetSlider(playerTag, Health);

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            OnDamaged?.Invoke(10);
        }
    }
}
