using System.Collections.Generic;
using UnityEngine;

public class playerProceduralAnimator : MonoBehaviour
{
    [Header("Head params")]

    [SerializeField] Transform camera;
    [SerializeField] Transform headTransform;
    [SerializeField] private float headLerpSpeed;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] private float maxHeadTurnAngle = 60f; // degrees from forward
    [SerializeField] private float behindThreshold = 120f; // when camera is too far back
    public Transform headLookTarget;

    [Header("Leg params")]
    [SerializeField] float stepDistance = 3.0f;
    [SerializeField] float stepHeight = 0.2f;
    [SerializeField] float groundCheckDistance = 1.0f;
    [SerializeField] float footPlacementOffset = 0.2f;
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
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float stretchAndSquish = 1.0f;

    [Header("Bob")]
    [SerializeField] private Transform headBobTransform;
    [SerializeField] private Transform bodyBobTransform;
    [SerializeField] private float playerBobOffset = 0.7f;
    [SerializeField] private float playerBobAmplitude = 0.1f;

    [SerializeField] private float playerHeadOffset = 0.9f;
    [SerializeField] private float playerHeadAmplitude = 0.05f;

    [Header("Ears")]
    [SerializeField] Transform leftEar;
    [SerializeField] Transform rightEar;
    [SerializeField] float ear_maxZRotation = 30.0f;
    [SerializeField] float ear_minZRotation = -20.0f;
    [SerializeField] float ear_maxYRotation = 30.0f;
    [SerializeField] float ear_minYRotation = -20.0f;
    private Vector3 leftEarVelocity;
    private Vector3 rightEarVelocity;

    [Header("Arm params")]
    [SerializeField] private Transform grabbedBody;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private LineRenderer leftArmRenderer;
    [SerializeField] private LineRenderer rightArmRenderer;
    [SerializeField] private float armLerpSpeed = 0.3f;
    [SerializeField] private float armMaxStretch = 3.0f;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    [SerializeField] float damping = 5f;
    [SerializeField] float stiffness = 50f;

    private Vector3 leftHandVel;
    private Vector3 rightHandVel;
    private Vector3 headVel;

    [Header("Grabbing params")]
    [SerializeField] PhysicsGrabber physicsGrabber;

    [Header("Particle Effects")]
    [SerializeField] ParticleSystem leftFootDust;
    [SerializeField] ParticleSystem rightFootDust;
    [SerializeField] ParticleSystem clapEffect;

    [Header("Audio")]
    [SerializeField] AudioClip footstep;
    [SerializeField] AudioClip clap;

    private Vector3 grabbedPoint;

    private Vector3 leftFootPreviousPosition;
    private Vector3 rightFootPreviousPosition;

    private Vector3 leftFootTargetPosition;
    private Vector3 rightFootTargetPosition;

    private Vector3 leftFootStandingPosition;
    private Vector3 rightFootStandingPosition;

    private float leftFootLerp = 1f;
    private float rightFootLerp = 1f;

    private bool stepLeftFoot = true;

    private int playerLayer;
    private int raycastMask;

    private float stepWaitTimer = 0.0f;

    private float velocityMagnitude;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        raycastMask = ~(1 << playerLayer); // Everything except the Player layer

    }

    private void Start()
    {

        StartCoroutine(LateStart());
    }

    IEnumerator<WaitForSeconds> LateStart() 
    {
        yield return new WaitForSeconds(0.5f);

        leftHand.SetParent(null);
        rightHand.SetParent(null);

        leftHandTarget.position = leftHand.position;
        rightHandTarget.position = rightHand.position;

        leftHand.position = body.position;
        rightHand.position = body.position;

        // Detach feet so they stay in world space
        leftFoot.SetParent(null);
        rightFoot.SetParent(null);

        leftFootPreviousPosition = leftFoot.position;
        rightFootPreviousPosition = rightFoot.position;

        leftFootTargetPosition = leftFoot.position;
        rightFootTargetPosition = rightFoot.position;

    }

    private void Update()
    {
        if (player.IsGrounded() && !player.isRagdolled) 
        {
            UpdateFootTargetPositions();
            Step();
        }
        else
        {
            TuckLegs();
        }

        BobPlayerWithLegs();
        BobbleHead();
        UpdateArmTargetPositions();

        RotateHead();
        


        DrawLegs();
        DrawArms();
        stepWaitTimer += Time.deltaTime;

        WobbleEars();

    }
    private void RotateHead()
    {
        Vector3 targetDir = camera.forward;

        if (headLookTarget)
        {
            targetDir = (headLookTarget.position - headTransform.position).normalized;
        }

        if (physicsGrabber.grabbing)
        {
            targetDir = (physicsGrabber.globalGrabPoint - headTransform.position).normalized;
        }

        if (player.isRagdolled) 
        {
            targetDir = body.forward;
        }

        Quaternion toRotation = Quaternion.LookRotation(targetDir, body.transform.up);
        headTransform.rotation = Quaternion.Slerp(headTransform.rotation, toRotation, headLerpSpeed * Time.time);

        headTransform.transform.localEulerAngles = new Vector3(Mathf.Clamp(-Mathf.DeltaAngle(headTransform.transform.localEulerAngles.x, 0), -30, 30), headTransform.transform.localEulerAngles.y, 0); // clamp angle x -30 to 30
        headTransform.transform.localEulerAngles = new Vector3(headTransform.transform.localEulerAngles.x, Mathf.Clamp(-Mathf.DeltaAngle(headTransform.transform.localEulerAngles.y, 0), -50, 50), 0);  // clamp angle y -50 to 50

    }

    IEnumerator<WaitForSeconds> LooseInterestInHeadLookTarget() 
    {
        yield return new WaitForSeconds(3.0f);
        headLookTarget = null;
    }

    private void WobbleEars() 
    {
        //based on head velocity, wobble the ears with just rotation
        Vector3 headVelocity = playerRigidbody.linearVelocity;
        float velocitySlerp = headVelocity.magnitude;

        Vector3 leftEarRotationTarget = Quaternion.Euler(0.0f, headVelocity.z, -headVelocity.y).eulerAngles;
        Vector3 rightEarRotationTarget = Quaternion.Euler(0.0f, headVelocity.z, headVelocity.y).eulerAngles;

        leftEar.transform.localRotation = Quaternion.Slerp(leftEar.transform.localRotation, 
            Quaternion.Euler(leftEarRotationTarget.x, leftEarRotationTarget.y, leftEarRotationTarget.z),
            1.0f / velocitySlerp);
        rightEar.transform.localRotation = Quaternion.Slerp(rightEar.transform.localRotation,
        Quaternion.Euler(rightEarRotationTarget.x, rightEarRotationTarget.y, rightEarRotationTarget.z),
        1.0f / velocitySlerp);
    }
    private void BobPlayerWithLegs()
    {
        float footDistance = Vector3.Distance(leftFoot.position, rightFoot.position);
        float headBobAmount = playerHeadAmplitude * footDistance;
        Vector3 headBobOffset = transform.up * (playerHeadOffset + headBobAmount);

        headBobTransform.position = playerRigidbody.transform.position + headBobOffset;

        float bobAmount = playerBobAmplitude * footDistance;
        Vector3 bobOffset = transform.up * (playerBobOffset + bobAmount);

        bodyBobTransform.position = playerRigidbody.transform.position + bobOffset;
    }


    private void UpdateFootTargetPositions()
    {
        Vector3 velocity = body.GetComponent<Rigidbody>().linearVelocity;
        Vector3 offset = velocity * velocityFactor;
        velocityMagnitude = velocity.magnitude;

        if(playerRigidbody.linearVelocity.magnitude < 0.4f) 
        {
            if (TryGetFootTarget(leftLegRenderer.transform.position, offset, out Vector3 target))
            {
                //stomp
                leftFootPreviousPosition = leftFoot.position;
                leftFootTargetPosition = target;
            }
            if (TryGetFootTarget(rightLegRenderer.transform.position, offset, out Vector3 target2))
            {
                //stomp
                rightFootPreviousPosition = rightFoot.position;
                rightFootTargetPosition = target2;
            }

            return;
        }

        // Step left foot
        if (stepLeftFoot && leftFootLerp >= 1f && stepWaitTimer >= stepWaitTime)
        {
            if (Vector3.Distance(leftFoot.position, leftLegRenderer.transform.position) > stepDistance)
            {
                if (TryGetFootTarget(leftLegRenderer.transform.position,offset, out Vector3 target))
                {
                    //stomp
                    leftFootPreviousPosition = leftFoot.position;
                    leftFootTargetPosition = target;
                    leftFootLerp = 0f;
                    stepLeftFoot = false;
                    stepWaitTimer = 0f;

                    AudioController.instance.PlayAudioClip(footstep, transform);
                    leftFootDust.Play();                    
                }
                else
                {
                    //in the air
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
                    //stomp
                    rightFootPreviousPosition = rightFoot.position;
                    rightFootTargetPosition = target;
                    rightFootLerp = 0f;
                    stepLeftFoot = true;
                    stepWaitTimer = 0f;

                    AudioController.instance.PlayAudioClip(footstep,transform);
                    rightFootDust.Play();
                }
                else
                {
                    //weeee we are in the air
                    rightFootPreviousPosition = rightFoot.position;
                    rightFootTargetPosition = rightLegRenderer.transform.position;
                    rightFootLerp = 0f;
                    stepLeftFoot = true;
                    stepWaitTimer = 0f;
                }
            }
        }
    }

    //updates the hand positions for the arm renderers
    private void UpdateArmTargetPositions() 
    {

        //If the player is holding down the grab button and is not ragdolled, move the hands according to the physics grabber
        if (player.isRagdolled)
        {
            //THis is the default position
            leftHand.position = SpringLerp(leftHand.position, leftHandTarget.position, ref leftHandVel);
            rightHand.position = SpringLerp(rightHand.position, rightHandTarget.position, ref rightHandVel);
            return;
        }

        if (physicsGrabber.grabbing)
        {   //move hands onto grab point on the grabbed object
            leftHand.position = Vector3.Lerp(leftHand.position, physicsGrabber.globalGrabPoint + (transform.right * 0.4f), armLerpSpeed);
            rightHand.position = Vector3.Lerp(rightHand.position, physicsGrabber.globalGrabPoint - (transform.right * 0.4f), armLerpSpeed);
        }
        else if(physicsGrabber.grabPressed)
        { 
            //move hands in front of the player to show the player is trying to grab
            leftHand.position = SpringLerp(leftHand.position, physicsGrabber.transform.position + (transform.right * 0.4f), ref leftHandVel);
            rightHand.position = SpringLerp(rightHand.position, physicsGrabber.transform.position - (transform.right * 0.4f), ref rightHandVel);
        }
        else if(physicsGrabber.raisePressed)
        {
            //move hands in above of the player to show the player is trying to raise
            leftHand.position = SpringLerp(leftHand.position, (transform.position + transform.up * 2.0f) + (transform.right * 0.75f) + (transform.forward * 0.1f), ref leftHandVel);
            rightHand.position = SpringLerp(rightHand.position, (transform.position + transform.up * 2.0f) - (transform.right * 0.75f) + (transform.forward * 0.1f), ref rightHandVel);
        }
        else
        {
            //THis is the default position
            leftHand.position = SpringLerp(leftHand.position, leftHandTarget.position, ref leftHandVel);
            rightHand.position = SpringLerp(rightHand.position, rightHandTarget.position, ref rightHandVel);

        }
    }
    
    private void BobbleHead() 
    {
        headTransform.position = SpringLerp(headTransform.position, headBobTransform.position, ref headVel);
    }
    
    private bool TryGetFootTarget(Vector3 origin, Vector3 velocityOffset,out Vector3 hitPoint)
    {
        Vector3 rayStart = origin + velocityOffset;
        Vector3 direction = Vector3.down;

        if (Physics.Raycast(rayStart, direction, out RaycastHit hit, groundCheckDistance, raycastMask))
        {
            hitPoint = hit.point - (Vector3.up * footPlacementOffset);
            return true;
        }
        hitPoint = origin - Vector3.up;
        return false;
    }

    private void Step()
    {
        if(playerRigidbody.linearVelocity.magnitude < 0.5f) 
        {
            leftFootLerp += Time.deltaTime * stepSpeed;

            Vector3 mid = (leftFootPreviousPosition + leftFootTargetPosition) / 2 + Vector3.up * stepHeight;
            leftFoot.position = Vector3.Lerp(Vector3.Lerp(leftFootPreviousPosition, mid, leftFootLerp),
                                                Vector3.Lerp(mid, leftFootTargetPosition, leftFootLerp),
                                                leftFootLerp);

            rightFootLerp += Time.deltaTime * stepSpeed;

            Vector3 Rmid = (rightFootPreviousPosition + rightFootTargetPosition) / 2 + Vector3.up * stepHeight;
            rightFoot.position = Vector3.Lerp(Vector3.Lerp(rightFootPreviousPosition, Rmid, rightFootLerp),
                                                Vector3.Lerp(Rmid, rightFootTargetPosition, rightFootLerp),
                                                rightFootLerp);
            return;
        }

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


    Vector3 SpringLerp(Vector3 current, Vector3 target, ref Vector3 velocity)
    {
        Vector3 displacement = target - current;
        Vector3 springForce = stiffness * displacement;
        Vector3 dampingForce = -damping * velocity;

        Vector3 acceleration = springForce + dampingForce;
        velocity += acceleration * Time.deltaTime;

        return current + velocity * Time.deltaTime;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody) return;
        if(other.attachedRigidbody.linearVelocity.magnitude > 1.0f || other.CompareTag("Player")) 
        {
            if (other.transform.IsChildOf(transform.parent)) return;
            headLookTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform == headLookTarget) 
        {
            headLookTarget = null;
        }
    }

}
