using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent toPerform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
            toPerform.Invoke();
    }
}
