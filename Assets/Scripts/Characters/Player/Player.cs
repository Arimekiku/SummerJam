using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCoolDown;

    [SerializeField] private LayerMask weaponLayer;
    [SerializeField] private LayerMask levelLayer;

    [SerializeField] private Weapon detectedWeapon;
    [SerializeField] private Melee meleeWeapon;

    [SerializeField] private Transform weaponContainer;
    [SerializeField] private Transform meleeWeaponContainer;

    [NonSerialized] public Vector2 currentCheckPointPosition;

    private bool dashOnCoolDown;
    public bool IsDashing { get; private set; }
    public bool canAct;

    private Weapon currentWeapon;
    private Rigidbody2D body;

    private Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canAct) 
        {
            RotateTowardsMouse();

            if (Input.GetKeyDown(KeyCode.LeftShift) && !dashOnCoolDown)
                Dash();

            if (Input.GetKeyDown(KeyCode.Mouse1)) 
                meleeWeapon.Attack(CursorPosition);

            if (Input.GetKeyDown(KeyCode.Mouse0))
                if (currentWeapon)
                    currentWeapon.Attack(CursorPosition);

            if (Input.GetKeyDown(KeyCode.E))
            {
                detectedWeapon = TryWeaponDetector();

                if (detectedWeapon != null)
                {
                    if (currentWeapon)
                        currentWeapon.CastOut(transform.position);

                    currentWeapon = detectedWeapon;
                    currentWeapon.TakeUp(weaponContainer);
                    detectedWeapon = null;
                }
            }   
        }
    }

    private void FixedUpdate()
    {
        if (!IsDashing && canAct)
            Move();
    }

    public void Initialize()
    {
        canAct = true;
        IsDashing = dashOnCoolDown = false;
        health = 3;
        gameObject.SetActive(true);
    }

    public void Deinitialize()
    {
        canAct = false;
        gameObject.SetActive(false);
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
        Vector2 newPosition = body.position + speed * Time.fixedDeltaTime * moveVector;
        body.MovePosition(newPosition);
    }

    private void Dash()
    {
        Vector2 directionDash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (directionDash == Vector2.zero) 
            return;

        IsDashing = true;
        dashOnCoolDown = true;
        StartCoroutine(CoolDownDash());
        StartCoroutine(DashMoving(directionDash));
    }

    private IEnumerator DashMoving(Vector2 directionDash)
    {
        float time = dashDuration;
        float speed = dashSpeed;

        while (time > 0.1)
        {
            Vector2 newPosition = body.position + speed * Time.fixedDeltaTime * directionDash;
            Vector2 newDirection = Vector2.zero;

            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, directionDash, 0.8f, levelLayer);

            if (hitInfo)
            {
                if (Math.Abs(Vector2.Dot(directionDash, hitInfo.normal)) >= 0.98f)
                    break;

                newDirection = SlideCollision(directionDash, hitInfo) * (speed * time / dashDuration);
            }

            if (newDirection != Vector2.zero)
                newPosition = body.position + newDirection * Time.fixedDeltaTime;

            body.MovePosition(newPosition);
            time -= Time.fixedDeltaTime;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        IsDashing = false;
    }

    private Vector3 SlideCollision(Vector2 direction, RaycastHit2D hitInfo)
    {
        return Vector3.ProjectOnPlane(direction, hitInfo.normal);
    }

    private IEnumerator CoolDownDash()
    {
        float time = dashCoolDown;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        dashOnCoolDown = false;
    }

    private Vector2 InputVector()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (inputVector.magnitude > 1) 
            inputVector = inputVector.normalized;

        return inputVector;
    }

    private Weapon TryWeaponDetector()
    {
        RaycastHit2D[] hitsInfo = Physics2D.CircleCastAll(transform.position, 1.5f, transform.forward, 1.5f, weaponLayer);

        if (hitsInfo.Length == 0)
            return null;

        foreach (RaycastHit2D hitInfo in hitsInfo)
        {
            if (hitInfo.collider.TryGetComponent(out IInteractable interactableObject))
            {
                interactableObject.Interact();
                return null;
            }

            Weapon weapon = hitInfo.collider.GetComponentInParent<Weapon>();

            if (weapon.OnTheFloor)
                return weapon;
        }

        return null;
    }

    public void TeleportToLastCheckPoint()
    {
        transform.position = currentCheckPointPosition;
        Camera.main.transform.position = transform.position;
    }
}
