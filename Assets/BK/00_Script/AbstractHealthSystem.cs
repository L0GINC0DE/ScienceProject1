using System;
using UnityEngine;

public abstract class AbstractHealthSystem : MonoBehaviour
{
    private int _health = 100;
    public int Health
    {
        get {return _health;}
        private set
        {
            _health = value;
            if (_health <= 0)
            {
                _health = 0;
                OnDead?.Invoke();
            }
        } 
    }

    public Action<int> OnDamaged;
    public Action<int> OnHeal;
    public Action OnDead;

    public virtual void OnEnable()
    {
        OnDamaged += Damage;
        
        OnHeal += Heal;

        OnDead += Dead;
    }

    public void SetHealth(int health)
    {
        _health = health;
    }

    public virtual void Damage(int damage)
    {
        Health -= damage;
    }

    public virtual void Heal(int heal)
    {
        Health += heal;
    }

    public virtual void Dead()
    {
        
    }
}
