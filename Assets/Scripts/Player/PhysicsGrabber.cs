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

    [Header("References")]
    [SerializeField] private GameObject kinematicBody;
    [SerializeField] private CosmeticHandler cosmeticHandler;
    [SerializeField] private RigidbodyPlayerController player;
    [SerializeField] private SphereCollider grabCollider;
    [SerializeField] private Transform playerRoot; // Main pivot of player body

    private Rigidbody grabbedObject;
    public bool grabbing = false;
    private Vector3 initialGrabPointRelative; // local point on object
    private Vector3 targetPosition;
    private Vector3 grabOffsetFromPlayer; // relative position from player root
    public bool grabPressed = false;

    [Header("Throwing")]
    [SerializeField] private float maxThrowForceTime = 50.0f;
    private float throwForceTimer = 0.0f;
    private bool chargingThrow = false;

    [Header("Item Storing")]
    [SerializeField] private Transform storedItemTransform;
    [SerializeField] private float dropItemDistance = 1.0f;
    private Rigidbody storedItem = null;

    void FixedUpdate()
    {
        globalGrabPoint = transform.position + (transform.forward * 1.0f) - (transform.up * 0.2f);

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

        // Store object offset relative to player root
        grabOffsetFromPlayer = playerRoot.InverseTransformPoint(grabbedObject.position);
        grabPoint = hitPoint;
        grabbing = true;

        grabbedObject.gameObject.layer = LayerMask.NameToLayer("GrabbedObject");
        kinematicBody.SetActive(true);
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
            // Target position based on original offset to player root
            Vector3 desiredWorldPosition = playerRoot.TransformPoint(grabOffsetFromPlayer);

            // Maintain grab point on object
            targetPosition = desiredWorldPosition - grabbedObject.transform.TransformVector(initialGrabPointRelative);

            Vector3 direction = targetPosition - grabbedObject.transform.position;

            if (direction.sqrMagnitude > minGrabMoveDistance * minGrabMoveDistance)
            {
                grabbedObject.linearVelocity = direction * grabForce;
            }

            // Set rotation to match player forward
            Quaternion targetRotation = Quaternion.LookRotation(playerRoot.forward);
            grabbedObject.MoveRotation(targetRotation);

            // Update global grab point for visuals
            globalGrabPoint = grabbedObject.transform.TransformPoint(initialGrabPointRelative);
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
        if (storedItem != null)
        {
            DropStoredItem();
        }
        else
        {
            StoreGrabbedItem();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!grabbing && grabPressed && other.attachedRigidbody != null)
        {
            GrabObject(other.attachedRigidbody, other.ClosestPoint(transform.position));
        }
    }
}
