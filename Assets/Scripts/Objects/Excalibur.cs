using UnityEngine;

public class Excalibur : InteractableRigidbody
{
    [SerializeField] AudioClip angelicChoir;
    [SerializeField] Transform lightShaft;

    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void ActivateObject(PhysicsGrabber grabber)
    {
        base.ActivateObject(grabber);
        GetComponent<Rigidbody>().isKinematic = false;
        AudioController.instance.PlayAudioClip(angelicChoir,transform);
    }
}
