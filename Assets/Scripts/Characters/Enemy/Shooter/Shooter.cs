using System.Collections;
using UnityEngine;

public class Shooter : Enemy
{
    [SerializeField] private BotWeapon weapon;
    [SerializeField] private BulletDetector bulletDetector;

    private new ShooterData data;
    
    private bool evasionOnCoolDown;

    private void Awake()
    {
        data = base.data as ShooterData;
    }

    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player) && stunTimer <= 0)
        {
            Vector2 playerPosition = player.transform.position;
            RotateTowardsPlayer(playerPosition);
            weapon.Attack(playerPosition);
        }

        if (stunTimer > 0)
            stunTimer -= Time.fixedDeltaTime;
    }
    
    public override void Activate()
    {
        weapon.InitializedWeapon();
        
        bulletDetector.BulletDetectEvent += EvasionBullet;
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

    protected override void Death()
    {
        base.Death();
        weapon.DestroyWeapon();
        Deactivate();
    }
}
