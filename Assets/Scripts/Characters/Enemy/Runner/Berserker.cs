using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Berserker : Enemy
{
    [SerializeField] private float timeToPreparation;
    [SerializeField] private float jumpCoolDownTime;

    public event Action<bool> OnJumpEvent;

    private Rigidbody2D rb;
    private bool isReady = true;
    private bool onTheJump;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            if (!onTheJump)
                RotateTowardsPlayer(player.transform.position);

            if (isReady)
                StartCoroutine(PreparetionForAttack(player));
        }
    }

    private IEnumerator PreparetionForAttack(Player player)
    {
        isReady = false;
        float time = timeToPreparation;

        while (time > 0)
        {
            RotateTowardsPlayer(player.transform.position);
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        Vector2 finishPosition = player.transform.position;
        StartCoroutine(Jump(finishPosition));
    }

    private IEnumerator Jump(Vector2 finishPosition)
    {
        onTheJump = true;
        OnJumpEvent?.Invoke(true);

        Vector2 directionJump = (finishPosition - rb.position).normalized;

        float distance = Vector2.Distance(finishPosition, transform.position);

        while (distance > 1f)
        {
            Vector2 newPosition = rb.position + speed * Time.fixedDeltaTime * directionJump;
            rb.MovePosition(newPosition);

            distance = Vector2.Distance(finishPosition, transform.position);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        onTheJump = false;
        OnJumpEvent?.Invoke(false);
        StartCoroutine(CoolDownJump());
    }

    private IEnumerator CoolDownJump()
    {
        float time = jumpCoolDownTime;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        isReady = true;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
