using System;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    [SerializeField] private CinemachineImpulseSource shake;
    [SerializeField] private Ammo enemyAmmo;
    [SerializeField] protected EnemyData data;

    protected float stunTimer;
    protected Player target;

    private Vector2 startPosition;

    protected virtual void Start()
    {
        currentHealth = data.MaxHealth;
        startPosition = transform.position;
        
        shake = GetComponent<CinemachineImpulseSource>();
    }

    protected void Update()
    {
        if (target is null)
            transform.Translate((startPosition - (Vector2)transform.position).normalized * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Vector2 directionDamageBoost = player.transform.position - transform.position;
            target.TakeDamage(1, directionDamageBoost);
        }
    }
    
    public override void Activate()
    {
        target = FindObjectOfType<Player>();
    }

    public override void Deactivate()
    {
        target = null;
    }

    protected virtual void RotateTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 castPosition = transform.position;
        Vector2 directionRotate = playerPosition - castPosition;
        float angle = Mathf.Atan2(directionRotate.y, directionRotate.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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
