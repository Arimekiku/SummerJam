using System.Collections;
using UnityEngine;

public abstract class PlayerWeapon : Weapon, IPickupable
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite paintedSprite;
    [SerializeField] private float speedThrow;
    [SerializeField] private float speedThrowRotation;

    private bool throwWeapon;

    protected bool isReloading;

    public bool Pickupable { get; private set; } = true;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void TakeUp(Transform weaponContainer)
    {
        transform.SetParent(weaponContainer, false);
        transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        
        Pickupable = false; 
    }

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
    
    public virtual void CastOut(Vector2 playerPosition)
    {
        transform.SetParent(null, true);
        transform.position = playerPosition;
        
        Pickupable = true;
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
    
    public void PickUp(Player player)
    {
        TakeUp(player.transform);
    }
    
    protected abstract void UpdateUI();
}