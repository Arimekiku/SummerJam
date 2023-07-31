using UnityEngine;

public class EnemyPistol : EnemyRangeWeapon
{
	public override void Attack(Vector2 targetPointPosition)
	{
		if (currentAmmo <= 0)
		{
			if(!onReload)
				Reload();
			
			return;
		}
		
		currentAmmo -= 1;
		poolObject.GetFreeElement(out BotBullet botBullet);
		Vector2 directionVector = (targetPointPosition - FirePointPosition).normalized;
		botBullet.SetDirectionAndStart(directionVector, FirePointPosition);
		AudioHandler.PlaySound(attackSound);
	}
}
