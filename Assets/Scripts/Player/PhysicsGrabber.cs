using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsGrabber : MonoBehaviour
{
 
    private Vector3 grabPoint;
    public Vector3 globalGrabPoint;

    [Header("Settings")]
    public float interactRange = 3f;
    public float grabForce = 10f;
    public float throwForce = 15f;
    public float useItemTimeoutDuration = 0.3f;

    private Rigidbody grabbedObject;
    public bool grabbing = false;
    private Vector3 initialGrabPointRelative;
    private Vector3 targetPosition;


    [Header("Throwing")]
    public float maxThrowForceTime = 50.0f;
    private float throwForceTimer = 0.0f;
    private bool chargingThrow = false;


    void FixedUpdate()
    {
        if (grabbing)
        {
            MoveGrabbedObject();

            if (chargingThrow) 
            {
                if (throwForceTimer < maxThrowForceTime)
                {
                    throwForceTimer += 1.0f;
                }
            }
        }
    }
    private void GrabObject()
    {
        Vector3 boxCenter = transform.position + transform.forward * (interactRange * 0.5f);
        Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, interactRange * 0.5f);
        Quaternion boxRotation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation);

        foreach (var hit in hits)
        {

            if (hit.attachedRigidbody != null && hit.CompareTag("Grabbable"))
            {         
                grabbedObject = hit.attachedRigidbody;
                Vector3 hitPoint = hit.ClosestPoint(boxCenter);

                initialGrabPointRelative = hitPoint - grabbedObject.transform.position;
                grabPoint = hitPoint;
                grabbing = true;
                break;
            }
        }
    }

    private void ReleaseObject()
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
            // Recalculate the current target world position
            Vector3 desiredWorldPosition = transform.position + transform.forward * interactRange;

            // Move to match the original offset on the object (for realism)
            targetPosition = desiredWorldPosition - initialGrabPointRelative;
            globalGrabPoint = targetPosition;
            Vector3 direction = targetPosition - grabbedObject.transform.position;

            // Apply velocity toward the target
            grabbedObject.linearVelocity = direction * grabForce;
        }
    }
    public void DoGrabObject(InputAction.CallbackContext obj) 
    {
        GrabObject();    
    }

    public void DoReleaseObject(InputAction.CallbackContext obj) 
    {
        ReleaseObject();
    }

    public void DoThrow(InputAction.CallbackContext obj) 
    {
        if (grabbedObject != null)
        {
            
            Rigidbody releasedObject = grabbedObject;
            ReleaseObject();
            releasedObject.AddForce(transform.forward * throwForce * throwForceTimer);
            
            throwForceTimer = 0.0f;
            chargingThrow = false;
        }
    }

    public void DoChargeThrow(InputAction.CallbackContext obj) 
    {
        chargingThrow = true;
    }

    public void DoPickUp() 
    {

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = transform.position + transform.forward * (interactRange * 0.5f);
        Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, interactRange * 0.5f);
        Quaternion boxRotation = transform.rotation;

        Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
}


