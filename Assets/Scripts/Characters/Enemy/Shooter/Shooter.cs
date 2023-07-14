public class Shooter : Enemy
{
    private void Awake()
    {
        OnDeathEvent += () => weapon.CastOut(transform.position); 
    }

    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            RotateTowardsPlayer(player.transform.position);
            weapon.Attack(player.transform.position);
        }
    }
}
