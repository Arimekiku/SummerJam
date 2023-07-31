using UnityEngine;

public class CameraObject : MonoBehaviour
{
    [SerializeField] private Transform targetOnScene;

    private Transform actualTarget;
    private static Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    private Vector2 CastedPosition => targetOnScene.position;

    private void FixedUpdate()
    {
        if (actualTarget)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetOnScene.rotation, Time.fixedDeltaTime);
            Vector2 resultPosition = (CursorPosition - CastedPosition);

            if (resultPosition.magnitude > 5f)
            {
                Vector2 anotherCast = (CursorPosition - CastedPosition) / 5f + CastedPosition;
                transform.position = anotherCast;
            }
            else
            {
                transform.position = CastedPosition;
            }
        }
    }

    public void LoseTarget()
    {
        actualTarget = null;
    }

    public void FindTarget()
    {
        actualTarget = targetOnScene;
    }
}
