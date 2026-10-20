using System;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthSystem : AbstractHealthSystem
{
    [SerializeField]private Slider _healthSlider;
    private bool first = true;
    private void Start()
    {
        _healthSlider.maxValue = 1000;
        _healthSlider.value = 1000;
        SetHealth(1000);
    }
    

    private void Update()
    {
        SetHealth();
    }

    private void OnDisable()
    {
        OnDamaged -= Damage;
    }


    public void SetHealth()
    {
        _healthSlider.value = Health;
        Debug.Log(Health);
    }
}
