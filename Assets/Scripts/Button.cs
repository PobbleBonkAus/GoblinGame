using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] UnityEvent ButtonPressed;
    [SerializeField] float pushAmount;

    Rigidbody rb;
    Vector3 pushTarget;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pushTarget = new Vector3(transform.position.x, transform.position.y, transform.position.z - pushAmount);
    }

    private void FixedUpdate()
    {
        if(transform.position.z < pushTarget.z) 
        {
            PressButton();
        }
    }

    void PressButton() 
    {
        rb.isKinematic = true;
        ButtonPressed.Invoke();
        Debug.Log("ButtonPressed");
    }
}   
