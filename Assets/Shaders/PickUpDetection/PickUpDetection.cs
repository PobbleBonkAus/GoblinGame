using System;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpDetection : MonoBehaviour
{
    [SerializeField] private SphereCollider PhysicsGrabber;
    //GameObject[] pickUpObjects;
    //Renderer pickUpObject;
    private void Start()
    {
          
    }
    GameObject highlightedObject;
    private void OnTriggerEnter(Collider other)
    {
        Renderer highlighted;
        highlighted = highlightedObject.GetComponent<Renderer>();
        if (other != null)
        {
            return;
        } 
        else if (Vector3.Distance(PhysicsGrabber.transform.position, other.transform.position) < Vector3.Distance(PhysicsGrabber.transform.position, highlightedObject.transform.position))
        {
            //Sets previous object to not show overlay
            highlighted.material.SetInt("_EnableOverlay", 0);

            //Puts new object as current object
            highlightedObject = other.gameObject;
            
            //Sets new object to show overlay
            highlighted.material.SetInt("_EnableOverlay", 1);
        }
    }
}
