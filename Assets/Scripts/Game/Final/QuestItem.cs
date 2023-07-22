using UnityEngine;

public class QuestItem : MonoBehaviour, IInteractable, IDestroyed
{
    [SerializeField] private Stand standToOccupy;
    [SerializeField] private Dialogue questDialogue;
    
    public InteractType Type { get; } = InteractType.Button;

    private SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        standToOccupy.Occupy(rend.sprite);
        questDialogue.Interact();
    }

    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }
}