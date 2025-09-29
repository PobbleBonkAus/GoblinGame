using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class trashPile : MonoBehaviour
{
    [SerializeField] GameObject[] trashSpawns;
    [SerializeField] int amountOfTrash = 10;

    Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void SpawnTrashPile(PhysicsGrabber playerGrabber) 
    {
        GameObject instance = Instantiate(trashSpawns[Random.Range(0,trashSpawns.Length)]);
        instance.transform.position = playerGrabber.transform.position;
        playerGrabber.ForceGrabObject(instance.GetComponent<Rigidbody>());

        amountOfTrash -= 1;
        if (amountOfTrash > 0) 
        {
            UpdateTrashPile();
        }
        else
        {
            Destroy(instance);
        }

    }

    void UpdateTrashPile()
    {
        transform.localScale -= baseScale / amountOfTrash; 
    }

    IEnumerator<WaitForSeconds> ReeneableCollider() 
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1);
        GetComponent<Collider>().enabled = true;

    }


    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInChildren<PhysicsGrabber>())
        {
            PhysicsGrabber grabber = other.GetComponentInChildren<PhysicsGrabber>();
            if (grabber.grabPressed && !grabber.grabbing)
            {
                Debug.Log("spawn trash");
                SpawnTrashPile(grabber);
            }
        }
    }
    
}
