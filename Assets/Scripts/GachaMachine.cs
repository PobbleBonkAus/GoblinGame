using System.Collections;
using UnityEngine;

public class GachaMachine : MonoBehaviour
{
    [SerializeField] private GameObject[] cosmetics;
    [SerializeField] private float ejectionForce = 4.0f;
    [SerializeField] private Transform cosmeticSpawnPoint;
    [SerializeField] private int minimumCoinsRequired = 3;
    [SerializeField] private float cosmeticEjectionDelay = 2.0f;

    int currentCoinsInputed = 0;

    void AddCoin() 
    {
        currentCoinsInputed += 1;
        if(currentCoinsInputed >= minimumCoinsRequired) 
        {
            StartCoroutine(SpawnRandomCosmetic());
        }
    }

    IEnumerator SpawnRandomCosmetic() 
    {
        yield return new WaitForSeconds(cosmeticEjectionDelay);

        GameObject cosmetic = Instantiate(cosmetics[Random.Range(0, cosmetics.Length)]);
        cosmetic.transform.SetPositionAndRotation(cosmeticSpawnPoint.position, Random.rotation);
        cosmetic.GetComponent<Rigidbody>().AddForce(cosmeticSpawnPoint.forward * ejectionForce,ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) 
        {
            AddCoin();
            Destroy(other);
        }
    }

}
