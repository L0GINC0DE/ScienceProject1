using System;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthSystem : AbstractHealthSystem
{
    [SerializeField]private Slider _healthSlider;
    private void Start()
    {
        SetHealth(1000);
        SetHealth();
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
        // 프리팹 에셋의 Slider는 수정하지 않습니다. 실제 HUD가 연결될 때까지 기다립니다.
        if (_healthSlider == null || !_healthSlider.gameObject.scene.IsValid()) return;
        _healthSlider.maxValue = 1000;
        _healthSlider.value = Health;
    }

    public void BindHealthSlider(Slider slider)
    {
        if (slider == null || !slider.gameObject.scene.IsValid()) return;
        _healthSlider = slider;
        SetHealth();
    }
}
