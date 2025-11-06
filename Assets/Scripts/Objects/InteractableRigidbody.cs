using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableRigidbody : MonoBehaviour
{
    public int value = 0;
    public ObjectType type;

    public bool isActivated;
    public bool isGrabbed;

    private Vector3 initialPos;
    private Quaternion initialRotation;

    [SerializeField] AudioClip hitClip;

    GameObject emberEffect;

    bool playingAudio = false;

    public enum ObjectType 
    {
        SMALL, //Can be lifted above head, doesnt apply force on player
        LARGE, //Cannot be equiped or lifted above head, applies force on player
        BREAKABLE, //Breaks lol
        EQUIPABLE, //Cosmetics
    }

    public virtual void ActivateObject(PhysicsGrabber grabber) 
    {
        isActivated = true;
    }

    public virtual void DeactivateObject() 
    {
        isActivated = false;
    }

    public virtual void OnGrab(PhysicsGrabber grabber) 
    {
        isGrabbed = true;
    }

    public virtual void OnDrop() 
    {
        isGrabbed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playingAudio) return;

        if (collision.relativeVelocity.magnitude > 3.0f) 
        {
            StartCoroutine(PauseAudioPlaying());
            AudioController.instance.PlayAudioClip(hitClip, transform);
        }
    }

    IEnumerator PauseAudioPlaying() 
    {
        playingAudio = true;
        yield return new WaitForSeconds(0.5f);
        playingAudio = false;
    }


}
