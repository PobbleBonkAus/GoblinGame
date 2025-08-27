using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //Track cosmetics
    //Track statues
    //Track Sellables
    //Track world borders

    

    private int statuesRaised = 0;
    private int[] cosmeticsObtained;
    private List<Rigidbody> sellables;

    [SerializeField] GameObject[] statues;
    [SerializeField] float statueRaiseSpeed = 0.5f;

    bool raisingStatue;
    GameObject statueBeingRaised;

    

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
    }

    public void RaiseStatue() 
    {
        statueBeingRaised = statues[statuesRaised];
        raisingStatue = true;
    }



}
