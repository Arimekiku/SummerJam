using System.Collections;
using UnityEngine;

public abstract class PlayerWeapon : ItemBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite paintedSprite;
    [SerializeField] private float speedThrow;
    [SerializeField] private float speedThrowRotation;
    [SerializeField] private AudioClip throwClip;

	[SerializeField] protected float rateOfAttackPerSec;
	[SerializeField] protected int damage;
	[SerializeField] protected AudioClip attackSound;

	public abstract void Attack(Vector2 targetPointAttack);
    
    private bool throwWeapon;
    protected bool isReloading;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!throwWeapon)
            return;

        other.TryGetComponent(out Character character);
        
        if (!other.isTrigger)
        {
            if (character is Player)
                return;
            
            StopAllCoroutines();
            Destroy(gameObject);
        }
        
        if (character is Enemy)
        {
            Vector2 directionDamageBoost = other.transform.position - transform.position;
            character.TakeDamage(damage * 2, directionDamageBoost);
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    protected void Equip(Player holder)
    {
        PlayerWeapon weaponToSpawn = Instantiate(this, transform.position, Quaternion.identity);

        weaponToSpawn.transform.SetParent(holder.RangedContainer, false);
        weaponToSpawn.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
    }
    
    public void Drop(Vector2 playerPosition)
    {
        transform.SetParent(null, true);
        transform.position = playerPosition;
    }

    protected IEnumerator ReloadTimer()
    {
        isReloading = true;
        float time = 1 / rateOfAttackPerSec;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        isReloading = false;
    }
    
    public virtual void ThrowWeapon(Vector2 targetPointPosition)
    {
        transform.SetParent(null, true);
        throwWeapon = true;

        Vector2 directionThrow = (targetPointPosition - (Vector2)transform.position).normalized;

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        StartCoroutine(ThrowWeaponRoutine());
        AudioHandler.PlaySound(throwClip);
        
        IEnumerator ThrowWeaponRoutine()
        {
            while (true)
            {
                Vector2 newPosition = (Vector2)transform.position + speedThrow * Time.fixedDeltaTime * directionThrow;
                rb.MovePosition(newPosition);
                transform.eulerAngles = new Vector3(0,0,transform.eulerAngles.z - speedThrowRotation * Time.fixedDeltaTime);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
    }

    protected abstract void UpdateUI();
}
