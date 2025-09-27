using System.Collections.Generic;
using UnityEngine;

public class RiverPoint : MonoBehaviour
{

    [SerializeField] float flowForce = 5.0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody) 
        {
            other.attachedRigidbody.AddForce(transform.forward * flowForce, ForceMode.Force);
        }
    }

}
