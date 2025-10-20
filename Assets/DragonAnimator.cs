using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DragonAnimator : MonoBehaviour
{
    [SerializeField] Transform bodyPivot;
    [SerializeField] Transform headPivot;
    [SerializeField] Transform tailPivot;

    [SerializeField] MultiAimConstraint headAim;
    [SerializeField] Transform headLookTarget;
    [SerializeField] Transform pitLookTransform;

    [SerializeField] float bodyWobbleSpeed = 2.0f;
    [SerializeField] float maxBodyWobble = 2.0f;

    private Transform currentLookTarget = null;
    private float bodyWobbleDistance = 0f;
    private bool wobbleTo = false;
    private Vector3 bodyStartPos;

    [Header("Dive Animation")]
    [SerializeField] float diveSpeed = 3.0f;
    [SerializeField] float waitTime = 1.0f;
    [SerializeField] float targetAngle = 95f;
    Quaternion targetRotation;
    bool diving = false;

    [SerializeField] ParticleSystem fireBreathParticles;
    public bool useHeadLook = true;

    private void Start()
    {
        bodyStartPos = bodyPivot.localPosition;
        targetRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (!diving)
        {
            WobbleBody();
        }

        headPivot.transform.LookAt(headLookTarget, Vector3.up);

        if (currentLookTarget && useHeadLook)
        {
            headLookTarget.position = Vector3.Lerp(
                headLookTarget.position,
                currentLookTarget.position,
                0.1f
            );
        }
        else
        {
            Vector3 forwardPos = headPivot.transform.position - transform.forward * 5f;
            headLookTarget.position = Vector3.Lerp(
                headLookTarget.position,
                forwardPos,
                0.1f
            );
        }

        if (diving)
        {
            bodyPivot.rotation = Quaternion.Slerp(
                bodyPivot.rotation,
                targetRotation,
                Time.deltaTime * diveSpeed
            );
        }
        else
        {
            bodyPivot.rotation = Quaternion.Slerp(
                bodyPivot.rotation,
                transform.rotation,
                Time.deltaTime * diveSpeed
            );
        }
    }

    void WobbleBody()
    {
        if (wobbleTo)
        {
            bodyWobbleDistance += bodyWobbleSpeed * Time.deltaTime;
            if (bodyWobbleDistance > maxBodyWobble)
                wobbleTo = false;
        }
        else
        {
            bodyWobbleDistance -= bodyWobbleSpeed * Time.deltaTime;
            if (bodyWobbleDistance < -maxBodyWobble)
                wobbleTo = true;
        }

        bodyPivot.localPosition = bodyStartPos + transform.right * bodyWobbleDistance;
    }

    public void DoDive()
    {
        if (!diving)
            StartCoroutine(DiveAnimation());
    }

    IEnumerator DiveAnimation()
    {
        diving = true;
        useHeadLook = false;
        targetRotation = Quaternion.Euler(targetAngle, transform.eulerAngles.y, 0f);
        yield return new WaitForSeconds(waitTime/2.0f);
        fireBreathParticles.Play();

        yield return new WaitForSeconds(waitTime);

        useHeadLook = true;
        diving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentLookTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == currentLookTarget)
        {
            currentLookTarget = null;
        }
    }
}
