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

    [SerializeField] UnityEvent OnValuableDeposit;
    [SerializeField] UnityEvent OnJunkDeposit;

    [SerializeField] AudioClip depositNoise;
    [SerializeField] AudioClip ejectionNoise;


    void RecieveObject(Rigidbody body) 
    {
        Debug.Log("recieved body" + body.name);
        int value = 0;

        if(body.TryGetComponent<InteractableRigidbody>(out InteractableRigidbody sellable)) 
        {
            value = sellable.value;
        }
        
        if(value != 0) 
        {
            StartCoroutine(SpawnCoins(value));
            Destroy(body.gameObject);
            OnValuableDeposit.Invoke();
        }
        else
        {
            Debug.Log("JUNK");
            StartCoroutine(SpitJunkBackOut(body));
            OnJunkDeposit.Invoke();
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

    IEnumerator SpitJunkBackOut(Rigidbody body) 
    {
        yield return new WaitForSeconds(initialEjectDelay);

        Debug.Log("Ejecting Junk");
        body.linearVelocity = coinSpawn.forward * ejectionForce;
    }


    private void OnTriggerEnter(Collider other)
    {     
        if (!other.isTrigger && other.attachedRigidbody) 
        {
            RecieveObject(other.attachedRigidbody);
        }

    }

}
