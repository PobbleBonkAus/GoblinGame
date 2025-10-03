using UnityEngine;

public class Lightbulb : InteractableRigidbody
{
    [SerializeField] Material litMat;
    [SerializeField] Material unlitMat;
    [SerializeField] MeshRenderer renderer;

    private void Awake()
    {
        //renderer = GetComponent<MeshRenderer>();
        renderer.GetComponent<MeshRenderer>().material = unlitMat;
    }

    public override void ActivateObject(PhysicsGrabber grabber) 
    {
        Debug.Log("Light");
        renderer.GetComponent<MeshRenderer>().material = litMat;
    }

    public override void DeactivateObject() 
    {
        renderer.GetComponent<MeshRenderer>().material = unlitMat;
    }
}
