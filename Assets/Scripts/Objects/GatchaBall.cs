using System.Collections;
using UnityEngine;

public class GatchaBall : InteractableRigidbody
{
    [SerializeField] GameObject particleObject;
    public override void ActivateObject(PhysicsGrabber grabber)
    {
        base.ActivateObject(grabber);
        //spawn hat


        GameObject instance = Instantiate(HatBoard.instance.RetrieveHat());
        instance.transform.position = transform.position;

        GameObject particle = Instantiate(particleObject);
        particle.transform.position = transform.position + transform.up * 0.2f;

        Destroy(gameObject);
    }

}
