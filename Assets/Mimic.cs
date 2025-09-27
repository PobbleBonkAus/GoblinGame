using System.Collections;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    [SerializeField] float jumpForce = 10.0f;
    [SerializeField] float jumpInterval = 2.0f;
    [SerializeField] float turnTorque = 3.0f;
    [SerializeField] float uprightTorque = 3.0f;
    private Transform target;
    private Rigidbody rb;
    private Coroutine jumpRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(target != null) 
        {
           RotateTowardsVelocity();         
        }
        KeepUpright();
    }

    private IEnumerator JumpTowardsTarget()
    {
        while (target != null)
        {
            yield return new WaitForSeconds(jumpInterval);

            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 jumpDir = direction + Vector3.up;

            rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
        }
    }


    private void RotateTowardsVelocity()
    {
        Vector3 vel = rb.linearVelocity;
        if (vel.sqrMagnitude > 0.01f)
        {
            Vector3 desiredDir = -vel.normalized;
            Vector3 currentDir = transform.forward;

            float angle = Vector3.SignedAngle(currentDir, desiredDir, Vector3.up);
            rb.AddTorque(Vector3.up * angle * turnTorque, ForceMode.Acceleration);
        }
    }

    private void KeepUpright()
    {
        Vector3 up = transform.up;
        Vector3 worldUp = Vector3.up;

        Vector3 tiltAxis = Vector3.Cross(up, worldUp);
        rb.AddTorque(tiltAxis * uprightTorque, ForceMode.Acceleration);
    }

    private void OnTriggerStay(Collider other)
    {
        if (target == null && other.CompareTag("Player"))
        {
            target = other.transform;
            jumpRoutine = StartCoroutine(JumpTowardsTarget());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == target)
        {
            target = null;
            if (jumpRoutine != null)
            {
                StopCoroutine(jumpRoutine);
                jumpRoutine = null;
            }
        }
    }
}
