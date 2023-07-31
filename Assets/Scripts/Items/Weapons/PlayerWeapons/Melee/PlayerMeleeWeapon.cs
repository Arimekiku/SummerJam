using UnityEngine;

public abstract class PlayerMeleeWeapon : PlayerWeapon
{
    [SerializeField] protected float distanceAttack;
    [SerializeField] protected float angleAttack;
    [SerializeField] protected AnimationCurve attackCurve;
}
