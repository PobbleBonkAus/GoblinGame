using UnityEngine;

public class playerProceduralAnimator : MonoBehaviour
{
    [SerializeField] float stepDistance = 3.0f;
    [SerializeField] float stepHeight = 0.2f;
    [SerializeField] float stepSpeed = 4.0f;
    [SerializeField] float stepWaitTime = 0.5f;
    [SerializeField] float velocityFactor = 0.4f;

    [SerializeField] Transform body;
    [SerializeField] Transform leftFoot;
    [SerializeField] Transform rightFoot;

    [SerializeField] LineRenderer leftLegRenderer;
    [SerializeField] LineRenderer rightLegRenderer;

    [SerializeField] private LayerMask playerMask; // Only used to exclude self

    [SerializeField] private RigidbodyPlayerController player;

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

        leftFootPreviousPosition = leftFoot.position;
        rightFootPreviousPosition = rightFoot.position;

        leftFootTargetPosition = leftFoot.position;
        rightFootTargetPosition = rightFoot.position;
    }

    private void Update()
    {
        if (player.IsGrounded()) 
        {
            UpdateFootTargetPositions();
            Step();
        }
        else
        {
            TuckLegs();
        }

        DrawLegs();
        stepWaitTimer += Time.deltaTime;
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

        leftFoot.position = Vector3.Lerp(leftFoot.position, leftLegRenderer.transform.position, 0.6f);
        leftFootPreviousPosition = leftFoot.position;
        leftFootTargetPosition = leftFoot.position;
        rightFoot.position = Vector3.Lerp(rightFoot.position, rightLegRenderer.transform.position, 0.6f);
        rightFootPreviousPosition = rightFoot.position;
        rightFootTargetPosition = rightFoot.position;
    }

    private void DrawLegs()
    {
        leftLegRenderer.SetPosition(0, leftLegRenderer.transform.position);
        leftLegRenderer.SetPosition(1, leftFoot.position);

        rightLegRenderer.SetPosition(0, rightLegRenderer.transform.position);
        rightLegRenderer.SetPosition(1, rightFoot.position);
    }
}
