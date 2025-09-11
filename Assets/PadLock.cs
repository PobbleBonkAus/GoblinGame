using UnityEngine;
using UnityEngine.Events;

public class PadLock : MonoBehaviour
{
    [SerializeField] Transform hook;
    [SerializeField] float openAnimationSpeed = 0.1f; // degrees per second
    [SerializeField] UnityEvent[] OnUnlockEvents;
    [SerializeField] GameObject key;

    bool unlocked = false;
    bool unlocking = false;
    float targetAngle = 45f; // how far to rotate before unlocking

    void Unlock()
    {
        if (unlocked) return;

        unlocked = true;
        GetComponent<Rigidbody>().isKinematic = false;

        foreach (UnityEvent unlockEvent in OnUnlockEvents)
        {
            unlockEvent.Invoke();
        }
    }

    private void Update()
    {
        if (unlocking && !unlocked)
        {
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
        if (!unlocked && other.gameObject == key)
        {
            unlocking = true;
        }
    }
}
