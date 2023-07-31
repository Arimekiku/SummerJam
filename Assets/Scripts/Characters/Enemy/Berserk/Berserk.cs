using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody2D))]
public class Berserk : Enemy
{
    [Space, SerializeField] private List<BerserkAxe> axes = new List<BerserkAxe>();

    private new BerserkData data;
    private Rigidbody2D rb;
    private bool isReady = true;
    private bool onTheJump;

    private void Awake()
    {
        data = base.data as BerserkData;
        
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        
        foreach (BerserkAxe berserkAxe in axes)
            berserkAxe.Initialize();
    }

    private void FixedUpdate()
    {
        if (target && stunTimer <= 0)
        {
            if (!onTheJump)
                RotateTowardsPlayer(target.transform.position);

            if (isReady)
                StartCoroutine(PreparationForAttack(target));
        }

        if (stunTimer > 0)
            stunTimer -= Time.fixedDeltaTime;
    }

    private IEnumerator PreparationForAttack(Player player)
    {
        Vector2 finishPosition = player.transform.position;
        int randValue = Random.Range(0, 100);
        isReady = false;
        
        if (randValue < 70)
        {
            yield return StartCoroutine(Preparation(data.JumpPrepareTime));
            StartCoroutine(Jump(finishPosition));
        }
        else
        {
            yield return StartCoroutine(Preparation(data.ThrowPrepareTime));
            StartCoroutine(ThrowAxe(finishPosition));
        }

        IEnumerator Preparation(float prepTime)
        {
            float time = prepTime;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            
            finishPosition = player.transform.position;
        }
    }

    private IEnumerator Jump(Vector2 finishPosition)
    {
        onTheJump = true;

        Vector2 directionJump = finishPosition - rb.position;

        float distance = Vector2.Distance(finishPosition, transform.position);
        float time = distance / data.MoveSpeed;

        directionJump = directionJump.normalized;

        foreach (BerserkAxe berserkAxe in axes)
            berserkAxe.RotateAttack(time);

        while (time > 0)
        {
            Vector2 newPosition = rb.position + data.MoveSpeed * Time.fixedDeltaTime * directionJump;
            rb.MovePosition(newPosition);

            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        onTheJump = false;
        StartCoroutine(CoolDownAttack(data.JumpCooldownTime));
    }

    private IEnumerator ThrowAxe(Vector2 targetPosition)
    {
        float time = 0;
        foreach (BerserkAxe berserkAxe in axes)
        {
            berserkAxe.Attack(targetPosition);
            time = (targetPosition - (Vector2)transform.position).magnitude / berserkAxe.SpeedFlying * 2f;
        }

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        StartCoroutine(CoolDownAttack(data.ThrowCooldownTime));
    }
    
    private IEnumerator CoolDownAttack(float timeCoolDown)
    {
        float time = timeCoolDown;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        isReady = true;
    }
}
