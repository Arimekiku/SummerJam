using UnityEngine;

public class ShieldBearer : Enemy
{
    [SerializeField] private float speedRotate;

    private float angle;

    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            RotateTowardsPlayer(player.transform.position);
            weapon.Attack(player.transform.position);
        }
    }

    protected override void RotateTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 castPosition = transform.position;
        Vector2 directionRotate = (playerPosition - castPosition).normalized;
        angle = Mathf.Atan2(directionRotate.y, directionRotate.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), speedRotate * Time.fixedDeltaTime);
    }
}
