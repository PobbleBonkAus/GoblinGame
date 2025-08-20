using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputActionMap playerInputActions;


    public PhysicsGrabber playerPhysicsGrabber;
    public playerProceduralAnimator playerAnimator;
    public RigidbodyPlayerController playerController;
    public PlayerCameraController cameraController;

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Awake()
    {
        inputActions = GetComponent<PlayerInput>().actions;
        playerInputActions = inputActions.FindActionMap("Player");

        playerController.move = playerInputActions.FindAction("Move");
        playerAnimator.pitchInput = playerInputActions.FindAction("Look").ReadValue<float>();

        cameraController.look = playerInputActions.FindAction("Look");
        

        playerInputActions.FindAction("Jump").started += playerController.DoJump;
        
        playerInputActions.FindAction("Grab").performed += playerPhysicsGrabber.DoGrabObject;
        playerInputActions.FindAction("Grab").canceled += playerPhysicsGrabber.DoReleaseObject;

        playerInputActions.FindAction("Throw").started += playerPhysicsGrabber.DoChargeThrow;
        playerInputActions.FindAction("Throw").canceled += playerPhysicsGrabber.DoThrow;

        playerInputActions.FindAction("StoreItem").started += playerPhysicsGrabber.DoPickUp;

        playerInputActions.FindAction("Ragdoll").started += playerController.DoRagdoll;
    }
}
