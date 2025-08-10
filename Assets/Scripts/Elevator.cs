using UnityEngine;

public class KinematicElevator : MonoBehaviour
{
    public Rigidbody elevatorRb; // assign platform's rigidbody
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 2f;
    public bool loop = true;
    public bool autoStart = true;

    private Vector3 targetPos;
    private bool movingUp;

    void Start()
    {
        if (elevatorRb == null)
            elevatorRb = GetComponent<Rigidbody>();

        if (startPoint != null)
            elevatorRb.position = startPoint.position;

        targetPos = endPoint.position;
        movingUp = true;

        if (!autoStart)
            enabled = false;
    }

    void FixedUpdate()
    {
        Vector3 newPos = Vector3.MoveTowards(elevatorRb.position, targetPos, speed * Time.fixedDeltaTime);
        elevatorRb.MovePosition(newPos);

        if (Vector3.Distance(elevatorRb.position, targetPos) < 0.01f)
        {
            if (loop)
            {
                movingUp = !movingUp;
                targetPos = movingUp ? endPoint.position : startPoint.position;
            }
            else
            {
                enabled = false;
            }
        }
    }

    // Optional method to trigger from another script or button
    public void ActivateElevator()
    {
        enabled = true;
    }
}
