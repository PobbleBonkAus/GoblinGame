using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float rotationSmoothTime = 0.1f;
    public float maxSpeed = 10.0f;
    public float maxSlope = 30f;
    private Rigidbody rb;

    [HideInInspector]
    public InputAction move;
    private Vector2 moveInput;


    [Header("Ragdoll")]
    public float ragdollTime = 3.0f;
    public bool isRagdolled = false;
    public float colliionRagdollLimit = 10.0f;


    [Header("Self Righting")]
    public float righteningForce = 100.0f;
    public float righteningForceLerp = 0.5f;
    public float righteningTrigger = 5.0f;
    public float minRighteningAngle = 3.0f;
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


    void FixedUpdate()
    {
        if (!isRagdolled) 
        {
            Move();

            if (physicsGrabber.grabPressed)
            {
                TurnAwayFromCamera();
            }
            else
            {

                TurnTowardsMoveDirection();
            }

            RightenPlayer();
        }

    }


    void Move()
    {
        if (isRagdolled) return;

        if(rb.linearVelocity.magnitude < maxSpeed) 
        {
            moveInput = move.ReadValue<Vector2>();
            Vector3 cameraRight = Vector3.ProjectOnPlane(cam.right, Vector3.up);
            Vector3 cameraForward = Vector3.ProjectOnPlane(cam.forward, currentSlopeNormal);

            Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            Vector3 velocity = moveDirection.normalized * moveSpeed;
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

    public void DoJump(InputAction.CallbackContext obj) 
    {
        if (isRagdolled && rb.linearVelocity.magnitude < 3.0f) 
        {
            EndRagdoll();
        }
        else if (IsGrounded())
        {
            //Debug.Log("jumping");
            rb.useGravity = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
