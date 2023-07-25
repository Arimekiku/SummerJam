using Cinemachine;
using UnityEngine;

public class ControlTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera firstCamera;
    [SerializeField] private CinemachineVirtualCamera secondCamera;
    [SerializeField] private bool invertAxis;
    
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
            if (!invertAxis)
                handler.SwapCameraLeftRight(firstCamera, secondCamera, exitDirection);
            else
                handler.SwapCameraUpDown(firstCamera, secondCamera, exitDirection);
        }
    }
}
