using UnityEngine;

public class BotBullet : Ammunition
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            Player player = other.GetComponent<Player>();
            
            if (player)
            {
                if (player.IgnoreDamage())
                    return;
                
                player.TakeDamage(damage, FlightDirection);
            }
            
            gameObject.SetActive(false);
            
            if (other.TryGetComponent(out DestroyablePiece _))
                return;
            
            if (!player)
                AudioHandler.PlaySound(impactSounds);
        }
    }
    
}
