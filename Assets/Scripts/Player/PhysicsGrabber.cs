using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InteractableRigidbody;

public class PhysicsGrabber : MonoBehaviour
{
    private Vector3 grabPoint;
    public Vector3 globalGrabPoint;

    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float largeObjectGrabForce = 10f;
    [SerializeField] private float smallObjectGrabForce = 100.0f;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float useItemTimeoutDuration = 0.3f;
    [SerializeField] private float minGrabMoveDistance = 0.2f;
    [SerializeField] private float grabbedObjectLinearDrag = 5.0f;
    [SerializeField] private float grabbedObjectAngularDrag = 5.0f;
    [SerializeField] private float maxGrabObjectRange = 3.0f;
    [SerializeField] private float heavyObjectMultiplier = 0.4f;
    [SerializeField] private float springSnapBackForce = -0.4f;
    private float grabForce;

    [Header("References")]
    [SerializeField] private GameObject kinematicBody;
    [SerializeField] private CosmeticHandler cosmeticHandler;
    [SerializeField] private PlayerController player;
    [SerializeField] private SphereCollider grabCollider;
    [SerializeField] private Transform playerRoot; // Main pivot of player body
    [SerializeField] private Transform head;
    [SerializeField] private PlayerCameraController cameraController;
 
    [Header("Throwing")]
    [SerializeField] public float maxThrowForceTime = 50.0f;
    [SerializeField] private float throwLockOutDuration = 1.0f;
    [SerializeField] private float headColliderDeactivationTimeAfterThrow = 0.5f;
    [SerializeField] private float throwZoomTimeMultiplier = 3.0f;
    [SerializeField] private float throwZoomAmount = 3.0f;
    private float initialCameraFOV;

    private float throwLockOutTime = 0.0f;
    [HideInInspector] public float throwForceTimer = 0.0f;
    private bool chargingThrow = false;



    [Header("Item Storing")]
    [SerializeField] private Transform storedItemTransform;
    [SerializeField] private float dropItemDistance = 1.0f;
    private Rigidbody storedItem = null;

    [Header("Raising Objects")]
    [SerializeField] Transform raisedObjectTransform;
    [SerializeField] float raiseSpeed;

    private bool raisingObject;
    Vector3 raiseOffset = Vector3.zero;

    private Rigidbody grabbedObject;
    private ObjectType objectType;
    private Vector3 initialGrabPointRelative; // local point on object
    private Vector3 targetPosition;
    private Vector3 grabOffsetFromPlayer; // relative position from player root
    
    [HideInInspector] public bool raisePressed = false;
    [HideInInspector] public bool grabPressed = false;
    [HideInInspector] public bool grabbing = false;

    private float grabbedObjectOriginalLinearDrag = 0.0f;
    private float grabbedObjectOriginalAngularDrag = 0.0f;
    Rigidbody playerRigidbody;

    private void Awake()
    {
        playerRigidbody = player.GetComponent<Rigidbody>();
        grabForce = smallObjectGrabForce;
    }

    void FixedUpdate()
    {
        //globalGrabPoint = transform.position;
        throwLockOutTime -= Time.fixedDeltaTime;

        if (grabbing)
        {
            MoveGrabbedObject();

            if (chargingThrow)
            {
                if (throwForceTimer < maxThrowForceTime)
                {
                    initialGrabPointRelative = Vector3.Lerp(initialGrabPointRelative, transform.forward, throwForceTimer * 0.006f);
                    throwForceTimer += 1.0f;
                    cameraController.SetZoom(-throwForceTimer);
                }
            }

            if (player.isRagdolled)
            {
                ReleaseObject();
            }
        }

        if (storedItem != null)
        {
            storedItem.transform.SetPositionAndRotation(storedItemTransform.position, storedItemTransform.rotation);
        }
    }

    private void GrabObject(Rigidbody hitRigidbody, Vector3 hitPoint)
    {
        if (player.isRagdolled) return;

        grabbedObject = hitRigidbody;

        // Store grab point relative to object
        initialGrabPointRelative = grabbedObject.transform.InverseTransformPoint(hitPoint);
        // Store object offset relative to player root this is important for when we rotate the player around, the object
        // should orbit around the player, not the physics grab origin.
        grabOffsetFromPlayer = playerRoot.InverseTransformPoint(transform.position);

        grabPoint = hitPoint;
        grabbing = true;

        grabbedObjectOriginalLinearDrag = grabbedObject.linearDamping;
        grabbedObjectOriginalAngularDrag = grabbedObject.angularDamping;

        //grabbedObject.linearDamping = grabbedObjectLinearDrag;
        grabbedObject.angularDamping = grabbedObjectAngularDrag;

        grabbedObject.gameObject.layer = LayerMask.NameToLayer("GrabbedObject");
        objectType = grabbedObject.GetComponent<InteractableRigidbody>().type;
        kinematicBody.SetActive(true);
    }

    private void ReleaseObject()
    {
        if (grabbing)
        {
            grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbable");
            grabbedObject.linearDamping = grabbedObjectOriginalLinearDrag;
            grabbedObject.angularDamping = grabbedObjectOriginalAngularDrag;

            grabbedObject = null;
            grabbing = false;
            raisingObject = false;
            raisePressed = false;

            kinematicBody.SetActive(false);
        }
    }


    private void MoveGrabbedObject()
    {
        if (grabbedObject != null)
        {
            raiseOffset = (raisingObject ? raiseOffset = raisedObjectTransform.position : Vector3.zero);
            grabOffsetFromPlayer = (raisingObject) ? raisedObjectTransform.localPosition : grabOffsetFromPlayer = transform.localPosition;
            grabbedObject.linearDamping = (!player.IsGrounded()) ? 0.0f : grabbedObject.linearDamping = grabbedObjectLinearDrag;


            Vector3 desiredWorldPosition = playerRoot.TransformPoint(grabOffsetFromPlayer);

            // Maintain grab point on object
            targetPosition = desiredWorldPosition - grabbedObject.transform.TransformVector(initialGrabPointRelative);
            Vector3 direction = targetPosition - grabbedObject.transform.position;
            globalGrabPoint = grabbedObject.position - grabbedObject.transform.TransformVector(initialGrabPointRelative);



            if (direction.sqrMagnitude > minGrabMoveDistance * minGrabMoveDistance)
            {
                // Force to move object
                Vector3 pullForce = direction * grabForce;

                grabbedObject.AddForce(pullForce, ForceMode.Force);

                Vector3 oppositeForce = -pullForce * Mathf.Clamp01(grabbedObject.mass / 50f);
                playerRigidbody.AddForce(oppositeForce, ForceMode.Force);
                
            }


            RotateGrabObject();
        }
    }

    void RotateGrabObject()
    {
        // Current world-space position of the grab point on the object
        Vector3 currentGrabPoint = grabbedObject.transform.TransformPoint(initialGrabPointRelative);

        // Desired grab point in world space (where it should be relative to player)
        Vector3 desiredGrabPoint = playerRoot.TransformPoint(grabOffsetFromPlayer);

        // Get current and desired directions from object center to grab point
        Vector3 currentDir = (currentGrabPoint - grabbedObject.worldCenterOfMass).normalized;
        Vector3 desiredDir = (desiredGrabPoint - grabbedObject.worldCenterOfMass).normalized;

        // Find rotation difference to align currentDir to desiredDir
        Quaternion deltaRotation = Quaternion.FromToRotation(currentDir, desiredDir);

        // Convert to axis-angle for torque calculation
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f; // Keep angles small for stability

        // Apply torque proportional to angle
        Vector3 torque = axis.normalized * (angle * Mathf.Deg2Rad * grabForce);
        grabbedObject.AddTorque(torque, ForceMode.Force);

        // Update global grab point for visuals
        globalGrabPoint = currentGrabPoint;
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
            else if (grabbedObject != null && objectType != ObjectType.LARGE)
            {
                storedItem = grabbedObject;
                ReleaseObject();
                storedItem.gameObject.layer = LayerMask.NameToLayer("StoredObject");
                storedItem.transform.SetPositionAndRotation(storedItemTransform.position, storedItemTransform.rotation);
                storedItem.isKinematic = true;
            }
        }
    }

    public void DropStoredItem()
    {
        storedItem.gameObject.layer = LayerMask.NameToLayer("Grabbable");
        storedItem.isKinematic = false;
        storedItem = null;
    }

    public void DoGrabObject(InputAction.CallbackContext obj)
    {
        grabPressed = true;
    }

    public void DoReleaseObject(InputAction.CallbackContext obj)
    {
        ReleaseObject();
        grabPressed = false;

    }

    public void DoRaiseObject(InputAction.CallbackContext obj) 
    {
        raisingObject = true;
        raisePressed = true;
    }

    public void DoLowerObject(InputAction.CallbackContext obj) 
    {
        raisingObject = false;
        raisePressed = false;
    }
    

    public void DoThrow(InputAction.CallbackContext obj)
    {
        if (grabbedObject != null)
        {
            StopCoroutine(EnableHeadCollider());
            StartCoroutine(EnableHeadCollider());
            
            Rigidbody releasedObject = grabbedObject;
            ReleaseObject();
            releasedObject.AddForce(((transform.forward) + (Vector3.up/2.0f)) * throwForce * throwForceTimer);

            cameraController.SetZoom(0.0f);


            throwLockOutTime = throwLockOutDuration;
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
        if (storedItem != null)
        {
            DropStoredItem();
        }
        else
        {
            StoreGrabbedItem();
        }
    }

    IEnumerator<WaitForSeconds> EnableHeadCollider() 
    {
        head.GetComponent<MeshCollider>().enabled = false;
       
        yield return new WaitForSeconds(headColliderDeactivationTimeAfterThrow);

        head.GetComponent<MeshCollider>().enabled = true;
    }


    private void OnTriggerStay(Collider other)
    {
        if (!grabbing && grabPressed && other.attachedRigidbody != null && throwLockOutTime < 0.0f)
        {
            GrabObject(other.attachedRigidbody, other.ClosestPoint(transform.position));
        }
    }
}
