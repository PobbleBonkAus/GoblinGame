using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class multiplayerPlayer : NetworkBehaviour
{
    private CharacterController characterController;
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private GameObject cameraPivot;
    [SerializeField] private float moveSpeed = 5f;

    private PlayerInputActions playerControls;

    private InputAction move;
    private InputAction look;

    private float rotationX, cameraRotationY;
    private Vector2 moveInput;
    private Vector2 lookInput;

    const float MIN_Y = -60.0f;
    const float MAX_Y = 70.0f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            // Disable camera and input for non-local players
            cam.gameObject.SetActive(false);
            enabled = false; // Disable this script on non-owners
            return;
        }
    }

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        move = playerControls.Player.Move;
        look = playerControls.Player.Look;

        move.Enable();
        look.Enable();
    }

    private void Update()
    {
        if (!IsOwner) return;

        GetInputs();
        Move();
    }

    private void GetInputs()
    {
        lookInput = look.ReadValue<Vector2>();
        moveInput = move.ReadValue<Vector2>();
    }

    private void Move()
    {
        rotationX += lookInput.x * mouseSensitivity * Time.deltaTime;
        cameraRotationY -= lookInput.y * mouseSensitivity * Time.deltaTime;
        cameraRotationY = Mathf.Clamp(cameraRotationY, MIN_Y, MAX_Y);

        cameraPivot.transform.localRotation = Quaternion.Euler(cameraRotationY, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, rotationX, 0f);

        Vector3 moveDir = (cameraPivot.transform.right * moveInput.x + cameraPivot.transform.forward * moveInput.y).normalized;
        characterController.SimpleMove(moveDir * moveSpeed);
    }

    private void OnDisable()
    {
        if (move != null) move.Disable();
        if (look != null) look.Disable();
    }
}
