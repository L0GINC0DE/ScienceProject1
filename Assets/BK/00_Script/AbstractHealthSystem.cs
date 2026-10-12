using System;
using UnityEngine;

public abstract class AbstractHealthSystem:MonoBehaviour
{
    private int health = 100;
    public int Health
    {
        get {return health;}
        private set
        {
            health = value;
            if (health <= 0)
            {
                health = 0;
            }
        } 
    }
    public Action<int>OnDamaged;
    public Action<int> OnHeal;

    private void OnEnable()
    {
        OnDamaged += Damage;
        
        OnHeal += Heal;
    }

    public virtual void Damage(int damage)
    {
        Health -= damage;
    }

    public virtual void Heal(int heal)
    {
        Health += heal;
    }
}
