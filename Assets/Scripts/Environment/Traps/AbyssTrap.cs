using UnityEngine;

public class AbyssTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsDashing)
                return;
            
            player.RequireReload();
        }
    }
}
