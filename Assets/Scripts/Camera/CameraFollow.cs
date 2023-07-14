using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float speed;

    private Transform targetTransform;

    private void Update()
    {
        if (targetTransform)
        {
            Vector2 cameraFollow = Vector2.Lerp(transform.position, targetTransform.position, speed * Time.deltaTime);
            transform.position = new Vector3(cameraFollow.x, cameraFollow.y, -10);
        }
    }

    public void Initialize()
    {
        targetTransform = FindAnyObjectByType<Player>().transform;

        Vector2 targetPosition = targetTransform.position;
        transform.position = targetPosition;
    }

    public void Deinitialize()
    {
        targetTransform = null;
    }
}
