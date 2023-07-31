using System;
using UnityEngine;

public class Arrow : Ammunition
{
    private bool isStuck;

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
                Drop();
                isStuck = false;
                gameObject.SetActive(false);
                return;
            }
            
            Stuck();
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            enemy.OnDeath += Drop;
            transform.SetParent(enemy.transform, true);
            enemy.TakeDamage(damage, FlightDirection);
            Stuck();
            return;
        }

        EnemyShield enemyShield = other.GetComponentInParent<EnemyShield>();
        if (enemyShield)
        {
            enemyShield.GetComponentInParent<Enemy>().OnDeath += Drop;
            transform.SetParent(enemyShield.transform, true);
            Stuck();
        }

        void Stuck()
        {
            rb.bodyType = RigidbodyType2D.Static;
            FlightDirection = Vector3.zero;
            isStuck = true;
        }
    }

    private void Drop()
    {
        transform.SetParent(null, true);
    }
}

