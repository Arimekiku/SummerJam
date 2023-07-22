using System;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int currentHealth;
    
    public event Action OnDeathEvent;
    public int Health => currentHealth;

    public virtual void TakeDamage(int damage, Vector2 damageDirection)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
            Death();
    }
    
    protected virtual void Death()
    {
        OnDeathEvent?.Invoke();
    }

    public abstract void Activate();
    public abstract void Deactivate();
}
