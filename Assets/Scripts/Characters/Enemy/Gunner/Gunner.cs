using System.Collections;
using UnityEngine;

public class Gunner : Enemy
{
    [SerializeField] private float delayBetweenShots;
    [SerializeField] private int numberOfShots;

    private bool isDelay = false;
    private int countShots;

    protected override void Start()
    {
        base.Start();
        countShots = numberOfShots;
    }

    void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            RotateTowardsPlayer(player.transform.position);
            if (!isDelay)
                AttackQueue(player);
        }
    }

    private void AttackQueue(Player player)
    {
        if (weapon.Attack(player.transform.position))
            countShots--;

        if (countShots <= 0)
        {
            isDelay = true;
            StartCoroutine(ResetQueue());
        }
    }

    private IEnumerator ResetQueue()
    {
        float time = delayBetweenShots;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        isDelay = false;
        countShots = numberOfShots;
    }

    protected override bool CheckPlayer(out Player player)
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, rangeDetected, playerLayer);

        if (playerCollider)
            if (playerCollider.TryGetComponent(out player)) 
                return true;

        player = null;
        return false;
    }
}
