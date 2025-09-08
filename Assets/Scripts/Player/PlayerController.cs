using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    float currentMoveSpeed = 0f;
    float moveDampening = 0f;
    [SerializeField] float rotationSmoothTime = 0.1f;
    [SerializeField] float maxSpeed = 10.0f;
    [SerializeField] float maxSlope = 30f;
    private Rigidbody rb;


    [HideInInspector] public InputAction move;
    private Vector2 moveInput;


    [Header("Ragdoll")]
    [HideInInspector] public bool isRagdolled = false;
    [SerializeField] float ragdollTime = 3.0f;
    [SerializeField] float ragdollTorqueKick = 3.0f;
    [SerializeField] float ragdollJumpKick = 3.0f;
    [SerializeField] float ragdollImpulse = 5.0f;
    [SerializeField] float colliionRagdollLimit = 10.0f;
    [HideInInspector] public bool isInOcean;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float downwardsImpulse = 3f;
    private bool jumpPressed = false;
    private bool isJumpHeld = false;
    [SerializeField] private float fallMultiplier = 4f;
    [SerializeField] private float lowJumpMultiplier = 6f;
    [SerializeField] private bool ragdollWhenJump = false;
    [SerializeField] private float fallingTrigger = -1.0f;

    [Header("Self Righting")]
    [SerializeField] float righteningForce = 100.0f;
    [SerializeField] float righteningForceLerp = 0.5f;
    [SerializeField] float righteningTrigger = 5.0f;
    [SerializeField] float minRighteningAngle = 3.0f;
    private float variedRighteningForce = 100.0f;

    [Header("Expressions")]
    [SerializeField] Renderer[] eyeRenderers;
    [SerializeField][Range(1, 4)] float eyeBlankStare;
    [SerializeField][Range(1, 4)] float eyeDazed;
    [SerializeField] GameObject eyeDazeGameObject;

    [Header("References")]
    [SerializeField] private PhysicsGrabber physicsGrabber;
    [SerializeField] Transform playerVisuals;
    [SerializeField] Transform cam;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] Transform groundCheckOrigin;
    
    private GameManager gameManager;
    private Vector3 groundPoint;
    private Vector3 currentSlopeNormal = Vector3.zero;
    private Transform orientation;

    [SerializeField] LayerMask wallCheckLayerMask;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        orientation = new GameObject().transform;
        gameManager = FindAnyObjectByType<GameManager>();
        orientation.position = transform.position;
    }


    void Start()
    {
        rb.Move(GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position, Quaternion.identity);
        playerVisuals.gameObject.SetActive(true);

        currentMoveSpeed = moveSpeed;
        moveDampening = rb.linearDamping;
    }

    void FixedUpdate()
    {
        if(transform.position.y < -3.0f) 
        {
            playerUI.FadeToBlack();
        }

        if (!isRagdolled) 
        {
            Move();
            TurnTowardsMoveDirection();
            RightenPlayer();
        }

        ApplyBetterJump();

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            rb.Move(GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position, Quaternion.identity);
            rb.linearVelocity = Vector3.zero;
        }
    }

    void Move()
    {
        if (isRagdolled) return;

        moveInput = move.ReadValue<Vector2>();


        // Camera-relative movement (ignores camera tilt)
        Vector3 cameraRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        Vector3 cameraForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // --- Handle slopes ---
        if (currentSlopeNormal != Vector3.zero)
            moveDirection = Vector3.ProjectOnPlane(moveDirection, currentSlopeNormal).normalized;

        // Build horizontal velocity
        Vector3 horizontalVelocity = moveDirection * currentMoveSpeed;

        // Clamp horizontal speed only
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeed);

        // Preserve vertical velocity (gravity, jumps, falling)
        float verticalVelocity = rb.linearVelocity.y;

        // Apply combined velocity
        Vector3 finalVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        if (Physics.Raycast(transform.position, horizontalVelocity.normalized, 1.0f, wallCheckLayerMask))
        {
            finalVelocity = new Vector3(0.0f, verticalVelocity, 0.0f);    
        }

        rb.linearVelocity = finalVelocity;
        
    }

    void TurnTowardsMoveDirection()
    {
        if(moveInput.magnitude > 0.1f)
        {
            Vector3 horizontalVelocity = rb.linearVelocity;
            horizontalVelocity.y = 0; // ignore vertical movement

            if (horizontalVelocity.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
            }
        }
    }

    void TurnAwayFromCamera() 
    {
        // Camera direction
        Vector3 cameraDir = cam.forward;

        // Calculate horizontal (Y axis) angle
        Vector3 flatCameraForward = cameraDir;
        flatCameraForward.y = 0;
        flatCameraForward.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(flatCameraForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
        
    }
    void RightenPlayer()
    {
        if(variedRighteningForce < righteningForce) 
        {
            variedRighteningForce += Time.deltaTime * righteningForceLerp;
        }
        var angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle > minRighteningAngle)
        {
            var axis = Vector3.Cross(transform.up, Vector3.up);
            rb.AddTorque(axis * angle * variedRighteningForce);
        }
    }


    void ApplyBetterJump()
    {
        if (isInOcean) return;

        if (rb.linearVelocity.y < fallingTrigger) // falling
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !isJumpHeld) // early release
        {
           // rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    public void DoJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started) // button pressed
        {
            if (rb == null) return;

            if (isRagdolled)
            {
                EndRagdoll();
                return;
            }
            else if (IsGrounded())
            {
                if (ragdollWhenJump) 
                {
                    isRagdolled = true;
                    rb.AddRelativeTorque(Random.rotation.eulerAngles * ragdollJumpKick, ForceMode.Impulse);
                }
                jumpPressed = true;
                isJumpHeld = true;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset Y
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        else if (ctx.canceled) // button released
        {
            isJumpHeld = false;
        }
    }
    public void DoRagdoll(InputAction.CallbackContext obj)
    {
        if (!isRagdolled)
        {
            StartRagdoll();
        }
        else if(rb.linearVelocity.magnitude < 3.0f)
        {
            EndRagdoll();
        }
    }


    public void StartRagdoll() 
    {
        isRagdolled = true;
        variedRighteningForce = 0.0f;
        rb.AddForce((rb.linearVelocity + Random.insideUnitSphere) * ragdollImpulse, ForceMode.Impulse);
        rb.AddForce(transform.up * ragdollImpulse, ForceMode.Impulse);
        rb.AddTorque(Random.rotation.eulerAngles * ragdollTorqueKick, ForceMode.Impulse);
        gameObject.layer = LayerMask.NameToLayer("Grabbable");

        eyeDazeGameObject.SetActive(true);
        eyeRenderers[0].material.SetFloat("_EyePlacement", eyeDazed);
        eyeRenderers[1].material.SetFloat("_EyePlacement", eyeDazed);
    }

    private void EndRagdoll() 
    {
        isRagdolled = false;
        gameObject.layer = LayerMask.NameToLayer("Player");

        eyeDazeGameObject.SetActive(false);
        eyeRenderers[0].material.SetFloat("_EyePlacement", eyeBlankStare);
        eyeRenderers[1].material.SetFloat("_EyePlacement", eyeBlankStare);
    }

   

    public bool IsGrounded()
    {
        if(Physics.Raycast(groundCheckOrigin.position, Vector3.down, out RaycastHit hit, 1.0f)) 
        {
            groundPoint = hit.point;
            currentSlopeNormal = hit.normal;
            return true;
        }
        else 
        {
            currentSlopeNormal = Vector3.zero;
            groundPoint = transform.position;
            return false;
        }

    }

    public void WashUpOnBeach() 
    {
        rb.MovePosition(gameManager.GetNearestBeachSpawn(transform.position));
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.impulse.magnitude > colliionRagdollLimit)
        {
            StartRagdoll();
        }
    }
}
