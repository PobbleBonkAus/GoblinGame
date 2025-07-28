using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsGrabber : MonoBehaviour
{
    [Header("References")]
    public Transform interactRayOrigin;         
    public Transform grabPoint;         

    [Header("Settings")]
    public float interactRange = 3f;
    public float grabForce = 10f;
    public float throwForce = 15f;
    public float useItemTimeoutDuration = 0.3f;

    private Rigidbody grabbedObject;
    private bool grabbing = false;
    private Vector3 initialGrabPointRelative;
    private Vector3 targetPosition;

    private PlayerInputActions playerControls;
    private InputAction interact;
    
    private void Start()
    {
        playerControls = new PlayerInputActions();
        interact = playerControls.Player.Interact;
        interact.Enable();
    }

    private void Update()
    {
        if (interact.ReadValue<float>() > 0) 
        {
            GrabObject();
        }
        else 
        {
            ReleaseObject();
        }
    }

    void FixedUpdate()
    {
        if (grabbing)
        {
            Debug.Log("grabbing");
            MoveGrabbedObject();
        }
    }

    public void GrabObject()
    {
        Ray ray = new Ray(interactRayOrigin.transform.position, interactRayOrigin.transform.forward);
        Debug.DrawRay(interactRayOrigin.transform.position, interactRayOrigin.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {

            Debug.Log(hit);
            if (hit.rigidbody != null)
            {
                grabbedObject = hit.rigidbody;
                initialGrabPointRelative = hit.point - grabbedObject.transform.position;
                grabPoint.position = hit.point;
                grabbing = true;
                Debug.Log(hit.rigidbody);
            }
        }
    }

    public void ReleaseObject()
    {
        if (grabbing)
        {
            grabbedObject = null;
            grabbing = false;
        }
    }

    
    private void MoveGrabbedObject()
    {
        if (grabbedObject != null)
        {
            targetPosition = grabPoint.position - initialGrabPointRelative;
            Vector3 direction = targetPosition - grabbedObject.transform.position;
            grabbedObject.linearVelocity = direction * grabForce / Mathf.Max(1f, grabbedObject.mass);
        }
    }

    public void RotateGrabbedObject(Vector3 rotationVector)
    {
        if (grabbedObject != null)
        {
            grabbedObject.angularVelocity = transform.TransformDirection(rotationVector);
        }
    }
}


