using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] private LayerMask levelLayer;

    [SerializeField] private Transform rangedWeaponContainer;
    [SerializeField] private Transform meleeWeaponContainer;

    [SerializeField] private List<Image> healthContainers;
    [SerializeField] private Transform ammoContainer;
    [SerializeField] private Transform pocketAmmoContainer;
    [SerializeField] private Image weaponImage;

    [NonSerialized] public Vector2 currentCheckPointPosition;
    
    public event Action OnWeaponPickup;
    public event Action OnReloadRequired;
    
    public Transform MeleeWeaponContainer => meleeWeaponContainer;
    public Transform RangedWeaponContainer => rangedWeaponContainer;
    public SpriteRenderer Rend { get; private set; }

    private PlayerMeleeWeapon currentMeleeWeapon;
    private PlayerRangedWeapon currentRangedWeapon;

    private readonly Dictionary<Type, int> ammoStack = new Dictionary<Type, int>();
    private List<Image> pocketAmmoSprites = new List<Image>();
    private CinemachineImpulseSource shake;

    public bool IsDashing { get; private set; }
    private bool invulnerable;
    private bool canAct;
    private bool canDash;

    private Image magazineToSpawn;
    private PlayerData info;
    private Rigidbody2D body;
    
    private static Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        Rend = GetComponent<SpriteRenderer>();
        shake = GetComponent<CinemachineImpulseSource>();
        
        magazineToSpawn = Resources.Load("Prefabs/Weapon/PlayerWeapon/AmmoUI", typeof(Image)) as Image;
        info = Resources.Load("Prefabs/Player/Data", typeof(PlayerData)) as PlayerData;
    }

    private void Start()
    {
        foreach (Image healthContainer in healthContainers)
            healthContainer.enabled = false;
        
        currentHealth = info.MaxHealth;
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

                weaponImage.sprite = null;
                pocketAmmoSprites.ForEach(s => Destroy(gameObject));
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
        Vector2 newPosition = body.position + info.MoveSpeed * Time.fixedDeltaTime * moveVector;
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
        canDash = false;
        StartCoroutine(CoolDownDash());
        StartCoroutine(DashMoving(directionDash));
        
        IEnumerator CoolDownDash()
        {
            float time = info.DashCooldown;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            canDash = true;
        }
        
        IEnumerator DashMoving(Vector2 dirDash)
        {
            float time = info.DashDuration;
            float speed = info.DashSpeed;
            
            while (time > 0.1)
            {
                Vector2 newPosition = body.position + speed * Time.fixedDeltaTime * dirDash;
                Vector2 newDirection = Vector2.zero;

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, dirDash, 0.8f, levelLayer);

                if (hitInfo)
                {
                    if (Math.Abs(Vector2.Dot(dirDash, hitInfo.normal)) >= 0.98f)
                        break;

                    newDirection = SlideCollision(dirDash, hitInfo) * (speed * time / info.DashDuration);
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
    
    private static Vector2 InputVector()
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

                    pocketAmmoSprites.ForEach(s => Destroy(s.gameObject));
                    pocketAmmoSprites.Clear();
                }
                
                currentRangedWeapon = weapon;
                weaponImage.sprite = currentRangedWeapon.WeaponSprite;
                
                currentRangedWeapon.currentAmmoUI = new AmmunitionUI[currentRangedWeapon.MaxAmmo];

                for (int i = 0; i < currentRangedWeapon.MaxAmmo; i++)
                {
                    AmmunitionUI UIToSpawn = currentRangedWeapon.currentAmmoUI[i] = Instantiate(currentRangedWeapon.AmmoUI, ammoContainer);
                    
                    if (i < currentRangedWeapon.CurrentAmmo)
                        UIToSpawn.Reload();
                    else
                        UIToSpawn.Deplete();
                }

                if (ammoStack.ContainsKey(currentRangedWeapon.GetType()))
                {
                    int magazinesToSpawn = ammoStack[currentRangedWeapon.GetType()] % currentRangedWeapon.MaxAmmo;
                    for (int i = 0; i < magazinesToSpawn; i++)
                    {
                        Image imageToSpawn = Instantiate(magazineToSpawn, pocketAmmoContainer);
                        pocketAmmoSprites.Add(imageToSpawn);

                        imageToSpawn.transform.localPosition = new Vector2(0, 30 * i);
                    }    
                }
                
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
        
        for (int i = 0; i < damage; i++) 
        {
            Image lastContainer = healthContainers.Last(h => h.enabled);
            lastContainer.enabled = false;
        }

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
            float time = info.HitInvulnerableTime + 0.1f * (damage - 1);

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
            
            float time = (info.HitInvulnerableTime + 0.1f * (damage - 1));
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
        
        if (ammoStack.ContainsKey(ammoType))
        {
            if (pocketAmmoSprites.Count != 0)
            {
                Image imageToDestroy = pocketAmmoSprites.Last();
                pocketAmmoSprites.Remove(imageToDestroy);
                ammoStack[ammoType] -= reloadWeapon.Reload(ammoStack[ammoType]);
                Destroy(imageToDestroy.gameObject);
            }
        }
    }
    
    public bool IgnoreDamage() => IsDashing || invulnerable;
    
    protected override void Death()
    {
        base.Death();
        RequireReload();
        RestoreHealth(info.MaxHealth);
    }

    public override void Activate()
    {
        base.Activate();
        
        int i = 0;
        
        foreach (Image healthContainer in healthContainers)
        {
            if (healthContainer.enabled == false)
                healthContainer.enabled = true;

            if (++i == currentHealth)
                break;
        }

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
        currentCheckPointPosition = newPoint.position;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, info.MaxHealth);
        
        for (int i = 0; i < amount; i++)
        {
            Image firstContainer = healthContainers.First(h => h.enabled == false);
            firstContainer.enabled = true;
        }
    }

    public void RestoreAmmo(int amount, PlayerWeapon ammoType)
    {
        Type type = ammoType.GetType();

        if (ammoStack.ContainsKey(type))
            ammoStack[type] += amount;
        else
            ammoStack.Add(ammoType.GetType(), amount);
        
        Image imageToSpawn = Instantiate(magazineToSpawn, pocketAmmoContainer);

        if (pocketAmmoSprites.Count != 0)
            imageToSpawn.transform.localPosition = pocketAmmoSprites.Last().transform.localPosition + Vector3.up * 30f;
        else
            imageToSpawn.transform.localPosition = Vector3.zero;
        
        pocketAmmoSprites.Add(imageToSpawn);
    }

    public void AllowDash()
    {
        canDash = true;
    }

    public void IncreaseMaxHp(int amount)
    {
        info.IncreaseMaxHealth(amount);

        for (int i = 0; i < amount; i++)
        {
            Image firstContainer = healthContainers.First(h => h.gameObject.activeSelf == false);
            firstContainer.gameObject.SetActive(true);
        }
        
        RestoreHealth(amount);
    }
}
