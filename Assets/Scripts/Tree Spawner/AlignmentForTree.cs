using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]

public class AlignmentForTree : MonoBehaviour
{
    public LayerMask ground;
    private float RandomTurnVal;
    void Awake()
    {
        // check up for ground
        CheckDown();
        //if not up check down for ground
        RandomTurnVal = Random.Range(-360f, 360f);
        gameObject.transform.localRotation = Quaternion.Euler(new Vector3 (gameObject.transform.localRotation.y, RandomTurnVal, gameObject.transform.localRotation.z));


    }
   
    private void CheckDown()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(gameObject.transform.position, -Vector3.up, out hitInfo, 1000f, ground))
        {

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, hitInfo.point.y, gameObject.transform.position.z);

            //float RandomTurnVal = Random.Range(-360f, 360f);
           // gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.y, RandomTurnVal, gameObject.transform.rotation.z);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
}
