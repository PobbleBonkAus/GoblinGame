using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PhysicsGrabber : MonoBehaviour
{
    private Vector3 grabPoint;
    public Vector3 globalGrabPoint;

    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float grabForce = 10f;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float useItemTimeoutDuration = 0.3f;
    [SerializeField] private float minGrabMoveDistance = 0.2f;

    [Header("references")]
    [SerializeField] GameObject kinematicBody;
    [SerializeField] CosmeticHandler cosmeticHandler;

    private Rigidbody grabbedObject;
    public bool grabbing = false;
    private Vector3 initialGrabPointRelative;
    private Vector3 targetPosition;


    [Header("Throwing")]
    [SerializeField] private float maxThrowForceTime = 50.0f;
    private float throwForceTimer = 0.0f;
    private bool chargingThrow = false;

    [Header("ItemStoring")]
    [SerializeField] Transform storedItemTransform;
    [SerializeField] float dropItemDistance = 1.0f;
    Rigidbody storedItem = null;

    void FixedUpdate()
    {
        if (grabbing)
        {
            MoveGrabbedObject();

            if (chargingThrow) 
            {
                if (throwForceTimer < maxThrowForceTime)
                {
                    initialGrabPointRelative = Vector3.Lerp(initialGrabPointRelative,transform.forward,throwForceTimer * 0.006f);
                    throwForceTimer += 1.0f;
                }
            }
        }

        if(storedItem != null) 
        {
            storedItem.transform.SetPositionAndRotation(storedItemTransform.position, storedItemTransform.rotation);
        }
    }
    private void GrabObject()
    {
        if (!isActiveAndEnabled) return;
        
        Vector3 boxCenter = transform.position + transform.forward * (interactRange * 0.5f);
        Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, interactRange * 0.5f);
        Quaternion boxRotation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation);

        foreach (var hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.gameObject.layer == LayerMask.NameToLayer("Grabbable")) 
            {         
                grabbedObject = hit.attachedRigidbody;
                Vector3 hitPoint = hit.ClosestPoint(boxCenter);

                initialGrabPointRelative = hitPoint - grabbedObject.transform.position;
                grabPoint = hitPoint;
                grabbing = true;

                grabbedObject.gameObject.layer = LayerMask.NameToLayer("GrabbedObject");
                kinematicBody.SetActive(true);
                break;
            }
        }
    }

    private void ReleaseObject()
    {
        if (grabbing)
        {
            grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbable");
            grabbedObject = null;
            grabbing = false;
            kinematicBody.SetActive(false);
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

            if (Vector3.Distance(targetPosition, desiredWorldPosition) > minGrabMoveDistance) 
            {
                // Apply velocity toward the target
                grabbedObject.linearVelocity = direction * grabForce;
            }

        }
    }

    public void StoreGrabbedItem() 
    {
        if (grabbedObject.CompareTag("Cosmetic")) 
        {
            cosmeticHandler.TryEquipCosmetic(grabbedObject.gameObject);
        }
        else
        {
            if (storedItem)
            {
                DropStoredItem();
            }
            else if (grabbedObject != null)
            {
                storedItem = grabbedObject;
                ReleaseObject();
                storedItem.gameObject.layer = LayerMask.NameToLayer("StoredObject");
                storedItem.transform.SetPositionAndRotation(storedItemTransform.position, storedItemTransform.rotation);
                storedItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    public void DropStoredItem() 
    {
       // storedItem.position = storedItemTransform.position + storedItemTransform.forward * dropItemDistance;
        storedItem.gameObject.layer = LayerMask.NameToLayer("Grabbable");
        storedItem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        storedItem = null;
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

    public void DoPickUp(InputAction.CallbackContext obj) 
    {
        if(storedItem != null)
        {
            DropStoredItem();
        }
        else
        {
            StoreGrabbedItem();
        }
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


