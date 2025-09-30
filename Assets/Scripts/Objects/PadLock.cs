using UnityEngine;
using UnityEngine.Events;

public class PadLock : MonoBehaviour
{
    [SerializeField] Transform hook;
    [SerializeField] float openAnimationSpeed = 0.1f; // degrees per second
    [SerializeField] UnityEvent[] OnUnlockEvents;

    bool unlocked = false;
    bool unlocking = false;
    float targetAngle = 45f; // how far to rotate before unlocking

    Rigidbody key;
    void Unlock()
    {
        if (unlocked) return;

        unlocked = true;
        GetComponent<Rigidbody>().isKinematic = false;

        foreach (UnityEvent unlockEvent in OnUnlockEvents)
        {
            unlockEvent.Invoke();
            
        }
        GetComponent<Rigidbody>().AddForce(-transform.forward * 100.0f, ForceMode.Impulse);
        key.isKinematic = false;
    }

    private void Update()
    {
        if (unlocking && !unlocked)
        {
            key.transform.rotation = Quaternion.LookRotation(-transform.right, transform.up);
            float step = openAnimationSpeed * Time.deltaTime;
            hook.transform.Rotate(Vector3.up, -step);
            Debug.Log(hook.rotation.eulerAngles);
            // check if rotated enough
            if (hook.rotation.eulerAngles.x < 10)
            {
                unlocking = false;
                Unlock();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!unlocked && other.CompareTag("Key"))
        {
            unlocking = true;
            key = other.attachedRigidbody;
            key.isKinematic = true;
            key.transform.position = transform.position - transform.forward;
            key.transform.rotation = Quaternion.LookRotation(-transform.right,transform.up);
        }
    }
}
