using UnityEngine;

public class Hat : InteractableRigidbody
{
   
    public override void ActivateObject(PhysicsGrabber grabber)
    {
        grabber.cosmeticHandler.UnequipCosmetic();
        grabber.cosmeticHandler.EquipCosmetic(transform.gameObject);
        grabber.ReleaseObject();
        Debug.Log(":HAT");
    }
}
