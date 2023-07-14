using System;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected float speed;

    public event Action OnDeathEvent;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) 
            OnDeathEvent?.Invoke();
    }
}
