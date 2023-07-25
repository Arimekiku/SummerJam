using System;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int currentHealth;
    
    public event Action OnDeath;
    public event Action OnActivate;
    public event Action OnDeactivate;
    
    public int Health => currentHealth;

    public virtual void TakeDamage(int damage, Vector2 damageDirection)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
            Death();
    }
    
    protected virtual void Death()
    {
        OnDeath?.Invoke();
    }

    public virtual void Activate()
    {
        OnActivate?.Invoke();
    }

    public virtual void Deactivate()
    {
        OnDeactivate?.Invoke();
    }
}
