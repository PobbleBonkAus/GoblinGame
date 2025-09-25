using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    [SerializeField] float jumpForce;
    [SerializeField] float jumpTime;
    private Transform target;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator<WaitForSeconds> JumpTowardsTarget() 
    {
        yield return new WaitForSeconds(jumpTime);   
        Vector3 direction = (target.position - transform.position).normalized;

        rb.AddForce((direction + Vector3.up) * jumpForce, ForceMode.Impulse);

        StartCoroutine(JumpTowardsTarget());
    }


    private void OnTriggerStay(Collider other)
    {
        if(target == null) 
        {
            target = other.transform;
            StartCoroutine(JumpTowardsTarget());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (other.transform == target)
            {
                target = null;
                StopCoroutine(JumpTowardsTarget());
            }
        }

    }
}
