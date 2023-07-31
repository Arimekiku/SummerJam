using System.Collections;
using UnityEngine;

public class Shooter : Enemy
{
    [Space, SerializeField] private Weapon weapon;
    [SerializeField] private BulletDetector bulletDetector;

    private new ShooterData data;
    
    private bool evasionOnCoolDown;

    private void Awake()
    {
        data = base.data as ShooterData;
        
        weapon.Initialize();
    }

    private void FixedUpdate()
    {
        if (target && stunTimer <= 0)
        {
            Vector2 playerPosition = target.transform.position;
            RotateTowardsPlayer(playerPosition);
            weapon.Attack(playerPosition);
        }

        if (stunTimer > 0)
            stunTimer -= Time.fixedDeltaTime;
    }
    
    public override void Activate()
    {
        bulletDetector.BulletDetectEvent += EvasionBullet;
    }

    public override void Deactivate()
    { 
        bulletDetector.BulletDetectEvent -= EvasionBullet;
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
                Vector2 newPosition = (Vector2)transform.position + data.DamageBoostSpeed * Time.fixedDeltaTime * damageDirection;
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
            float timer = data.EvasionDistance / data.EvasionSpeed;

            while (timer > 0)
            {
                Vector2 newPosition = (Vector2)transform.position + data.EvasionSpeed * Time.fixedDeltaTime * direction;
                transform.position = newPosition;
                timer -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            StartCoroutine(CoolDownEvasion());
        }

        IEnumerator CoolDownEvasion()
        {
            float timer = data.EvasionCooldown;

            while (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
                yield return Time.fixedDeltaTime;
            }

            evasionOnCoolDown = false;
        }
    }
}
