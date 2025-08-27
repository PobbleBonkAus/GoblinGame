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
    [SerializeField] private float grabbedObjectLinearDrag = 5.0f;
    [SerializeField] private float grabbedObjectAngularDrag = 5.0f;

    [Header("References")]
    [SerializeField] private GameObject kinematicBody;
    [SerializeField] private CosmeticHandler cosmeticHandler;
    [SerializeField] private PlayerController player;
    [SerializeField] private SphereCollider grabCollider;
    [SerializeField] private Transform playerRoot; // Main pivot of player body


    [Header("Throwing")]
    [SerializeField] public float maxThrowForceTime = 50.0f;
    [SerializeField] private float throwLockOutDuration = 1.0f;
    private float throwLockOutTime = 0.0f;
    [HideInInspector] public float throwForceTimer = 0.0f;
    private bool chargingThrow = false;

    [Header("Item Storing")]
    [SerializeField] private Transform storedItemTransform;
    [SerializeField] private float dropItemDistance = 1.0f;
    private Rigidbody storedItem = null;


    private Rigidbody grabbedObject;
    private Vector3 initialGrabPointRelative; // local point on object
    private Vector3 targetPosition;
    private Vector3 grabOffsetFromPlayer; // relative position from player root
    
    public bool grabPressed = false;
    public bool grabbing = false;

    private float grabbedObjectOriginalLinearDrag = 0.0f;
    private float grabbedObjectOriginalAngularDrag = 0.0f;

    [Header("Finesse Mode")]
    [HideInInspector] public bool finesseMode = false;
    [SerializeField] private float finesseModeSpeed = 4.0f;
    [SerializeField] private float finesseModeMaxDistance = 2.0f;
    [HideInInspector] public InputAction finesseModeInput;
    private Vector3 finesseOffset = Vector3.zero;



    void FixedUpdate()
    {
        globalGrabPoint = transform.position;
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
        grabOffsetFromPlayer = playerRoot.InverseTransformPoint(hitPoint);

        grabPoint = hitPoint;
        grabbing = true;

        grabbedObjectOriginalLinearDrag = grabbedObject.linearDamping;
        grabbedObjectOriginalAngularDrag = grabbedObject.angularDamping;

        grabbedObject.linearDamping = grabbedObjectLinearDrag;
        grabbedObject.angularDamping = grabbedObjectAngularDrag;

        grabbedObject.gameObject.layer = LayerMask.NameToLayer("GrabbedObject");
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
            kinematicBody.SetActive(false);
            finesseOffset = Vector3.zero;

            finesseMode = false;
        }
    }

    private void MoveGrabbedObject()
    {
        if (grabbedObject != null)
        {
            // Target position based on original offset to player root
            Vector3 desiredWorldPosition = playerRoot.TransformPoint(grabOffsetFromPlayer);

            // Maintain grab point on object
            targetPosition = desiredWorldPosition - grabbedObject.transform.TransformVector(initialGrabPointRelative) + finesseOffset;


            if (finesseMode) 
            {

                Vector3 finesseInput = new Vector3(finesseModeInput.ReadValue<Vector2>().x, finesseModeInput.ReadValue<Vector2>().y, 0.0f);
                Vector3 localOffset =
                      player.transform.right * finesseInput.x   // left/right
                    + player.transform.up * finesseInput.y;  // up/down

                finesseOffset += localOffset * finesseModeSpeed;
                


                if (finesseOffset.magnitude > finesseModeMaxDistance) finesseOffset = finesseOffset.normalized * finesseModeMaxDistance;

            }



            Vector3 direction = targetPosition - grabbedObject.transform.position;

            globalGrabPoint = grabbedObject.position - grabbedObject.transform.TransformVector(initialGrabPointRelative) + finesseOffset;

            if (direction.sqrMagnitude > minGrabMoveDistance * minGrabMoveDistance)
            {
                grabbedObject.AddForce(direction * grabForce,ForceMode.Force);
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
            else if (grabbedObject != null)
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

        player.playerGrab = true;
    }

    public void DoReleaseObject(InputAction.CallbackContext obj)
    {
        ReleaseObject();
        grabPressed = false;

    }

    public void DoThrow(InputAction.CallbackContext obj)
    {
        if (grabbedObject != null)
        {
            Rigidbody releasedObject = grabbedObject;
            ReleaseObject();
            releasedObject.AddForce(transform.forward * throwForce * throwForceTimer);

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

    public void DoToggleFinesse(InputAction.CallbackContext obj) 
    {
        if (grabbing) 
        {
            if (finesseMode) 
            {
                grabbedObject.linearDamping = 3.0f;
                finesseMode = false;
            }
            else
            {
                grabbedObject.linearDamping = 30.0f;
                finesseMode = true;
            }

        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!grabbing && grabPressed && other.attachedRigidbody != null && throwLockOutTime < 0.0f)
        {
            GrabObject(other.attachedRigidbody, other.ClosestPoint(transform.position));
        }
    }
}
