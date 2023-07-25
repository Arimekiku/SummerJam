using Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] allCameras;

    [Space, SerializeField] private float dashDampingChangeAmount;
    private Coroutine playerDashing;
    
    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer currentTransposer;

    private void Awake()
    {
        for (int i = 0; i < allCameras.Length; i++)
        {
            if (allCameras[i].enabled)
            {
                currentCamera = allCameras[i];
                currentTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }
    }

    public void SwapCameraLeftRight(CinemachineVirtualCamera firstSideCamera, CinemachineVirtualCamera secondSideCamera, Vector2 direction)
    {
        if (currentCamera == firstSideCamera && (direction.x > 0f))
        {
            currentCamera.enabled = false;

            currentCamera = secondSideCamera;
        } 
        else if (currentCamera == secondSideCamera && (direction.x < 0f))
        {
            currentCamera.enabled = false;

            currentCamera = firstSideCamera;
        }
        
        currentCamera.enabled = true;
        currentTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    
    public void SwapCameraUpDown(CinemachineVirtualCamera firstSideCamera, CinemachineVirtualCamera secondSideCamera, Vector2 direction)
    {
        if (currentCamera == firstSideCamera && (direction.y > 0f))
        {
            currentCamera.enabled = false;

            currentCamera = secondSideCamera;
        } 
        else if (currentCamera == secondSideCamera && (direction.y < 0f))
        {
            currentCamera.enabled = false;

            currentCamera = firstSideCamera;
        }
        
        currentCamera.enabled = true;
        currentTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
}
