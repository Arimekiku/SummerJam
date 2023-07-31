using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Ammunition : PoolableObject
{
	[SerializeField] private float speed = 6;
	[SerializeField] protected AudioClip[] impactSounds;
	
	protected int damage;

	protected Rigidbody2D rb;
	public Vector2 FlightDirection { get; protected set; }

	protected void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (FlightDirection != Vector2.zero)
			Flight();
	}
	
	protected abstract void OnTriggerEnter2D(Collider2D other);

	private void Flight()
	{
		Vector2 newPosition = (Vector2)transform.position + speed * Time.fixedDeltaTime * FlightDirection;
		rb.MovePosition(newPosition);
	}

	public void SetDirectionAndStart(Vector2 direction, Vector2 startPosition)
	{
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		transform.SetPositionAndRotation(startPosition, Quaternion.Euler(0f, 0f, angle - 90f));

		FlightDirection = direction.normalized;
		gameObject.SetActive(true);
	}

	public void SetDamage(int newDamage)
	{
		if (newDamage < 0)
			throw new ArgumentException("Damage cannot be null");
		
		damage = newDamage;
	}
}