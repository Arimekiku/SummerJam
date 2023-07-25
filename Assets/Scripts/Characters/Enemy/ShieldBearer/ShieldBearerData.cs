using UnityEngine;


[CreateAssetMenu(fileName = "ShieldBearer", menuName = "Characters/Shield Bearer")]
public class ShieldBearerData : EnemyData
{
    [Space, SerializeField] private float rotationSpeed;

    public float RotationSpeed => rotationSpeed;
}