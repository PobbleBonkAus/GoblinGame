using UnityEngine;
using UnityEngine.InputSystem;

public class trashPile : MonoBehaviour
{
    GameObject[] piles;
    [SerializeField] GameObject trashBall;

    int currentTrashPile = -1;

    private void Start()
    {        
        piles = GetComponentsInChildren<GameObject>();
    }

    public void UpdateTrashPile() 
    {
        if(currentTrashPile < piles.Length) 
        {
            if(currentTrashPile == -1) 
            {
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<MeshCollider>().enabled = false;
                return;
            }

            GameObject pile = piles[currentTrashPile - 1];
            pile.GetComponent<MeshRenderer>().enabled = false;
            pile.GetComponent<MeshCollider>().enabled = false;

            currentTrashPile += 1;
        }

    }

    public void SpawnTrashPile(PhysicsGrabber playerGrabber) 
    {
        GameObject instance = Instantiate(trashBall);
        instance.transform.position = playerGrabber.transform.position;

        playerGrabber.ForceGrabObject(instance.GetComponent<Rigidbody>());
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            SpawnTrashPile(other.GetComponentInChildren<PhysicsGrabber>());
        }
    }
}
