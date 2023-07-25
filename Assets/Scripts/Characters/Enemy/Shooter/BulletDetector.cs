using System;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    public event Action<Vector2> BulletDetectEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
            return;

        if (other.TryGetComponent(out PlayerBullet playerBullet))
        {
            Vector2 bulletPosition = playerBullet.transform.position;

            Vector2 directionMove;
            
            float distance = Vector2.Distance(bulletPosition, transform.position + transform.right);
            float distance2 = Vector2.Distance(bulletPosition, transform.position - transform.right);
            
            if (distance > distance2)
                directionMove = transform.right;
            else
                directionMove = -transform.right;

            if (!WillBulletHit())
                return;
            
            bool WillBulletHit()
            {
                RaycastHit2D[] hitsInfo = Physics2D.RaycastAll(bulletPosition, playerBullet.FlightDirection);
                foreach (RaycastHit2D hit2D in hitsInfo)
                {
                    if (!hit2D.collider.isTrigger)
                        return false;
                    
                    if (!hit2D.collider.TryGetComponent(out Shooter enemy))
                        continue;

                    if (enemy != gameObject.GetComponentInParent<Shooter>()) 
                        continue;
                    
                    enemy.EvasionBullet(directionMove);
                    return true;
                }

                return false;
            }
        }
    }
}
