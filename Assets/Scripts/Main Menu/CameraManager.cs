using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera[] cameras;

    public CinemachineCamera mainCamera;
    public CinemachineCamera galleryCamera;

    public CinemachineCamera startCamera;
    private CinemachineCamera currentCamera;

    private void Start()
    {
        for (int i = 0; i < cameras.Length -1; i++)
        {
            if (cameras[i] == currentCamera)
            {
                cameras[i].Priority = 20;
            }
            else
            {
                cameras[i].Priority = 10;
            }
        }
    }
    public void SwitchCamera(CinemachineCamera newCam)
    {
        currentCamera = newCam;

        currentCamera.Priority = 20;

        for (int i = 0;i < cameras.Length;i++)
        {
            if (cameras[i] != currentCamera)
            {
                cameras[i].Priority = 10;
            }
        }
    }
}
