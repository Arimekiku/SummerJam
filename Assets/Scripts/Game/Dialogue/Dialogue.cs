using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour, IInteractable, IDestroyed
{
    [SerializeField] private InteractType dialogType;
    [SerializeField] private DialogueHandler dialogueBox;
    [SerializeField] private int timeBeforeExecuting;
    [Space, SerializeField] private UnityEvent onDialogueEnd;
    [Space, SerializeField] private DialoguePhrase[] phrases;
    [Space, SerializeField] private DialogueActor[] actors;

    public InteractType Type => dialogType;

    private void Start()
    {
        if (dialogType != InteractType.Timer)
            return;
        
        Invoke(nameof(Interact), timeBeforeExecuting);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Type != InteractType.Trigger) 
            return;

        if (other.TryGetComponent(out Player _))
            Interact();
    }

    public void Interact()
    {
        dialogueBox.StartDialogue(phrases, actors, onDialogueEnd);
    }

    public void DestroyThisObject()
    {
        Destroy(this);
    }
}
