using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour, IInteractable, IDestroyable
{
    [SerializeField] private DialogueHandler dialogueBox;
    [Space, SerializeField] private UnityEvent onDialogueEnd;
    [Space, SerializeField] private DialoguePhrase[] phrases;
    [Space, SerializeField] private DialogueActor[] actors;

    public void DestroyThisObject()
    {
        Destroy(this);
    }

    public void Interact()
    {
        dialogueBox.StartDialogue(phrases, actors, onDialogueEnd);

    }
}
