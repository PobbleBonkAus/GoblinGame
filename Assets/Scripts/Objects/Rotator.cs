using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 30.0f;
    [SerializeField] Vector3 rotationAxis = Vector3.forward;
    [SerializeField] float maxRotationSpeed = 30.0f;

    Rigidbody rb;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(rb.angularVelocity.magnitude < maxRotationSpeed) 
        {
            rb.AddTorque(rotationAxis * rotationSpeed, ForceMode.Force);
        } 
       
    }
}
