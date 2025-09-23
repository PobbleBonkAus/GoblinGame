using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableRigidbody : MonoBehaviour
{
    public int value = 0;
    public ObjectType type;

    public bool isActivated;
    public bool isGrabbed;
    public enum ObjectType 
    {
        SMALL, //Can be lifted above head, doesnt apply force on player
        LARGE, //Cannot be equiped or lifted above head, applies force on player
        BREAKABLE, //Breaks lol
        EQUIPABLE, //Cosmetics
    }

    private void Update()
    {
        if(transform.position.y < -10.0f) 
        {
            transform.position = new Vector3(Random.Range(-30, 30), 100, Random.Range(-30, 30));
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }

    public virtual void ActivateObject() 
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


}
