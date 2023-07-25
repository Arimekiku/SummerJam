using UnityEngine;


[CreateAssetMenu(fileName = "Shooter", menuName = "Characters/Shooter")]
public class ShooterData : EnemyData
{
    [Space, SerializeField] private float evasionCooldown;
    [SerializeField] private float evasionDistance;
    [SerializeField] private float evasionSpeed;

    public float EvasionCooldown => evasionCooldown;
    public float EvasionDistance => evasionDistance;
    public float EvasionSpeed => evasionSpeed;
}