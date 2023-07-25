using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] private Transform rangedWeaponContainer;
    [SerializeField] private Transform meleeWeaponContainer;

    public event Action OnWeaponPickup;
    public event Action OnReloadRequired;
    public Vector2 CurrentCheckPointPosition { get; private set; }
    
    public SpriteRenderer Rend { get; private set; }

    private PlayerMeleeWeapon currentMeleeWeapon;
    private PlayerRangedWeapon currentRangedWeapon;

    private readonly Dictionary<Type, int> ammoStack = new Dictionary<Type, int>();
    private CinemachineImpulseSource shake;

    public bool IsDashing { get; private set; }
    private bool invulnerable;
    private bool canAct;
    private bool canDash;

    private PlayerUI uiHandler;
    private PlayerData data;
    private Rigidbody2D body;
    
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        Rend = GetComponent<SpriteRenderer>();
        shake = GetComponent<CinemachineImpulseSource>();
        uiHandler = GetComponent<PlayerUI>();

        data = Resources.Load("Prefabs/Player/Data", typeof(PlayerData)) as PlayerData;
    }

    private void Start()
    {
        currentHealth = data.MaxHealth;
        uiHandler.UpdateHealth(currentHealth);
    }

    private void Update()
    {
        if (!canAct) 
            return;
        
        RotateTowardsMouse();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            Dash();

        if (Input.GetKeyDown(KeyCode.Mouse1)) 
            if (currentMeleeWeapon)
                currentMeleeWeapon.Attack(data.CursorPosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            if (currentRangedWeapon)
                currentRangedWeapon.Attack(data.CursorPosition);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentRangedWeapon)
            {
                if (currentRangedWeapon is not Bow)
                {
                    currentRangedWeapon.ThrowWeapon(data.CursorPosition);
                    uiHandler.ClearWeapon();
                    currentRangedWeapon = null;
                }
            }

            if (currentMeleeWeapon)
            {
                currentMeleeWeapon.ThrowWeapon(data.CursorPosition);
                currentRangedWeapon = null;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            IPickupable item = DetectPickup();
            
            PickUpItem(item);
        }

        if (Input.GetKeyDown(KeyCode.R))
            if(currentRangedWeapon)
                Reload(currentRangedWeapon);
    }

    private void FixedUpdate()
    {
        if (!IsDashing && canAct)
            Move();
    }

    private void RotateTowardsMouse()
    {
        Vector2 lookDirection = data.CursorPosition - body.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void Move()
    {
        Vector2 moveVector = PlayerData.GetInputVector();
        Vector2 newPosition = body.position + data.MoveSpeed * Time.fixedDeltaTime * moveVector;
        body.MovePosition(newPosition);
    }

    private void Dash()
    {
        if (!canDash)
            return;
        
        Vector2 directionDash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (directionDash == Vector2.zero)
            directionDash = transform.up;

        IsDashing = true;
        canDash = false;
        
        StartCoroutine(CoolDownDash());
        StartCoroutine(DashMoving(directionDash));
        
        IEnumerator CoolDownDash()
        {
            float time = data.DashCooldown;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            canDash = true;
        }
        
        IEnumerator DashMoving(Vector2 dirDash)
        {
            float time = data.DashDuration;
            float speed = data.DashSpeed;
            
            while (time > 0.1)
            {
                Vector2 newPosition = body.position + speed * Time.fixedDeltaTime * dirDash;
                Vector2 newDirection = Vector2.zero;

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, dirDash, 0.8f);

                if (!hitInfo.collider.isTrigger)
                {
                    if (Math.Abs(Vector2.Dot(dirDash, hitInfo.normal)) >= 0.98f)
                        break;

                    newDirection = SlideCollision(dirDash, hitInfo) * (speed * time / data.DashDuration);
                }

                if (newDirection != Vector2.zero)
                    newPosition = body.position + newDirection * Time.fixedDeltaTime;

                body.MovePosition(newPosition);
                time -= Time.fixedDeltaTime;

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            IsDashing = false;
            
            Vector3 SlideCollision(Vector2 direction, RaycastHit2D hitInfo) => Vector3.ProjectOnPlane(direction, hitInfo.normal);
        }
    }

    private IPickupable DetectPickup()
    {
        Collider2D[] hitsInfo = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        if (hitsInfo.Length == 0)
            return null;

        foreach (Collider2D hitInfo in hitsInfo)
        {
            if (hitInfo.TryGetComponent(out IPickupable pickup))
            {
                if (pickup.Pickupable)
                {
                    pickup.PickUp(this);
                    return pickup;
                }

                if (hitInfo.TryGetComponent(out Weapon weapon))
                    weapon.transform.parent = rangedWeaponContainer;
            }

            if (hitInfo.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.Type == InteractType.Button)
                    interactable.Interact();
                
                return null;
            }
        }

        return null;
    }

    private void PickUpItem<T>(T item) where T : IPickupable
    {
        Vector2 playerPosition = transform.position;
        
        switch (item)
        {
            case PlayerMeleeWeapon weapon:
            {
                if (currentMeleeWeapon)
                    currentMeleeWeapon.CastOut(playerPosition);

                currentMeleeWeapon = weapon;
                return;
            }
            case PlayerRangedWeapon weapon:
            {
                OnWeaponPickup?.Invoke();
                
                if (currentRangedWeapon)
                {
                    currentRangedWeapon.CastOut(playerPosition);

                    uiHandler.ClearWeapon();
                }
                
                currentRangedWeapon = weapon;
                uiHandler.UpdateWeapon(currentRangedWeapon, ammoStack);

                return;
            }
        }
    }
    
    public override void TakeDamage(int damage, Vector2 damageDirection)
    {
        if (IgnoreDamage()) 
            return;
        
        base.TakeDamage(damage, damageDirection);
        shake.GenerateImpulseWithForce(1f);
        
        uiHandler.UpdateHealth(currentHealth);

        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (Collider2D coll in collidersInRange)
        {
            if (coll.TryGetComponent(out BotBullet _))
            {
                coll.gameObject.SetActive(false);
                continue;                
            }

            if (coll.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(0, transform.position - enemy.transform.position);
                enemy.Stun();
            }
        }

        if (!invulnerable)
        {
            StartCoroutine(TimerInvulnerable());
            StartCoroutine(DamageBoost());
        }

        IEnumerator TimerInvulnerable()
        {
            Time.timeScale = 0.4f;
            float time = data.HitInvulnerableTime + 0.1f * (damage - 1);

            invulnerable = true;
        
            while (time > 0)
            {
                time -= Time.fixedUnscaledDeltaTime;
                yield return new WaitForSeconds(Time.fixedUnscaledDeltaTime);
            }

            Time.timeScale = 1f;
            invulnerable = false;
        }

        IEnumerator DamageBoost()
        {
            damageDirection = damageDirection.normalized;
            
            float time = (data.HitInvulnerableTime + 0.1f * (damage - 1));
            float distance = damage;

            float speed = distance / time;

            IsDashing = true;
            
            while (time > 0)
            {
                Vector2 newPosition = body.position +  speed * Time.fixedDeltaTime * damageDirection;
                body.MovePosition(newPosition);
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            IsDashing = false;
        }
    }

    private void Reload(PlayerRangedWeapon reloadWeapon)
    {
        Type ammoType = reloadWeapon.GetType();

        if (!ammoStack.TryGetValue(ammoType, out int ammoCount))
            return;
        
        ammoStack[ammoType] -= reloadWeapon.Reload(ammoCount);
        uiHandler.DepleteAmmo();
    }
    
    public bool IgnoreDamage() => IsDashing || invulnerable;
    
    protected override void Death()
    {
        base.Death();
        RequireReload();
        RestoreHealth(data.MaxHealth);
    }

    public override void Activate()
    {
        base.Activate();
        
        uiHandler.UpdateHealth(currentHealth);

        canAct = true;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        
        canAct = false;
    }

    public void RequireReload()
    {
        OnReloadRequired?.Invoke();
    }

    public void NewCheckPoint(Transform newPoint)
    {
        CurrentCheckPointPosition = newPoint.position;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, data.MaxHealth);
        
        uiHandler.UpdateHealth(currentHealth);
    }

    public void RestoreAmmo(int amount, PlayerWeapon ammoType)
    {
        Type type = ammoType.GetType();

        if (ammoStack.ContainsKey(type))
            ammoStack[type] += amount;
        else
            ammoStack.Add(ammoType.GetType(), amount);
        
        uiHandler.RestoreAmmo();
    }

    public void AllowDash()
    {
        canDash = true;
    }

    public void IncreaseMaxHp(int amount)
    {
        data.IncreaseMaxHealth(amount);
        
        RestoreHealth(amount);
        uiHandler.UpdateHealth(currentHealth);
    }
}
