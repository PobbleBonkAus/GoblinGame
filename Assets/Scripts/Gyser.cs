using System.Collections.Generic;
using UnityEngine;

public class Gyser : MonoBehaviour
{

    [SerializeField]
    private float timer = 10.0f;
    [SerializeField]
    private float gyserForce = 10.0f;
    [SerializeField]
    private Transform gyserDirectionTransform;

    List<Rigidbody> rigidbodies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("ShootWater");   
    }

    private void FixedUpdate()
    {
        

    }

    IEnumerator<WaitForSeconds> ShootWater() 
    {
        
        yield return new WaitForSeconds(timer); 

        for (int i = 0; i < rigidbodies.Count; i++) 
        {
            rigidbodies[i].AddForce(gyserDirectionTransform.up * gyserForce, ForceMode.Impulse);
            print(rigidbodies[i].name);
        }

        StartCoroutine("ShootWater");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null) 
        {
            rigidbodies.Remove(other.attachedRigidbody);    
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (rigidbodies.Contains(other.attachedRigidbody)) 
        {
            rigidbodies.Remove(other.attachedRigidbody);
        }
    }


}
