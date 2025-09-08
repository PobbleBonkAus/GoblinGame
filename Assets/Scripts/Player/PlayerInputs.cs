using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputActionMap playerInputActions;


    public PhysicsGrabber playerPhysicsGrabber;
    public playerProceduralAnimator playerAnimator;
    public PlayerController playerController;
    public PlayerCameraController cameraController;
    public TutorialIcons tutorial;

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    
    void Awake()
    {
        inputActions = GetComponent<PlayerInput>().actions;
        playerInputActions = inputActions.FindActionMap("Player");

        playerController.move = playerInputActions.FindAction("Move");
        cameraController.look = playerInputActions.FindAction("Look");

        playerInputActions.FindAction("Jump").started += playerController.DoJump;
        
        playerInputActions.FindAction("Grab").performed += playerPhysicsGrabber.DoGrabObject;
        playerInputActions.FindAction("Grab").canceled += playerPhysicsGrabber.DoReleaseObject;

        playerInputActions.FindAction("Throw").started += playerPhysicsGrabber.DoChargeThrow;
        playerInputActions.FindAction("Throw").canceled += playerPhysicsGrabber.DoThrow;

        playerInputActions.FindAction("RaiseObject").performed += playerPhysicsGrabber.DoRaiseObject;
        playerInputActions.FindAction("RaiseObject").canceled += playerPhysicsGrabber.DoLowerObject;

        playerInputActions.FindAction("StoreItem").started += playerPhysicsGrabber.DoPickUp;

        playerInputActions.FindAction("Ragdoll").started += playerController.DoRagdoll;

        //tutorial actions
        playerInputActions.FindAction("Move").performed += tutorial.DoWalkAction;
        playerInputActions.FindAction("Jump").performed += tutorial.DoJumpAction;
        playerInputActions.FindAction("Grab").performed += tutorial.DoGrabAction;
    }


}
