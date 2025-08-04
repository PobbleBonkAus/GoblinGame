using UnityEngine;

public class playerProceduralAnimator : MonoBehaviour
{
    [Header("Head params")]

    [SerializeField] Transform camera;
    [SerializeField] Transform headTransform;
    [SerializeField] private float headLerpSpeed;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] private float pitchOffset = 0f; // positive = look more upwards

    [Header("Leg params")]
    [SerializeField] float stepDistance = 3.0f;
    [SerializeField] float stepHeight = 0.2f;
    [SerializeField] float stepSpeed = 4.0f;
    [SerializeField] float stepWaitTime = 0.5f;
    [SerializeField] float velocityFactor = 0.4f;
    [SerializeField] float kneeExtension = 0.2f;
    [SerializeField] Transform leftFoot;
    [SerializeField] Transform rightFoot;
    [SerializeField] LineRenderer leftLegRenderer;
    [SerializeField] LineRenderer rightLegRenderer;

    [Header("Body params")]
    [SerializeField] Transform body;
    [SerializeField] private LayerMask playerMask; // Only used to exclude self
    [SerializeField] private RigidbodyPlayerController player;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float playerBobOffset = 0.7f;
    [SerializeField] private float playerBobAmplitude = 0.1f;
    [SerializeField] private float playerHeadOffset = 0.9f;
    [SerializeField] private float playerHeadAmplitude = 0.05f;
    
    [Header("Arm params")]
    [SerializeField] private Transform grabbedBody;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private LineRenderer leftArmRenderer;
    [SerializeField] private LineRenderer rightArmRenderer;
    [SerializeField] private float armLerpSpeed = 0.3f;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    [Header("Grabbing params")]
    [SerializeField] PhysicsGrabber physicsGrabber;

    private Vector3 grabbedPoint;
    private Vector3 leftFootPreviousPosition;
    private Vector3 rightFootPreviousPosition;

    private Vector3 leftFootTargetPosition;
    private Vector3 rightFootTargetPosition;

    private float leftFootLerp = 1f;
    private float rightFootLerp = 1f;

    private bool stepLeftFoot = true;

    private int playerLayer;
    private int raycastMask;

    private float stepWaitTimer = 0.0f;


    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        raycastMask = ~(1 << playerLayer); // Everything except the Player layer
    }

    private void Start()
    {

        // Detach feet so they stay in world space
        leftFoot.SetParent(null);
        rightFoot.SetParent(null);

        leftHand.SetParent(null);
        rightHand.SetParent(null);

        leftHandTarget.position = leftHand.position;
        rightHandTarget.position = rightHand.position;

        leftFootPreviousPosition = leftFoot.position;
        rightFootPreviousPosition = rightFoot.position;

        leftFootTargetPosition = leftFoot.position;
        rightFootTargetPosition = rightFoot.position;
    }

    private void Update()
    {
        if (player.IsGrounded()  && !player.isRagdolled) 
        {
            UpdateFootTargetPositions();
            Step();
            BobPlayerWithLegs();
        }
        else
        {
            TuckLegs();
        }

        UpdateArmTargetPositions();
        RotateHeadWithCamera();

        DrawLegs();
        DrawArms();
        stepWaitTimer += Time.deltaTime;
    }

    private void BobPlayerWithLegs() 
    {
        playerBody.transform.position = new Vector3(transform.position.x, 
            transform.position.y + playerBobOffset + (Vector3.Distance(leftFoot.position, rightFoot.position) * playerBobAmplitude),
            transform.position.z);

        headTransform.position = new Vector3(transform.position.x,
        transform.position.y + playerHeadOffset + (Vector3.Distance(leftFoot.position, rightFoot.position) * playerHeadAmplitude),
        transform.position.z);
    }

    private void UpdateFootTargetPositions()
    {
        Vector3 velocity = body.GetComponent<Rigidbody>().linearVelocity;
        Vector3 offset = velocity * velocityFactor;

        // Step left foot
        if (stepLeftFoot && leftFootLerp >= 1f && stepWaitTimer >= stepWaitTime)
        {
            if (Vector3.Distance(leftFoot.position, leftLegRenderer.transform.position) > stepDistance)
            {
                if (TryGetFootTarget(leftLegRenderer.transform.position,offset, out Vector3 target))
                {
                    leftFootPreviousPosition = leftFoot.position;
                    leftFootTargetPosition = target;
                    leftFootLerp = 0f;
                    stepLeftFoot = false;
                    stepWaitTimer = 0f;
                }
                else
                {
                    leftFootPreviousPosition = leftFoot.position;
                    leftFootTargetPosition = leftLegRenderer.transform.position;
                    leftFootLerp = 0f;
                    stepLeftFoot = false;
                    stepWaitTimer = 0f;
                }
            }
        }
        // Step right foot
        else if (!stepLeftFoot && rightFootLerp >= 1f && stepWaitTimer >= stepWaitTime)
        {
            if (Vector3.Distance(rightFoot.position, rightLegRenderer.transform.position) > stepDistance)
            {
                if (TryGetFootTarget(rightLegRenderer.transform.position,offset, out Vector3 target))
                {
                    rightFootPreviousPosition = rightFoot.position;
                    rightFootTargetPosition = target;
                    rightFootLerp = 0f;
                    stepLeftFoot = true;
                    stepWaitTimer = 0f;
                }
                else
                {
                    rightFootPreviousPosition = rightFoot.position;
                    rightFootTargetPosition = rightLegRenderer.transform.position;
                    rightFootLerp = 0f;
                    stepLeftFoot = true;
                    stepWaitTimer = 0f;
                }
            }
        }
    }

    private void UpdateArmTargetPositions() 
    {
        if (physicsGrabber.grabbing) 
        {
            leftHand.position = Vector3.Lerp(leftHand.position, physicsGrabber.globalGrabPoint, armLerpSpeed);
            rightHand.position = Vector3.Lerp(rightHand.position, physicsGrabber.globalGrabPoint, armLerpSpeed);
        }
        else
        {
            leftHand.position = Vector3.Lerp(leftHand.position, leftHandTarget.position, armLerpSpeed);
            rightHand.position = Vector3.Lerp(rightHand.position, rightHandTarget.position, armLerpSpeed);
        }
    }

    private void RotateHeadWithCamera()
    {
        // Camera direction
        Vector3 cameraDir = camera.forward;

        // Calculate horizontal (Y axis) angle
        Vector3 flatCameraForward = cameraDir;
        flatCameraForward.y = 0;
        flatCameraForward.Normalize();

        Vector3 flatHeadForward = playerBody.forward;
        flatHeadForward.y = 0;
        flatHeadForward.Normalize();

        float yawAngle = Vector3.SignedAngle(flatHeadForward, flatCameraForward, Vector3.up);

        // Calculate vertical (X axis) angle with pitch offset
        Vector3 headLocalForward = playerBody.InverseTransformDirection(cameraDir);
        float pitchAngle = Mathf.Atan2(headLocalForward.y, headLocalForward.z) * Mathf.Rad2Deg;

        // Apply user-defined offset
        pitchAngle += pitchOffset;

        // Clamp pitch
        float clampedPitch = Mathf.Clamp(pitchAngle, -30f, 30f);

        if (yawAngle > 90f || yawAngle < -90f) 
        {
            yawAngle = 0f;
            clampedPitch = 0f;
        }

        float clampedYaw = Mathf.Clamp(yawAngle, -90f, 90f);


        // Final head rotation in local space (pitch X, yaw Y, no roll)
        Quaternion targetRotation = Quaternion.Euler(-clampedPitch, clampedYaw, 0f);
        Quaternion finalRotation = Quaternion.Lerp(headTransform.localRotation, targetRotation, Time.deltaTime * headLerpSpeed);

        headTransform.localRotation = finalRotation;
    }

    private bool TryGetFootTarget(Vector3 origin, Vector3 velocityOffset,out Vector3 hitPoint)
    {
        Vector3 rayStart = origin + velocityOffset;
        Vector3 direction = Vector3.down;

        if (Physics.Raycast(rayStart, direction, out RaycastHit hit, 0.5f, raycastMask))
        {
            hitPoint = hit.point;
            return true;
        }
        Debug.Log("did not hit anything");
        hitPoint = origin - Vector3.up * 1f;
        return false;
    }

    private void Step()
    {
        if (leftFootLerp < 1f)
        {
            leftFootLerp += Time.deltaTime * stepSpeed;
            Vector3 mid = (leftFootPreviousPosition + leftFootTargetPosition) / 2 + Vector3.up * stepHeight;
            leftFoot.position = Vector3.Lerp(Vector3.Lerp(leftFootPreviousPosition, mid, leftFootLerp),
                                             Vector3.Lerp(mid, leftFootTargetPosition, leftFootLerp),
                                             leftFootLerp);
        }

        if (rightFootLerp < 1f)
        {
            rightFootLerp += Time.deltaTime * stepSpeed;
            Vector3 mid = (rightFootPreviousPosition + rightFootTargetPosition) / 2 + Vector3.up * stepHeight;
            rightFoot.position = Vector3.Lerp(Vector3.Lerp(rightFootPreviousPosition, mid, rightFootLerp),
                                              Vector3.Lerp(mid, rightFootTargetPosition, rightFootLerp),
                                              rightFootLerp);
        }
    }

    private void TuckLegs() 
    {
        leftFootLerp = 0;
        rightFootLerp = 0;

        Vector3 tuckOffset = transform.up * -0.25f;

        leftFoot.position = Vector3.Lerp(leftFoot.position, leftLegRenderer.transform.position + tuckOffset, 0.6f);
        leftFootPreviousPosition = leftFoot.position;
        leftFootTargetPosition = leftFoot.position;
        rightFoot.position = Vector3.Lerp(rightFoot.position, rightLegRenderer.transform.position + tuckOffset, 0.6f);
        rightFootPreviousPosition = rightFoot.position;
        rightFootTargetPosition = rightFoot.position;
    }

    private void DrawLegs()
    {
        leftLegRenderer.SetPosition(0, leftLegRenderer.transform.position);
        leftLegRenderer.SetPosition(1, ((leftLegRenderer.transform.position + leftFoot.position)/2.0f) 
            + leftLegRenderer.transform.forward * kneeExtension);
        leftLegRenderer.SetPosition(2, leftFoot.position);

        rightLegRenderer.SetPosition(0, rightLegRenderer.transform.position);
        rightLegRenderer.SetPosition(1, ((rightLegRenderer.transform.position + rightFoot.position) / 2.0f) 
            + rightLegRenderer.transform.forward * kneeExtension);
        rightLegRenderer.SetPosition(2, rightFoot.position);
    }

    private void DrawArms()
    {
        rightArmRenderer.SetPosition(0, rightArmRenderer.transform.position);
        rightArmRenderer.SetPosition(1, rightHand.position);

        leftArmRenderer.SetPosition(0, leftArmRenderer.transform.position);
        leftArmRenderer.SetPosition(1, leftHand.position);
    }

}
