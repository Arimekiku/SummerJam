using UnityEngine;

public class Bat : PlayerMeleeWeapon
{
    public override void Attack(Vector2 pointOfAttack)
    {
        Vector2 castedTransform = transform.position;
        Vector2 directionAttack = pointOfAttack - castedTransform;

        Collider2D[] hitInfos = Physics2D.OverlapCircleAll(castedTransform, distanceAttack);

        foreach (Collider2D hitInfo in hitInfos)
        {
            if (hitInfo.transform.parent)
            {
                if (hitInfo.transform.parent.TryGetComponent(out Enemy enemy))
                {
                    Vector2 directionToEnemy = enemy.transform.position - transform.position;
                    if (Vector2.Angle(directionToEnemy, directionAttack) <= angleAttack / 2)
                        enemy.TakeDamage(damage, (Vector2)hitInfo.transform.position - castedTransform);
                }
            }
        }
    }

    protected override void UpdateUI()
    {
        throw new System.NotImplementedException();
    }

    public override void Initialize(Player player)
    {
        throw new System.NotImplementedException();
    }
}
