using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Button : MonoBehaviour
{
    [SerializeField] private UnityEvent ButtonPressed;
    [SerializeField] private float pushDistance = 0.1f; // how far it can move
    [SerializeField] private float pressThreshold = 0.95f; // % of travel before "pressed"

    private Rigidbody rb;
    private Vector3 startPos;
    private Vector3 pushDir;
    private Vector3 targetPos;
    private bool isPressed = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None; // we’ll handle locking
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        startPos = transform.position;
        pushDir = transform.forward.normalized; // local forward direction
        targetPos = startPos + pushDir * -pushDistance; // press backwards
    }

    private void FixedUpdate()
    {
        // --- Lock motion to local z axis only ---
        Vector3 toStart = transform.position - startPos;
        float distance = Vector3.Dot(toStart, pushDir); // signed distance along forward axis
        Vector3 constrainedPos = startPos + pushDir * distance;

        transform.position = constrainedPos;

        // --- Restrict velocity to only along pushDir ---
        float speed = Vector3.Dot(rb.linearVelocity, pushDir);
        rb.linearVelocity = pushDir * speed;

        // --- Clamp movement within range ---
        float clampedDistance = Mathf.Clamp(distance, -pushDistance, 0f);
        transform.position = startPos + pushDir * clampedDistance;

        // --- Trigger event when pressed ---
        float pressPercent = Mathf.Abs(clampedDistance / pushDistance);
        if (!isPressed && pressPercent >= pressThreshold)
        {
            PressButton();
        }
    }

    void PressButton()
    {
        isPressed = true;
        rb.isKinematic = true; // lock it once pressed
        ButtonPressed?.Invoke();
        Debug.Log("Button Pressed!");
    }
}