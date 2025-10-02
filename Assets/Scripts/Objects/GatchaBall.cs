using UnityEngine;

public class GatchaBall : InteractableRigidbody
{
    public override void ActivateObject(PhysicsGrabber grabber)
    {
        base.ActivateObject(grabber);
        //spawn hat

        GameObject instance = Instantiate(HatBoard.instance.RetrieveHat());
        instance.transform.position = transform.position;

        Destroy(gameObject);
    }
}
