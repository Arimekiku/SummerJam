using UnityEngine;

public class TrapArrow : Ammunition
{
    [SerializeField] private int trapDamage;

    private void Awake()
    {
        damage = trapDamage;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            if (other.TryGetComponent(out Player player))
            {
                if (player.IgnoreDamage())
                    return;
                
                player.TakeDamage(damage, transform.up);
            }

            Destroy(gameObject);
        }
    }
}
