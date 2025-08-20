using UnityEngine;
using UnityEngine.InputSystem;

public class DitherEnable : MonoBehaviour
{

    [Range(0f,1f)] public float alphaChange;
    public PhysicsGrabber physicsGrabber;
    bool grabPressed;


    private void Update()
    {
        grabPressed = physicsGrabber.grabPressed;
        if (grabPressed)
        {
            colorAlpha(alphaChange);
        }
        else
        {
            colorAlpha(1f);
        }
    }

    private void colorAlpha(float alphaValue)
    {
        foreach (Renderer render in gameObject.GetComponentsInChildren<Renderer>())
        {
            render.material.SetVector("_BaseValue", new Vector4(1, 1, 1, alphaValue));
        }
    }
}