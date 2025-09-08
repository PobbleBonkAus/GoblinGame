using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //Track cosmetics
    //Track statues
    //Track Sellables
    //Track world borders

    private int statuesRaised = 0;
    private List<Rigidbody> sellables;
    

    [SerializeField] GameObject[] statues;
    [SerializeField] float statueRaiseSpeed = 0.5f;
    [SerializeField] Transform[] beachSpawns; 


    bool raisingStatue;
    GameObject statueBeingRaised;

    public static GameManager gameManager;




    private void Update()
    {
        if (raisingStatue) 
        {
            statueBeingRaised.transform.position += Vector3.up * statueRaiseSpeed;
            if(statueBeingRaised.transform.position.y > 19) 
            {
                raisingStatue = false;
                statuesRaised += 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }


    }

    public void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void RaiseStatue() 
    {
        statueBeingRaised = statues[statuesRaised];
        raisingStatue = true;
    }


    public Vector3 GetNearestBeachSpawn(Vector3 playerPosition) 
    {
        Vector3 nearestBeach = beachSpawns[0].position;
        float distanceToNearestBeach = 1000000.0f;

        for(int i = 0; i < beachSpawns.Length; i++) 
        {
            float distance = Vector3.Distance(playerPosition, beachSpawns[i].position);
            if (distance < distanceToNearestBeach) 
            {
                nearestBeach = beachSpawns[i].position;
                distanceToNearestBeach = distance;
            }
        }

        return nearestBeach;

    }

}
