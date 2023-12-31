using UnityEngine;

public class PlayerBullet : Ammunition
{
	protected override void OnTriggerEnter2D(Collider2D other)
	{
		other.TryGetComponent(out Character character);

		if (!other.isTrigger)
		{
			if (character is Player)
				return;
			
			gameObject.SetActive(false);
			
			if (other.TryGetComponent(out DestroyablePiece _))
				return;
			
			AudioHandler.PlaySound(impactSounds);
		}
		
		if (character is Enemy)
		{
			gameObject.SetActive(false);
			character.TakeDamage(damage, FlightDirection);
		}
	}
}