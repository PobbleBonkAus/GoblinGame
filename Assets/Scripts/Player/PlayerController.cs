using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
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
    float variedRighteningForce = 100.0f;

    [Header("References")]
    [SerializeField] private PhysicsGrabber physicsGrabber;
    [SerializeField] Transform playerVisuals;
    [SerializeField] Transform cam;

    private Vector3 groundPoint;

    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] float hoverFactor = 3.0f;


    Vector3 currentSlopeNormal = Vector3.zero;
    Transform orientation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        orientation = new GameObject().transform;
        orientation.position = transform.position;
    }


    void Start()
    {
        rb.Move(GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position, Random.rotation);
    }

    void FixedUpdate()
    {
        if (!isRagdolled) 
        {
            if (!physicsGrabber.finesseMode) 
            {
                Move();
            }

            if (physicsGrabber.grabPressed)
            {
                //TurnAwayFromCamera();
                TurnTowardsMoveDirection();
            }
            else
            {

                TurnTowardsMoveDirection();
            }

            RightenPlayer();
        }

        ApplyBetterJump();

    }


    void Move()
    {
        if (isRagdolled) return;

        if(rb.linearVelocity.magnitude < maxSpeed) 
        {
            moveInput = move.ReadValue<Vector2>();
            Vector3 cameraRight = Vector3.ProjectOnPlane(cam.right, Vector3.up);
            Vector3 cameraForward = Vector3.ProjectOnPlane(cam.forward, currentSlopeNormal);
            Vector3 flatCameraForward = cameraForward;
            flatCameraForward.y = 0;
            flatCameraForward.Normalize();

            Vector3 moveDirection = flatCameraForward * moveInput.y + cameraRight * moveInput.x;

            Vector3 velocity = moveDirection * moveSpeed;
            Vector3 currentYVelocity = Vector3.up * rb.linearVelocity.y;

            rb.AddForce(velocity + currentYVelocity);
        }
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
        if (rb.linearVelocity.y < fallingTrigger) // falling
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !isJumpHeld) // early release
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    public void DoJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started) // button pressed
        {
            if (isRagdolled && rb.linearVelocity.magnitude < 3.0f && IsGrounded())
            {
                EndRagdoll();
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
        else 
        {
            EndRagdoll();
        }
    }


    private void StartRagdoll() 
    {
        isRagdolled = true;
        variedRighteningForce = 0.0f;
        rb.AddForce(rb.linearVelocity * ragdollImpulse, ForceMode.Impulse);
        rb.AddForce(transform.up * ragdollImpulse, ForceMode.Impulse);
        rb.AddTorque(Random.rotation.eulerAngles * ragdollTorqueKick, ForceMode.Impulse);
        gameObject.layer = LayerMask.NameToLayer("Grabbable");
    }

    private void EndRagdoll() 
    {
        isRagdolled = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

   

    public bool IsGrounded()
    {

        if(Physics.Raycast(groundCheckOrigin.position, Vector3.down,out RaycastHit hit, 1.0f)) 
        {
            groundPoint = hit.point;
            currentSlopeNormal = hit.normal;
            return true;
        }
        else 
        {
            groundPoint = transform.position;
            return false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.impulse.magnitude > colliionRagdollLimit)
        {
            StartRagdoll();
        }
    }

    private void OnDrawGizmos()
    {      
        Gizmos.DrawRay(transform.position, Vector3.down);
    }

}
