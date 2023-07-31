using System;
using UnityEngine;


public abstract class Character : MonoBehaviour
{
    public event Action OnDeath;
    
    protected int currentHealth;

    public virtual void TakeDamage(int damage, Vector2 damageDirection)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
            Death();
    }
    
    protected virtual void Death()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public abstract void Activate();

    public abstract void Deactivate();
}
