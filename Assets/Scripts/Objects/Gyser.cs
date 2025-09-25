using System.Collections.Generic;
using UnityEngine;

public class Gyser : MonoBehaviour
{

    [SerializeField]
    private float timer = 1.0f;
    [SerializeField]
    private float gyserForce = 1000.0f;
    [SerializeField]
    private Transform gyserDirectionTransform;

    List<Rigidbody> rigidbodies = new List<Rigidbody>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShootWater());   
    }


    IEnumerator<WaitForSeconds> ShootWater() 
    {
        yield return new WaitForSeconds(timer);

        for (int i = 0; i < rigidbodies.Count; i++) 
        {
            rigidbodies[i].AddForce(gyserDirectionTransform.up * gyserForce, ForceMode.Impulse);
        }

        StartCoroutine(ShootWater());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null) 
        {
            if (!rigidbodies.Contains(other.attachedRigidbody)) 
            {
                rigidbodies.Add(other.attachedRigidbody);
            }
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
