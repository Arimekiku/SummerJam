using Cinemachine;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private CinemachineImpulseSource shake;
    [SerializeField] private Ammo enemyAmmo;
    [SerializeField] protected EnemyData data;

    protected float stunTimer;
    
    protected virtual void Start()
    {
        currentHealth = data.MaxHealth;
        
        Activate();
        shake = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Vector2 directionDamageBoost = player.transform.position - transform.position;
            player.TakeDamage(1, directionDamageBoost);
        }
    }

    protected bool CheckPlayer(out Player player)
    {
        player = null;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, data.DetectionRange);

        foreach (Collider2D collide in colliders)
        {
            if (collide.isTrigger)
                continue;
            
            if (collide.TryGetComponent(out player))
                break;
        }

        if (!player)
            return false;
        
        Vector2 directionPlayer = player.transform.position - transform.position;
        
        RaycastHit2D[] hitsInfo = Physics2D.RaycastAll(transform.position, directionPlayer, data.DetectionRange);

        foreach (RaycastHit2D hitInfo in hitsInfo)
        {
            if (hitInfo.collider.isTrigger) 
                continue;

            return hitInfo.collider.TryGetComponent(out player);
        }
        
        player = null;
        return false;
    }

    protected virtual void RotateTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 castPosition = transform.position;
        Vector2 directionRotate = playerPosition - castPosition;
        float angle = Mathf.Atan2(directionRotate.y, directionRotate.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    public override void Deactivate()
    {
        Destroy(gameObject);
    }

    public void Stun()
    {
        stunTimer = 1f;
    }
    
    protected override void Death()
    {
        data.HandleDeath(transform);

        Vector3 randomVector = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1) * 0.4f;
        shake.GenerateImpulseWithVelocity(randomVector);

        if (this is not Berserk)
            if (Random.value < 0.2f)
                Instantiate(enemyAmmo, transform.position, transform.rotation);

        base.Death();
    }
}
