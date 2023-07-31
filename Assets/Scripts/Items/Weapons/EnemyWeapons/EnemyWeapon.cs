using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour
{
    [SerializeField] protected float rateOfAttackPerSec;
    [SerializeField] protected int damage;
    [SerializeField] protected AudioClip attackSound;

    public abstract void Attack(Vector2 targetPointAttack);
    public abstract void Initialize();
}