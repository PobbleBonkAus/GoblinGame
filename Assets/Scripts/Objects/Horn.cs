using UnityEngine;

public class Horn : InteractableRigidbody
{
    [SerializeField] AudioClip hornToot;
    public override void ActivateObject(PhysicsGrabber grabber)
    {
        base.ActivateObject(grabber);
        AudioController.instance.PlayAudioClip(hornToot,transform);
    }
}
