using UnityEngine;

public class Enemy : Character
{
    [SerializeField] protected LayerMask noIgnoreLayer;
    [SerializeField] protected BotWeapon weapon;
    [SerializeField] protected float rangeDetected;
    [SerializeField] protected LayerMask playerLayer;

    private bool isInitialized;

    protected virtual void Start()
    {
        weapon.TakeUp(transform);
    }

    protected virtual bool CheckPlayer(out Player player)
    {
        if (!isInitialized) 
        {
            player = null;
            return false;
        }

        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, rangeDetected, playerLayer);

        if (playerCollider) 
        {
            if (playerCollider.TryGetComponent(out player))
            {
                Vector2 directionPlayer = player.transform.position - transform.position;

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, directionPlayer, rangeDetected, noIgnoreLayer);
                if (hitInfo.collider)
                    if (((1 << hitInfo.collider.gameObject.layer) & playerLayer) != 0)
                        return true;
            }
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

    public void Initialize() => isInitialized = true;
    public void Deinitialize() => isInitialized = false;
}
