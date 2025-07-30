using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    

    [Header("References")]
    public Transform player;
    public Transform cameraTransform;

    [Header("Cameras")]
    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;

    [Header("Variables")]
    public float lookSensitivity = 5.0f;



    [HideInInspector]
    public InputAction look;
    private Vector2 lookInput;
    private bool isFirstPerson = false;

    public Camera currentCamera;
    float pitch = 0;
    private void Awake()
    {
        currentCamera = thirdPersonCamera;
    }

    private void Update()
    {
        Look();
    }

    void Look()
    {
        lookInput = look.ReadValue<Vector2>();

        float yaw = lookInput.x * lookSensitivity;
        float pitchDelta = -lookInput.y * lookSensitivity;

        // Rotate player horizontally (yaw)
        player.Rotate(0, yaw, 0);

        // Apply and clamp vertical rotation (pitch)
        pitch += pitchDelta;
        pitch = Mathf.Clamp(pitch, -80f, 80f); // Adjust as needed

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void DoChangeCamera(InputAction.CallbackContext obj)
    {
        isFirstPerson = !isFirstPerson;

        if (isFirstPerson)
        {
            thirdPersonCamera.gameObject.SetActive(true);
            firstPersonCamera.gameObject.SetActive(false);
        }
        else
        {
            thirdPersonCamera.gameObject.SetActive(false);
            firstPersonCamera.gameObject.SetActive(true);
        }
    }
}
