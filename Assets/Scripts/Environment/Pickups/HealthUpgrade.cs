using UnityEngine;

public class HealthUpgrade : MonoBehaviour, IInteractable
{
    [SerializeField] private int amount;
    [SerializeField] private Dialogue dialogue;

    public InteractType Type { get; } = InteractType.Button;

    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public void Interact()
    {
        player.IncreaseMaxHp(amount);
        dialogue.Interact();
        Destroy(gameObject);
    }
}
