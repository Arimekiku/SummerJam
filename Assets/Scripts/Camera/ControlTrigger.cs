using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class ControlTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera firstCamera;
    [SerializeField] private CinemachineVirtualCamera secondCamera;
    
    private Collider2D coll;
    private CameraHandler handler;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        handler = FindObjectOfType<CameraHandler>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Vector2 exitDirection = (other.transform.position - coll.bounds.center).normalized;
        
        if (other.TryGetComponent(out Player _))
        {
            handler.SwapCamera(firstCamera, secondCamera, exitDirection);
        }
    }
}
