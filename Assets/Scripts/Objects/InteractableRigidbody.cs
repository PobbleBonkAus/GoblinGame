using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableRigidbody : MonoBehaviour
{
    public int value = 0;
    public ObjectType type;
    
    public enum ObjectType 
    {
        SMALL, //Can be lifted above head, doesnt apply force on player
        LARGE, //Cannot be equiped or lifted above head, applies force on player
        BREAKABLE, //Breaks lol
        EQUIPABLE, //Cosmetics
    }


}
