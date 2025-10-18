using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputs : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputActionMap playerInputActions;


    public PhysicsGrabber playerPhysicsGrabber;
    public playerProceduralAnimator playerAnimator;
    public PlayerController playerController;
    public PlayerCameraController cameraController;
    public OptionsMenu optionsMenu;

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

        playerInputActions.FindAction("Ragdoll").started += playerController.DoRagdoll;

        playerInputActions.FindAction("ToggleOptionsMenu").started += DoOpenOptions;
        playerInputActions.FindAction("ResetPlayer").started += DoResetPlayer;
        playerInputActions.FindAction("ResetGame").started += DoResetGame;
        playerInputActions.FindAction("Leave").performed += DoLeaveGame;
    }

    public void DoOpenOptions(InputAction.CallbackContext obj)
    {
        optionsMenu.gameObject.SetActive(!optionsMenu.gameObject.activeSelf);
    }

    public void DoResetPlayer(InputAction.CallbackContext obj) 
    {
        if (optionsMenu.gameObject.activeSelf) 
        {
            playerController.ResetPlayer();
            optionsMenu.gameObject.SetActive(false);
        }
    }

    public void DoResetGame(InputAction.CallbackContext obj) 
    {

        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        
    }

    public void DoLeaveGame(InputAction.CallbackContext obj)
    {

        GameManager.instance.PlayerLeave(GetComponent<PlayerInput>());

    }
}
