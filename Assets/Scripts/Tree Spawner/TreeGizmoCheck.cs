using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class TreeGizmoCheck : MonoBehaviour
{
    private TreeSpawning tS;
    public LayerMask ground;
    private Vector3 spherePos;
    private void Start()
    {
        tS = GetComponent<TreeSpawning>();
    }
    private void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -Vector3.up, out hitInfo, 1000f, ground))
        {
            spherePos = hitInfo.point;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(spherePos, tS.radius);
    }
}
