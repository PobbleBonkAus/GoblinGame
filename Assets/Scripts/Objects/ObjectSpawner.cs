using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] spawnables;
    public int objectsToSpawn = 100;
    public float spawnRadius = 80.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < objectsToSpawn; i++) 
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;

            GameObject obj = Instantiate(spawnables[Random.Range(0, spawnables.Length)],spawnPosition,Random.rotation,transform.parent);

            obj.transform.position = spawnPosition;
            obj.transform.rotation = Random.rotation;


        }
    }
}
