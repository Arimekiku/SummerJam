using UnityEngine;

public class BotBullet : Ammunition
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                if (player.IgnoreDamage())
                    return;
                player.TakeDamage(damage, flightDirection);
            }
            
            gameObject.SetActive(false);
        }
        
        if (other.TryGetComponent(out PlayerShield _))
        {
            RaycastHit2D[] hitsInfo = Physics2D.RaycastAll(transform.position, flightDirection);
            
            foreach (RaycastHit2D raycastHit2D in hitsInfo)
            {
                if (!raycastHit2D.collider.GetComponent<PlayerShield>()) 
                    continue;
                
                Vector2 normalVector = raycastHit2D.normal;
                Vector2 newDirection = Vector2.Reflect(flightDirection, normalVector);
                flightDirection = newDirection;
                transform.rotation = Quaternion.LookRotation(newDirection);
                return;
            }
        }
    }
}
