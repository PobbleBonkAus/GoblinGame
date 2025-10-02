using UnityEngine;

public class Lightbulb : InteractableRigidbody
{
    [SerializeField] Material litMat;
    [SerializeField] Material unlitMat;
    [SerializeField] Transform bulb;
    MeshRenderer renderer;

    private void Awake()
    {
        //renderer = GetComponent<MeshRenderer>();
        renderer.GetComponent<MeshRenderer>().materials[1] = unlitMat;
    }

    public override void ActivateObject(PhysicsGrabber grabber) 
    {
        Debug.Log("Light");
        renderer.GetComponent<MeshRenderer>().materials[1] = litMat;
    }

    public override void DeactivateObject() 
    {
        renderer.GetComponent<MeshRenderer>().materials[1] = unlitMat;
    }
}
