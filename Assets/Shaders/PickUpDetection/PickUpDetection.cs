using System;
using UnityEngine;

public class PickUpDetection : MonoBehaviour
{
    Collider pickUpCollider;
    GameObject[] pickUpObjects;
    private void Start()
    {
        pickUpCollider = GetComponent<Collider>();    
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
}
