using System;
using UnityEngine;

public class Shield : PlayerRangedWeapon
{
	[SerializeField] private int hp;
	[SerializeField] private float angle;
	[SerializeField] private int countBullet;
    	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.TryGetComponent(out BotBullet botBullet))
			return;
		RaycastHit2D[] hitsInfo = Physics2D.RaycastAll(botBullet.transform.position, botBullet.FlightDirection);
		botBullet.gameObject.SetActive(false);
		TakeDamage(1);
		foreach (RaycastHit2D hit2D in hitsInfo)
		{
			if (!hit2D.collider.GetComponent<Shield>())
				continue;
			Vector2 normalVector = hit2D.normal;
			Vector2 newDirection = Vector2.Reflect(botBullet.FlightDirection, normalVector);
			poolAmmo.GetFreeElement(out Ammunition ammunition);
			ammunition.SetDirectionAndStart(newDirection, hit2D.point);
			return;
		}
	}

	public override void Attack(Vector2 targetPointPosition)
	{
		if (isReloading) 
			return;

		if (currentAmmo <= 0) 
			return;

		UpdateUI();

		int limit;

		if (countBullet > currentAmmo)
		{
			limit = (int)Math.Ceiling((double)currentAmmo / 2);
			currentAmmo = 0;
		}
		else
		{
			limit = (int)Math.Ceiling((double)countBullet / 2);
			currentAmmo -= countBullet;
		}

		Vector2 directionAttack = targetPointPosition - AttackPointPosition;
		
		for (int i = 0; i < limit; i++)
		{
			if (i == 0)
			{
				poolAmmo.GetFreeElement(out Ammunition bullet);
				bullet.SetDirectionAndStart(directionAttack, AttackPointPosition);
				continue;
			}
			
			CreateBullet(limit - i);
			CreateBullet(i - limit);
		}
		AudioHandler.PlaySound(attackSound);

		StartCoroutine(ReloadTimer());
		
		void CreateBullet(int i)
		{
			poolAmmo.GetFreeElement(out Ammunition ammunition);
			Vector2 directionVector = (Quaternion.Euler(0,0, angle / limit * i) * (directionAttack) ).normalized;
			ammunition.SetDirectionAndStart(directionVector, AttackPointPosition);
		}
	}

	private void TakeDamage(int takeDamage)
	{
		hp -= takeDamage;
		if (hp <= 0)
		{
			Destroy(gameObject);
		}
	}
	
	protected override void UpdateUI()
	{
	}
}
