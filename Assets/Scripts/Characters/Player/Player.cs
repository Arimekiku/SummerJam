using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    public bool debugMode;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCoolDown;
    [SerializeField] private int maxHealth;

    [SerializeField] private float defaultTimeInvul;
    
    [SerializeField] private LayerMask levelLayer;
    
    [SerializeField] private PlayerMeleeWeapon currentMeleeWeapon;
    [SerializeField] private PlayerRangedWeapon currentRangedWeapon;

    [SerializeField] private Transform rangedWeaponContainer;
    [SerializeField] private Transform meleeWeaponContainer;

    [SerializeField] private Image loader;
    [SerializeField] private List<Image> healthContainers;
    [SerializeField] private CinemachineVirtualCamera cameraToEnable;

    public Transform MeleeWeaponContainer => meleeWeaponContainer;
    public Transform RangedWeaponContainer => rangedWeaponContainer;
    public SpriteRenderer Rend { get; private set; }

    [NonSerialized] public Vector2 currentCheckPointPosition;

    private readonly Dictionary<Type, int> ammoStack = new Dictionary<Type, int>();

    private bool dashOnCoolDown;
    public bool IsDashing { get; private set; }
    private bool invulnerable;
    private bool canAct;
    private bool canDash;

    private Rigidbody2D body;
    private CameraObject cameraObject;
    
    private static Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        Rend = GetComponentInChildren<SpriteRenderer>();
        cameraObject = FindObjectOfType<CameraObject>();

        currentHealth = maxHealth;
        
        EnterDebug();
    }

    private void Update()
    {
        if (!canAct) 
            return;
        
        RotateTowardsMouse();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashOnCoolDown)
            Dash();

        if (Input.GetKeyDown(KeyCode.Mouse1)) 
            if (currentMeleeWeapon)
                currentMeleeWeapon.Attack(CursorPosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            if (currentRangedWeapon)
                currentRangedWeapon.Attack(CursorPosition);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentRangedWeapon)
            {
                if (currentRangedWeapon is not Bow)
                {
                    currentRangedWeapon.ThrowWeapon(CursorPosition);
                    currentRangedWeapon = null;
                }
            }

            if (currentMeleeWeapon)
            {
                currentMeleeWeapon.ThrowWeapon(CursorPosition);
                currentRangedWeapon = null;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            IPickupable item = DetectPickup();
            
            PickUpItem(item);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(currentRangedWeapon)
                Reload(currentRangedWeapon);
        }
    }

    private void FixedUpdate()
    {
        if (!IsDashing && canAct)
            Move();
    }

    private void RotateTowardsMouse()
    {
        Vector2 lookDirection = CursorPosition - body.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void Move()
    {
        Vector2 moveVector = InputVector();
        Vector2 newPosition = body.position + moveSpeed * Time.fixedDeltaTime * moveVector;
        body.MovePosition(newPosition);
    }

    private void Dash()
    {
        if (!canDash)
            return;
        
        Vector2 directionDash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (directionDash == Vector2.zero) 
            return;

        IsDashing = true;
        dashOnCoolDown = true;
        StartCoroutine(CoolDownDash());
        StartCoroutine(DashMoving(directionDash));
        
        IEnumerator CoolDownDash()
        {
            float time = dashCoolDown;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            dashOnCoolDown = false;
        }
        
        IEnumerator DashMoving(Vector2 dirDash)
        {
            float time = dashDuration;
            float speed = dashSpeed;
            
            while (time > 0.1)
            {
                Vector2 newPosition = body.position + speed * Time.fixedDeltaTime * dirDash;
                Vector2 newDirection = Vector2.zero;

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, dirDash, 0.8f, levelLayer);

                if (hitInfo)
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

            IsDashing = false;
            
            Vector3 SlideCollision(Vector2 direction, RaycastHit2D hitInfo) => Vector3.ProjectOnPlane(direction, hitInfo.normal);
        }
    }
    
    private Vector2 InputVector()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (inputVector.magnitude > 1) 
            inputVector = inputVector.normalized;

        return inputVector;
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
            }

            if (hitInfo.TryGetComponent(out IInteractable interactable))
            {
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
                if (currentRangedWeapon)
                    currentRangedWeapon.CastOut(playerPosition);

                currentRangedWeapon = weapon;
                return;
            }
        }
    }
    
    public override void TakeDamage(int damage, Vector2 damageDirection)
    {
        if (IgnoreDamage()) 
            return;
        
        base.TakeDamage(damage, damageDirection);
        
        for (int i = 0; i < damage; i++) 
        {
            Image lastContainer = healthContainers.Last(h => h.enabled);
            lastContainer.enabled = false;
        }
        
        if (!invulnerable)
        {
            StartCoroutine(TimerInvulnerable());
            StartCoroutine(DamageBoost());
        }

        IEnumerator TimerInvulnerable()
        {
            float time = defaultTimeInvul + 0.1f * (damage - 1);

            invulnerable = true;
        
            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            invulnerable = false;
        }

        IEnumerator DamageBoost()
        {
            damageDirection = damageDirection.normalized;
            
            float time = (defaultTimeInvul + 0.1f * (damage - 1));
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
        foreach (Type ammoStackKey in ammoStack.Keys)
        {
            if (ammoStackKey != reloadWeapon.GetType())
                continue;
            
            ammoStack[ammoStackKey] -= reloadWeapon.Reload(ammoStack[ammoStackKey]);
            return;
        }
    }
    
    public bool IgnoreDamage() => IsDashing || invulnerable;
    
    protected override void Death()
    {
        base.Death();
        cameraToEnable.enabled = true;
        TeleportToLastCheckPoint();
        RestoreHealth(maxHealth);
    }

    public void TeleportToLastCheckPoint()
    {
        Deactivate();

        loader.rectTransform.DOScale(new Vector3(1, 1, 1), 1f).OnComplete(() =>
        {
            Activate();
            transform.position = currentCheckPointPosition;
            loader.rectTransform.DOScale(new Vector3(0, 0, 0), 1f);
        });
    }

    public override void Activate()
    {
        int i = 0;
        
        foreach (Image healthContainer in healthContainers)
        {
            if (healthContainer.enabled == false)
                healthContainer.enabled = true;

            if (++i == currentHealth)
                break;
        }

        canAct = true;
        cameraObject.FindTarget();
    }

    public override void Deactivate()
    {
        canAct = false;
        cameraObject.LoseTarget();
    }

    public void NewCheckPoint(Transform newPoint)
    {
        currentCheckPointPosition = newPoint.position;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        
        for (int i = 0; i < amount; i++)
        {
            Image firstContainer = healthContainers.First(h => h.enabled == false);
            firstContainer.enabled = true;
        }
    }

    public void RestoreAmmo(int amount, PlayerWeapon ammoType)
    {
        foreach (Type keyValuePair in ammoStack.Keys)
        {
            if (keyValuePair == ammoType.GetType())
            {
                ammoStack[keyValuePair] += amount;
                return;
            }
        }

        ammoStack.Add(ammoType.GetType(), amount);
    }

    public void AllowDash()
    {
        canDash = true;
    }

    public void IncreaseMaxHp(int amount)
    {
        maxHealth += amount;

        for (int i = 0; i < amount; i++)
        {
            Image firstContainer = healthContainers.First(h => h.gameObject.activeSelf == false);
            firstContainer.gameObject.SetActive(true);
        }
        
        RestoreHealth(amount);
    }

    private void EnterDebug()
    {
        if (debugMode)
        {
            Activate();
            cameraObject.FindTarget();
            FindObjectOfType<GameBeginner>().BeginGame();
            transform.position = new Vector2(110, 0);
        }
    }
}
