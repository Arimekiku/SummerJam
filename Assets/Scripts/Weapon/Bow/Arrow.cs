using System;
using UnityEngine;

public class Arrow : Ammunition
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask shieldLayer;

    private bool isStuck = false;

    public event Action LiftOnTheFloorEvent;

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Ammunition>())
            return;

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            if (!isStuck) return;
            rb.bodyType = RigidbodyType2D.Dynamic;
            transform.rotation = Quaternion.identity;
            LiftOnTheFloorEvent?.Invoke();
            transform.SetParent(null, false);
            isStuck = false;
            gameObject.SetActive(false);
            return;
        }

        if (((1 << other.gameObject.layer) & damageLayer) != 0)
        {
            Character enemy = other.gameObject.GetComponentInParent<Character>();
            enemy.OnDeathEvent += DropTheFloor;
            transform.SetParent(enemy.transform, true);
            enemy.TakeDamage(damage);
        }

        if (((1 << other.gameObject.layer) & shieldLayer) != 0)
        {
            Character enemy = other.gameObject.GetComponentInParent<Character>();
            enemy.OnDeathEvent += DropTheFloor;
            transform.SetParent(enemy.transform, true);
        }

        rb.bodyType = RigidbodyType2D.Static;
        flightDirection = Vector3.zero;
        isStuck = true;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!isStuck)
            base.OnTriggerEnter2D(other);
    }

    private void DropTheFloor()
    {
        transform.SetParent(null, true);
        isStuck = false;
    }
}

