using UnityEngine;

public class Lightbulb : InteractableRigidbody
{
    [SerializeField] Material litMat;
    [SerializeField] Material unlitMat;
    [SerializeField] Transform bulb;
    MeshRenderer renderer;

    private void Awake()
    {
        renderer = bulb.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = unlitMat;
    }

    public override void ActivateObject(PhysicsGrabber grabber) 
    {
        renderer.sharedMaterial = litMat;
    }

    public override void DeactivateObject() 
    {
        renderer.sharedMaterial = unlitMat;
    }
}
