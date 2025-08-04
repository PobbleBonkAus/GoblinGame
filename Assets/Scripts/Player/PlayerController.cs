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
    private Rigidbody rb;

    [HideInInspector]
    public InputAction move;
    private Vector2 moveInput;


    [Header("Ragdoll")]
    public float ragdollTime = 3.0f;
    public bool isRagdolled = false;

    [Header("References")]
    [SerializeField] private PhysicsGrabber physicsGrabber;
    [SerializeField] Transform playerVisuals;
    [SerializeField] Transform cam;

    private Vector3 groundPoint;

    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] float hoverFactor = 3.0f;



    Transform orientation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        orientation = new GameObject().transform;
        orientation.position = transform.position;
        
    }


    void FixedUpdate()
    {
        if (!isRagdolled) 
        {
            Move();
            if (physicsGrabber.grabbing)
            {
                TurnAwayFromCamera();
            }
            else
            {
                TurnTowardsMoveDirection();
            }

        }

    }

    void Move()
    {
        if (isRagdolled) return;
        if(rb.linearVelocity.magnitude < maxSpeed) 
        {
            moveInput = move.ReadValue<Vector2>();
            Vector3 cameraRight = Vector3.ProjectOnPlane(cam.right, Vector3.up);
            Vector3 cameraForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up);

            Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            Vector3 velocity = moveDirection.normalized * moveSpeed;
            Vector3 currentYVelocity = Vector3.up * rb.linearVelocity.y;

            rb.AddForce(velocity + currentYVelocity);
        }

    }

    void TurnTowardsMoveDirection()
    {
        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0; // ignore vertical movement

        if (horizontalVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
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

    public void DoJump(InputAction.CallbackContext obj) 
    {
        if(IsGrounded())
        {
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
        rb.freezeRotation = false;
    }

    private void EndRagdoll() 
    {
        isRagdolled = false;
        Vector3 upRotation = new Vector3(0, transform.eulerAngles.y, 0);
        transform.eulerAngles = upRotation;

        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = true;
    }

    public bool IsGrounded()
    {

        if(Physics.Raycast(groundCheckOrigin.position, Vector3.down,out RaycastHit hit, 1.0f)) 
        {
            groundPoint = hit.point;
            return true;
        }
        else 
        {
            groundPoint = transform.position;
            return false;
        }

    }

    private void OnDrawGizmos()
    {      
        Gizmos.DrawRay(transform.position, Vector3.down);
    }

}
