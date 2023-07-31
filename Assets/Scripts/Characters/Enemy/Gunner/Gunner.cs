using System.Collections;
using UnityEngine;

public class Gunner : Enemy
{
    [Space, SerializeField] private MachineGun weapon;

    private new GunnerData data;

    private void Awake()
    {
        data = base.data as GunnerData;
        
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
}
