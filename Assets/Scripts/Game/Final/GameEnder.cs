using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameEnder : MonoBehaviour, IInteractable
{
    [SerializeField] private Stand[] stands;
    [SerializeField] private UnityEvent successEnd;
    [SerializeField] private UnityEvent failedEnd;

    public InteractType Type { get; } = InteractType.Button;
    
    public void Interact()
    {
        if (stands.Any(stand => !stand.IsOccupied))
        {
            failedEnd.Invoke();
            return;
        }

        successEnd.Invoke();
    }
}
