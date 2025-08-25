using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class TreeSpawning : MonoBehaviour
{
    [Header("Tree Settings")]
    public LayerMask ground;
    public float radius;
    public int desnisty;
    public GameObject[] treeOBJs;

    [Header("Tree Maker")]
    public bool MakeTrees;

    private Vector3 randomLocationInRadius;
    public Vector3 wireLocation;
    private void Update()
    {
        GetWireframeGizmos();
        if (MakeTrees)
        {
            TreeButton();
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        TreeButton();
    }
    
    public void TreeButton()
    {
        //send a raycast down to check for ground layer
        for (int i = 0; i < desnisty; i++)
        {
            //get random position from sphere hitpoint
            randomLocationInRadius = transform.position + Random.insideUnitSphere * radius;

            //spawn point is either up or down from the firstpoint pos is based on if a ray hit the ground layer

            //choose random tree
            int treeNumber = Random.Range(0, treeOBJs.Length);

            //spawn chosen random tree at random location with original location of tree prefab

            Instantiate(treeOBJs[treeNumber], randomLocationInRadius, Quaternion.identity);
        }
        MakeTrees = false;
    }

    private void GetWireframeGizmos()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, -Vector3.up, out hitInfo, 1000f, ground))
        {
            wireLocation = hitInfo.point;
        }

        randomLocationInRadius = wireLocation + Random.insideUnitSphere * radius;


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(wireLocation, radius);
    }
    
}
