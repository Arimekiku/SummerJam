using System.Collections;
using UnityEngine;

public abstract class PlayerMeleeWeapon : PlayerWeapon
{
    [SerializeField] protected float distanceAttack;
    [SerializeField] protected float angleAttack;
    [SerializeField] protected AnimationCurve attackCurve;
    [SerializeField] protected Transform handTransform;

    protected SpriteRenderer currentRenderer;

    private void Awake()
    {
        currentRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected abstract IEnumerator AttackAnimation();
}
