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
        Ray ray = new Ray(interactRayOrigin.transform.position, interactRayOrigin.transform.forward * interactRange);
       
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.rigidbody != null)
            {
                if (!hit.rigidbody.CompareTag("Grabbable")) return;

                grabbedObject = hit.rigidbody;
                initialGrabPointRelative = hit.point - grabbedObject.transform.position;
                grabPoint.position = hit.point;
                grabbing = true;
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
            targetPosition = grabPoint.position - initialGrabPointRelative;
            Vector3 direction = targetPosition - grabbedObject.transform.position;
            grabbedObject.linearVelocity = direction * grabForce / Mathf.Max(1.0f, grabbedObject.mass);
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
        Gizmos.DrawRay(interactRayOrigin.transform.position, interactRayOrigin.transform.forward * interactRange);
    }
}


