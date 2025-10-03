using UnityEngine;

public class TV : InteractableRigidbody
{
    [SerializeField] Material litMat;
    [SerializeField] Material unlitMat;
    [SerializeField] MeshRenderer renderer;

    Material[] mats;
    private void Awake()
    {
        mats = renderer.materials;
    }

    public override void ActivateObject(PhysicsGrabber grabber)
    {
        Debug.Log("Light");
        renderer.materials = new Material[2] { mats[0], litMat};
    }

    public override void DeactivateObject()
    {
        renderer.materials = new Material[2] { mats[0], unlitMat };
    }

}
