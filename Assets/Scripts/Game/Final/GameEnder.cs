using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameEnder : MonoBehaviour, IInteractable
{
    [SerializeField] private Stand[] stands;
    [SerializeField] private UnityEvent successEnd;
    [SerializeField] private UnityEvent failedEnd;

    public InteractType Type { get; } = InteractType.Button;
    public int Result { get; private set; }
    
    public void Interact()
    {
        if (stands.Any(stand => !stand.IsOccupied))
        {
            failedEnd.Invoke();
            return;
        }

        Result = stands.Sum(stand => stand.StandValue) switch
        {
            3 => 1,
            -3 => -1,
            _ => 0
        };

        successEnd.Invoke();
    }
}
