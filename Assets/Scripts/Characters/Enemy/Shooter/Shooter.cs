using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Shooter : Enemy
{
    [SerializeField] private BulletDetector bulletDetector;
    [SerializeField] private float coolDownEvasion;
    [SerializeField] private float distanceEvasion;
    [SerializeField] private float speedEvasion;
    
    private bool evasionOnCoolDown;
    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            Vector2 playerPosition = player.transform.position;
            RotateTowardsPlayer(playerPosition);
            weapon.Attack(playerPosition);
        }
    }

    public override void TakeDamage(int damage, Vector2 damageDirection)
    {
        base.TakeDamage(damage, damageDirection);

        StartCoroutine(DamageBoost());
        
        IEnumerator DamageBoost()
        {
            damageDirection = damageDirection.normalized;
            
            float time = 0.1f;

            while (time > 0)
            {
                Vector2 newPosition = (Vector2)transform.position + speedDamageBust * Time.fixedDeltaTime * damageDirection;
                transform.position = newPosition;
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
    }

    public void EvasionBullet(Vector2 direction)
    {
        if (evasionOnCoolDown)
            return;
        StartCoroutine(Dash());
        
        IEnumerator Dash()
        {
            evasionOnCoolDown = true;
            float timer = distanceEvasion / speedEvasion;

            while (timer > 0)
            {
                Vector2 newPosition = (Vector2)transform.position + speedEvasion * Time.fixedDeltaTime * direction;
                transform.position = newPosition;
                timer -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            StartCoroutine(CoolDownEvasion());
        }

        IEnumerator CoolDownEvasion()
        {
            float timer = coolDownEvasion;

            while (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
                yield return Time.fixedDeltaTime;
            }

            evasionOnCoolDown = false;
        }
    }

    public override void Activate()
    {
        base.Activate();
        bulletDetector.BulletDetectEvent += EvasionBullet;
    }

    protected override void Death()
    {
        base.Death();
        weapon.DestroyWeapon();
        Deactivate();
    }
}
