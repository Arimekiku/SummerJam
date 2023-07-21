using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;

    private SpriteRenderer currentRenderer;
    private Camera currentCamera;
    
    private void Awake()
    {
        Cursor.visible = false;
        
        currentCamera = Camera.main;

        currentRenderer = GetComponent<SpriteRenderer>();
        currentRenderer.sprite = defaultSprite;
    }

    private void Update()
    {
        Vector2 newPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = newPosition;
    }
}
