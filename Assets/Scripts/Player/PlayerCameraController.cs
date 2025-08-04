using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Header("looking")]
    [SerializeField] private float lookSensitivity = 10.0f;
    [SerializeField] private bool smoothCamera = false;
    [SerializeField] private float smoothCameraLerp = 0.4f;
    [SerializeField] private float maxLookAngle = 80.0f;
    [SerializeField] private float minLookAngle = 5.0f;

    [Header("Zooming")]
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float zoomLerp = 0.5f;
    [SerializeField] private float maxZoom = 10.0f;
    [SerializeField] private float minZoom = 0.01f;


    [HideInInspector]
    public InputAction look;
    private Vector2 lookInput;

    [SerializeField] private Transform target; // What the camera pivots around (usually the player)
    private float pitch = 0f;
    private float yaw = 0f;
    private float targetDistance = 8f;
    private float currentDistance;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentDistance = targetDistance;
    }

    private void Update()
    {
        lookInput = look.ReadValue<Vector2>();
        Look();

        if (cam != null)
        {
            Vector3 desiredCameraPos = transform.position - transform.forward * currentDistance;
            cam.transform.position = Vector3.Lerp(cam.transform.position, desiredCameraPos, zoomLerp);
            cam.transform.LookAt(target.position);
        }
    }

    private void Look()
    {
        yaw += lookInput.x * lookSensitivity * Time.deltaTime;
        pitch -= lookInput.y * lookSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minLookAngle, maxLookAngle);

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);

        if (smoothCamera)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothCameraLerp);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

}
