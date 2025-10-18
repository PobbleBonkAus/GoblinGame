using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaMachine : MonoBehaviour
{
    [SerializeField] private GameObject[] cosmetics;
    [SerializeField] private float ejectionForce = 4.0f;
    [SerializeField] private Transform cosmeticSpawnPoint;
    [SerializeField] private int minimumCoinsRequired = 3;
    [SerializeField] private float cosmeticEjectionDelay = 2.0f;

    [SerializeField] private Transform gatchaCrank;
    [SerializeField] private float crankSpeed;


    [SerializeField] AudioClip coinDepositNoise;
    [SerializeField] AudioClip gatchaSpawnNoise;

    int currentCoinsInputed = 0;

    bool addingCoin = false;

    private void Update()
    {
        if (addingCoin)
        {
            gatchaCrank.transform.Rotate(Vector3.forward, crankSpeed);
        }
    }

    void AddCoin() 
    {
        currentCoinsInputed += 1;
        if(currentCoinsInputed >= minimumCoinsRequired) 
        {
            addingCoin = true;
            AudioController.instance.PlayAudioClip(coinDepositNoise, transform);
            StartCoroutine(SpawnRandomCosmetic());
        }
    }

    IEnumerator SpawnRandomCosmetic() 
    {
        yield return new WaitForSeconds(cosmeticEjectionDelay);
        AudioController.instance.PlayAudioClip(gatchaSpawnNoise, transform);

        addingCoin = false;
        GameObject cosmetic = Instantiate(cosmetics[Random.Range(0, cosmetics.Length)]);
        cosmetic.transform.SetPositionAndRotation(cosmeticSpawnPoint.position, Random.rotation);
        cosmetic.GetComponent<Rigidbody>().AddForce(cosmeticSpawnPoint.forward * ejectionForce,ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) 
        {
            AddCoin();
            Destroy(other.gameObject);
        }
    }

}
