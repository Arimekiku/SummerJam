using UnityEngine;

public class XAxisCameraHolder : MonoBehaviour
{
    private Player player;

    private bool isTracking;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (isTracking)
            transform.position = new Vector2(player.transform.position.x, transform.position.y);
    }

    public void EnableTracking()
    {
        isTracking = true;
    }

    public void DisableTracking()
    {
        isTracking = false;
    }
}
