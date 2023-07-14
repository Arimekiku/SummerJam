using System.Collections;
using UnityEngine;

public class Melee : Weapon
{
    [SerializeField] private int damage;
    [SerializeField] private float distanceAttack;
    [SerializeField] private float angleAttack;
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private Transform handTransform;

    private SpriteRenderer currentRenderer;

    private void Awake()
    {
        currentRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override bool Attack(Vector2 pointOfAttack)
    {
        StartCoroutine(AttackAnimation());

        Vector2 castedTransform = transform.position;
        Vector2 directionAttack = pointOfAttack - castedTransform;

        Collider2D[] hitInfos = Physics2D.OverlapCircleAll(transform.position, distanceAttack);

        foreach (Collider2D hitInfo in hitInfos)
        {
            if (hitInfo.transform.parent)
            {
                if (hitInfo.transform.parent.TryGetComponent(out Enemy enemy))
                {
                    Vector2 directionToEnemy = enemy.transform.position - transform.position;
                    if (Vector2.Angle(directionToEnemy, directionAttack) <= angleAttack / 2)
                        enemy.TakeDamage(damage);
                }
            }
        }

        return false;
    }

    private IEnumerator AttackAnimation()
    {
        currentRenderer.enabled = true;
        yield return RotateTowards(new Vector3(0, 0, angleAttack));

        yield return RotateTowards(Vector3.zero);
        currentRenderer.enabled = false;

        IEnumerator RotateTowards(Vector3 targetRotation)
        {
            Quaternion startRotation = handTransform.localRotation;
            float currentTime = 0f, targetTime = 1f;

            while (currentTime != targetTime)
            {
                currentTime = Mathf.MoveTowards(currentTime, targetTime, Time.deltaTime / 0.25f);

                handTransform.localRotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetRotation), attackCurve.Evaluate(currentTime));

                yield return new WaitForEndOfFrame();
            }
        }
    }

    protected override void UpdateUI()
    {
        throw new System.NotImplementedException();
    }
}
