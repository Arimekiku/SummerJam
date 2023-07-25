using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Corpse : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        rb.AddForce(direction);
    }
}