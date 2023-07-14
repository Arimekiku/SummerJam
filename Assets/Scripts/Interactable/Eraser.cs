using UnityEngine;
using UnityEngine.UI;

public class Eraser : MonoBehaviour, IInteractable
{
    [SerializeField] private ErasableWall objectToErase;
    [SerializeField] private Image eraserUI;

    public void Interact()
    {
        objectToErase.MakeErasable();
        eraserUI.enabled = true;
        Destroy(gameObject);
    }
}
