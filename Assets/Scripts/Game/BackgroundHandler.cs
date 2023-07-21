using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    [SerializeField] private Player player;
    [Space, SerializeField] private Color targetColor;
    
    private SpriteRenderer rend;
    
    private const float MaximumRange = 80f;
    private Vector3 playerStartPosition;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        playerStartPosition = player.transform.position;
    }

    private void Update()
    {
        float currentValue = Mathf.Abs(player.transform.position.x) > MaximumRange ? 1 : (player.transform.position.x - playerStartPosition.x) / MaximumRange;

        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, currentValue);

        if (currentValue >= 1f)
        {
            rend.color = targetColor;
            Destroy(this);
        }
    }
}
