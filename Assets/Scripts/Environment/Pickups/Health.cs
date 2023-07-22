using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int healthToRestore;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.RestoreHealth(healthToRestore);
            Destroy(gameObject);
        }
    }
}
