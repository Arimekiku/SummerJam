using UnityEngine;


[CreateAssetMenu(fileName = "Berserk", menuName = "Characters/Berserk")]
public class BerserkData : EnemyData
{
    [Space, SerializeField] private float moveSpeed;
    [SerializeField] private float timeToPreparationJump;
    [SerializeField] private float timeToPreparationThrow;
    [SerializeField] private float jumpCooldownTime;
    [SerializeField] private float throwCooldownTime;

    public float MoveSpeed => moveSpeed;
    public float JumpPrepareTime => timeToPreparationJump;
    public float ThrowPrepareTime => timeToPreparationThrow;
    public float JumpCooldownTime => jumpCooldownTime;
    public float ThrowCooldownTime => throwCooldownTime;
}