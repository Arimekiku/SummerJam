using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Ammunition : MonoBehaviour, IDestroyed
{
    [SerializeField] protected int damage;
    [SerializeField] private float speed;
    [SerializeField] protected LayerMask destroyLayer;
    [SerializeField] protected LayerMask damageLayer;

    protected Rigidbody2D rb;
    protected Vector3 flightDirection;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (flightDirection != Vector3.zero)
            Flight();
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & destroyLayer) != 0)
        {
            gameObject.SetActive(false);
            return;
        }

        if (((1 << other.gameObject.layer) & damageLayer) != 0)
        {
            Character character = other.gameObject.GetComponentInParent<Character>();
            character.TakeDamage(damage);
            gameObject.SetActive(false);
            return;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & destroyLayer) != 0)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void Flight()
    {
        Vector2 newPosition = transform.position + speed * Time.fixedDeltaTime * flightDirection;
        rb.MovePosition(newPosition);
    }

    public void SetDirectionAndStart(Vector2 direction, Vector2 startPosition)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.SetPositionAndRotation(startPosition, Quaternion.Euler(0f, 0f, angle - 90f));

        flightDirection = direction.normalized;
        gameObject.SetActive(true);
    }

    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }
}
