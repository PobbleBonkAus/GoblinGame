using UnityEngine;

public class MilestoneDoor : MonoBehaviour
{
    public bool unlocked = false;

    [SerializeField] Rigidbody leftDoor;
    [SerializeField] Rigidbody rightDoor;
    [SerializeField] float torqueStrength = 500f;

    public void UnlockDoor()
    {
        if (unlocked) return;
        unlocked = true;

        leftDoor.isKinematic = false;
        rightDoor.isKinematic = false;

        // Apply torque around the hinge axis (usually Y for upright doors)
        leftDoor.AddTorque(Vector3.up * torqueStrength, ForceMode.Impulse);
        rightDoor.AddTorque(Vector3.up * -torqueStrength, ForceMode.Impulse);
    }
}
