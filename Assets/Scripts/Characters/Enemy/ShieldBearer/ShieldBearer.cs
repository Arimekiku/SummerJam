using UnityEngine;

public class ShieldBearer : Enemy
{
    [SerializeField] private BotShield weapon;
    
    private new ShieldBearerData data;
    
    private float angle;

    private void Awake()
    {
        data = base.data as ShieldBearerData;
    }

    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player) && stunTimer <= 0)
        {
            Vector2 playerPosition = player.transform.position;
            RotateTowardsPlayer(playerPosition);
            weapon.Attack(transform.up);
        }

        if (stunTimer > 0)
            stunTimer -= Time.fixedDeltaTime;
    }

    protected override void RotateTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 castPosition = transform.position;
        Vector2 directionRotate = (playerPosition - castPosition).normalized;
        angle = Mathf.Atan2(directionRotate.y, directionRotate.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), data.RotationSpeed * Time.fixedDeltaTime);
    }

    protected override void Death()
    {
        base.Death();
        Deactivate();
    }
}
