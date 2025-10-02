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
        renderer.sharedMaterials[0] = unlitMat;
    }

    public override void ActivateObject(PhysicsGrabber grabber) 
    {
        Debug.Log("Light");
        renderer.sharedMaterials[0] = litMat;
    }

    public override void DeactivateObject() 
    {
        renderer.sharedMaterials[0] = unlitMat;
    }
}
