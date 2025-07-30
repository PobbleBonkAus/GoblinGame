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

    private Rigidbody rb;

    [HideInInspector]
    public InputAction move;
    private Vector2 moveInput;

    [SerializeField]
    Transform cameraTransform;

    [Header("Ragdoll")]
    public float ragdollTime = 3.0f;
    private bool isRagdolled = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }


    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (isRagdolled) return;

        moveInput = move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        Vector3 velocity = moveDirection.normalized * moveSpeed;
        Vector3 currentYVelocity = Vector3.up * rb.linearVelocity.y;


        rb.linearVelocity = velocity + currentYVelocity;

    }


    public void DoJump(InputAction.CallbackContext obj) 
    {
        if(IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void DoRagdoll(InputAction.CallbackContext obj) 
    {
        if (!isRagdolled) 
        {
            StartCoroutine("Ragdoll");
        }
    }


    IEnumerator Ragdoll() 
    {
        isRagdolled = true;
        rb.freezeRotation = false;

        yield return new WaitForSeconds(ragdollTime);

        isRagdolled = false;
        Vector3 upRotation = new Vector3(0, transform.eulerAngles.y, 0);
        transform.eulerAngles = upRotation;

        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = true;
    }


    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

}
