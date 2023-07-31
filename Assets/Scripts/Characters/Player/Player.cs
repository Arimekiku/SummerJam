using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float hitInvulnerableTime;

    [Header("Weapon")]
    [SerializeField] private Transform rangedWeaponContainer;
    [SerializeField] private Transform meleeWeaponContainer;

    public Vector2 CurrentCheckPointPosition { get; private set; }
    public Transform RangedContainer => rangedWeaponContainer;

    private PlayerMeleeWeapon currentMeleeWeapon;
    private PlayerRangedWeapon currentPlayerRangedWeapon;

    private readonly Dictionary<Type, int> ammoStack = new Dictionary<Type, int>();
    private CinemachineImpulseSource shake;

    private bool isDashing;
    private bool invulnerable;
    private bool canAct;

    private PlayerUI uiHandler;
    private Rigidbody2D body;
    
    private static Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        shake = GetComponent<CinemachineImpulseSource>();
        uiHandler = GetComponent<PlayerUI>();
        
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!canAct) 
            return;
        
        RotateTowardsMouse();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Dash();

        if (Input.GetKeyDown(KeyCode.Mouse1)) 
            if (currentMeleeWeapon)
                currentMeleeWeapon.Attack(CursorPosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            if (currentPlayerRangedWeapon)
                currentPlayerRangedWeapon.Attack(CursorPosition);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentPlayerRangedWeapon)
            {
                if (currentPlayerRangedWeapon is not Bow)
                {
                    currentPlayerRangedWeapon.ThrowWeapon(CursorPosition);
                    uiHandler.ClearWeapon();
                    currentPlayerRangedWeapon = null;
                }
            }

            if (currentMeleeWeapon)
            {
                currentMeleeWeapon.ThrowWeapon(CursorPosition);
                currentPlayerRangedWeapon = null;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.R))
            if(currentPlayerRangedWeapon)
                Reload(currentPlayerRangedWeapon);
    }

    private void FixedUpdate()
    {
        if (!isDashing && canAct)
            Move();
    }
    
    private static Vector2 GetInputVector()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (inputVector.magnitude > 1) 
            inputVector = inputVector.normalized;

        return inputVector;
    }

    private void IncreaseMaxHealth(int amount)
    {
        if (amount < 0)
            throw new Exception("Invalid HP amount");

        maxHealth += amount;
    }

    private void RotateTowardsMouse()
    {
        Vector2 lookDirection = CursorPosition - body.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void Move()
    {
        Vector2 moveVector = GetInputVector();
        Vector2 newPosition = body.position + moveSpeed * Time.fixedDeltaTime * moveVector;
        body.MovePosition(newPosition);
    }

    private void Dash()
    {
        if (isDashing)
            return;
        
        Vector2 directionDash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (directionDash == Vector2.zero)
            directionDash = transform.up;

        isDashing = true;

        StartCoroutine(CoolDownDash());
        StartCoroutine(DashMoving(directionDash));
        
        IEnumerator CoolDownDash()
        {
            float time = dashCooldown;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
        
        IEnumerator DashMoving(Vector2 dirDash)
        {
            float time = dashDuration;
            float speed = dashSpeed;
            
            while (time > 0.1)
            {
                Vector2 newPosition = body.position + speed * Time.fixedDeltaTime * dirDash;
                Vector2 newDirection = Vector2.zero;

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, dirDash, 0.8f);

                if (!hitInfo.collider.isTrigger)
                {
                    if (Math.Abs(Vector2.Dot(dirDash, hitInfo.normal)) >= 0.98f)
                        break;

                    newDirection = SlideCollision(dirDash, hitInfo) * (speed * time / dashDuration);
                }

                if (newDirection != Vector2.zero)
                    newPosition = body.position + newDirection * Time.fixedDeltaTime;

                body.MovePosition(newPosition);
                time -= Time.fixedDeltaTime;

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            isDashing = false;
            
            Vector3 SlideCollision(Vector2 direction, RaycastHit2D hitInfo) => Vector3.ProjectOnPlane(direction, hitInfo.normal);
        }
    }

    private void Interact()
    {
        Collider2D[] hitsInfo = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach (Collider2D hitInfo in hitsInfo)
        {
            if (!hitInfo.TryGetComponent(out IInteractable interactable)) 
                continue;
            
            interactable.Interact();
            return;
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
            float time = hitInvulnerableTime + 0.1f * (damage - 1);

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
            
            float time = hitInvulnerableTime + 0.1f * (damage - 1);
            float distance = damage;

            float speed = distance / time;

            isDashing = true;
            
            while (time > 0)
            {
                Vector2 newPosition = body.position +  speed * Time.fixedDeltaTime * damageDirection;
                body.MovePosition(newPosition);
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            isDashing = false;
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
    
    public bool IgnoreDamage() => isDashing || invulnerable;
    
    protected override void Death()
    {
        base.Death();
        RestoreHealth(maxHealth);
    }

    public override void Activate()
    {
        //uiHandler.UpdateHealth(currentHealth);

        canAct = true;
    }

    public override void Deactivate()
    {
        canAct = false;
    }

    public void NewCheckPoint(Transform newPoint)
    {
        CurrentCheckPointPosition = newPoint.position;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        
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

    public void IncreaseMaxHp(int amount)
    {
        IncreaseMaxHealth(amount);
        
        RestoreHealth(amount);
        uiHandler.UpdateHealth(currentHealth);
    }
}
