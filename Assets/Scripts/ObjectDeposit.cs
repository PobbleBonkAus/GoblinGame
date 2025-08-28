using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ObjectDeposit : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float initialEjectDelay = 1.0f;
    [SerializeField] private float coinSpacingTime = 0.5f;
    [SerializeField] private float ejectionForce = 10.0f;
    [SerializeField] private Vector3 baseEjectionAngle = Vector3.zero;
    [SerializeField] private float minEjectionAngle = 10.0f;
    [SerializeField] private float maxEjectionAngle = 30.0f;

    [SerializeField] private Transform coinSpawn;

    [SerializeField] UnityEvent OnDeposit;

    void RecieveObject(InteractableRigidbody interactableObject) 
    {
        Debug.Log("Recieved Object");
        string tag = interactableObject.tag;
        switch (tag)
        {
            default:
                Debug.Log("found default " + interactableObject.name);
                StartCoroutine(SpitJunkBackOut(interactableObject));
                break;
            case "Sellable":
                Debug.Log("found sellable" + interactableObject.name);
                StartCoroutine(SpawnCoins(interactableObject.value));
                Destroy(interactableObject.gameObject);
                OnDeposit.Invoke();
                break;
            case "Junk":
                Debug.Log("found junk " + interactableObject.name);
                StartCoroutine(SpitJunkBackOut(interactableObject));
                break;
        }
    }
    
    IEnumerator SpawnCoins(int itemValue) 
    {
        yield return new WaitForSeconds(initialEjectDelay);

        for(int i = 0; i < itemValue; i++)
        {
            SpawnCoin();
            
            yield return new WaitForSeconds(coinSpacingTime);
        }
    }
    
    void SpawnCoin() 
    {
        Debug.Log("Spawning Coin");
        GameObject coin = Instantiate(coinPrefab);
        coin.transform.SetPositionAndRotation(coinSpawn.position, Random.rotation);
        coin.GetComponent<Rigidbody>().AddForce(coinSpawn.forward * ejectionForce,ForceMode.Impulse);
    }

    IEnumerator SpitJunkBackOut(InteractableRigidbody interactableObject) 
    {
        yield return new WaitForSeconds(initialEjectDelay);
        Debug.Log("Ejecting Junk");
        interactableObject.GetComponent<Rigidbody>().linearVelocity = coinSpawn.forward * ejectionForce;
    }

    IEnumerator SpitPlayerBackOut(GameObject player) 
    {
        yield return new WaitForSeconds(initialEjectDelay);
        player.GetComponent<PlayerController>().isRagdolled = true;
        player.GetComponent<Rigidbody>().linearVelocity = coinSpawn.forward * ejectionForce;

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered");
        if (other.TryGetComponent<InteractableRigidbody>(out InteractableRigidbody body)) 
        {
            RecieveObject(body);
        }
        else if (other.gameObject.CompareTag("Player")) 
        {
            SpitPlayerBackOut(other.gameObject);
        }

    }

}
