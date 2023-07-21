using UnityEngine;
using UnityEngine.UI;

public class Eraser : MonoBehaviour, IInteractable
{
    [SerializeField] private ErasableWall objectToErase;
    [SerializeField] private Image eraserUI;

    public InteractType Type { get; } = InteractType.Trigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
            Interact();
    }

    public void Interact()
    {
        objectToErase.MakeErasable();
        eraserUI.enabled = true;
        Destroy(gameObject);
    }
}
