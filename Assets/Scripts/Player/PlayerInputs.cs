using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputActionMap playerInputActions;


    public PhysicsGrabber playerPhysicsGrabber;
    public RigidbodyPlayerController playerController;
    public CameraController cameraController;

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Awake()
    {
        inputActions = GetComponent<PlayerInput>().actions;
        playerInputActions = inputActions.FindActionMap("Player");

        playerController.move = playerInputActions.FindAction("Move");
        cameraController.look = playerInputActions.FindAction("Look");

        playerInputActions.FindAction("Jump").started += playerController.DoJump;
        
        playerInputActions.FindAction("Grab").started += playerPhysicsGrabber.DoGrabObject;
        playerInputActions.FindAction("Grab").canceled += playerPhysicsGrabber.DoReleaseObject;

        playerInputActions.FindAction("Throw").started += playerPhysicsGrabber.DoChargeThrow;
        playerInputActions.FindAction("Throw").canceled += playerPhysicsGrabber.DoThrow;

        playerInputActions.FindAction("Ragdoll").started += playerController.DoRagdoll;
        playerInputActions.FindAction("ChangeCamera").started += cameraController.DoChangeCamera;

    }
}
