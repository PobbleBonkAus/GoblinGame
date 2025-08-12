using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableRigidbody : MonoBehaviour
{
    public int value = 0;
    public ObjectType type;
    
    public enum ObjectType 
    {
        SMALL,
        LARGE,
        BREAKABLE,
        EQUIPABLE,
    }


}
