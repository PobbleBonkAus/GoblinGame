using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class trashPile : MonoBehaviour
{
    [SerializeField] GameObject[] trashSpawns;
    [SerializeField][Range(5,12)] int amountOfTrash = 10;

    [SerializeField] GameObject state1;
    [SerializeField] GameObject state2;
    [SerializeField] GameObject state3;

    int currentAmountLeft;

    bool canGrabFrom = true;

    private void Awake()
    {
        currentAmountLeft = amountOfTrash;
        state1.gameObject.SetActive(true);
    }

    public void SpawnTrashPile(PhysicsGrabber playerGrabber) 
    {
        GameObject instance = Instantiate(trashSpawns[Random.Range(0,trashSpawns.Length)]);
        instance.transform.position = playerGrabber.transform.position;
        playerGrabber.ForceGrabObject(instance.GetComponent<Rigidbody>());

        currentAmountLeft -= 1;
        if (currentAmountLeft > 0) 
        {
            UpdateTrashPile();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void UpdateTrashPile()
    {
        if ((currentAmountLeft < amountOfTrash * 0.33))
        {
            state1.SetActive(false);
            state2.SetActive(false);
            state3.SetActive(true);
        }
        else if ((currentAmountLeft < amountOfTrash * 0.66))
        {
            state1.SetActive(false);
            state2.SetActive(true);
            state3.SetActive(false);
        }
        else
        {
            state1.SetActive(true);
            state2.SetActive(false);
            state3.SetActive(false);
        }

    }

    IEnumerator<WaitForSeconds> WaitTime() 
    {
        canGrabFrom = false;
        yield return new WaitForSeconds(1.0f);
        canGrabFrom = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canGrabFrom) return;

        if (other.GetComponentInChildren<PhysicsGrabber>())
        {
            PhysicsGrabber grabber = other.GetComponentInChildren<PhysicsGrabber>();
            if (grabber.grabPressed && !grabber.grabbing)
            {
                Debug.Log("spawn trash");
                SpawnTrashPile(grabber);

                StartCoroutine(WaitTime());
            }
        }
    }


}
