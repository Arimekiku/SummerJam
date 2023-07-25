using System;
using UnityEngine;

public class Arrow : Ammunition
{
    private bool isStuck;

    public event Action LiftOnTheFloorEvent;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                if (!isStuck) 
                    return;
                rb.bodyType = RigidbodyType2D.Dynamic;
                transform.rotation = Quaternion.identity;
                LiftOnTheFloorEvent?.Invoke();
                transform.SetParent(null, true);
                isStuck = false;
                gameObject.SetActive(false);
                return;
            }
            Stuck();
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            enemy.OnDeath += DropTheFloor;
            transform.SetParent(enemy.transform, true);
            enemy.TakeDamage(damage, FlightDirection);
            Stuck();
            return;
        }

        BotShield botShield = other.GetComponentInParent<BotShield>();
        if (botShield)
        {
            botShield.GetComponentInParent<Enemy>().OnDeath += DropTheFloor;
            transform.SetParent(botShield.transform, true);
            Stuck();
            return;
        }

        void Stuck()
        {
            rb.bodyType = RigidbodyType2D.Static;
            FlightDirection = Vector3.zero;
            isStuck = true;
        }
    }

    private void DropTheFloor()
    {
        transform.SetParent(null, true);
    }
}

