using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	[SerializeField] protected float rateOfAttackPerSec;
	[SerializeField] protected int damage;
	[SerializeField] protected AudioClip releaseSound;

	public abstract void Initialize();
	
	public abstract void Attack(Vector2 targetPointAttack); 
}
