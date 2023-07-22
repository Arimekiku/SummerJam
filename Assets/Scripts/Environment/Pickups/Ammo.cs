using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private PlayerWeapon refillType;
    
    private const int CountAmmo = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.RestoreAmmo(CountAmmo, refillType);
            Destroy(gameObject);
        }
    }
}
