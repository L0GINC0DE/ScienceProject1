using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : AbstractHealthSystem
{
    [SerializeField] private PlayerUI playerUI;
    private void Start()
    {
        SetHealth(100);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        
        OnDamaged += playerUI.SetSlider;
    }

    private void Update()
    {
        playerUI.SetSlider(Health);

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            OnDamaged?.Invoke(10);
        }
    }

    private void OnDisable()
    {
        OnDamaged -= playerUI.SetSlider;
    }
}
