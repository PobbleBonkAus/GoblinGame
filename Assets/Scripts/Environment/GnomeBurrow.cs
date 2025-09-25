using System.Collections.Generic;
using UnityEngine;

public class GnomeBurrow : MonoBehaviour
{
    [SerializeField] GnomeHole HoleA;
    [SerializeField] GnomeHole HoleB;
    [SerializeField] float launchForce;
    [SerializeField] float launchDelay;

    void LaunchOut(Rigidbody body, Transform hole)
    {
        body.AddForce(hole.forward * launchForce, ForceMode.Impulse);
    }


    public void OnHoleEnter(Transform hole, Rigidbody body) 
    {
        StartCoroutine(EnterHole(hole, body));
    }

    public IEnumerator<WaitForSeconds> EnterHole(Transform hole, Rigidbody body) 
    {
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;

        if(hole == HoleA.transform)
        {
            body.MovePosition(HoleB.transform.position);
            HoleB.GetComponent<Collider>().enabled = false;
        }
        else if (hole == HoleB.transform)
        {
            body.MovePosition(HoleA.transform.position);
            HoleA.GetComponent<Collider>().enabled = false;
        }
        else
        {
            Debug.Log("What Hole");
        }

            yield return new WaitForSeconds(launchDelay);

        body.isKinematic = false;

        if (hole == HoleA.transform)
        {
            LaunchOut(body, HoleB.transform);
            StartCoroutine(HoleB.ReEnableCollider());
        }
        else
        {
            LaunchOut(body, HoleA.transform);
            StartCoroutine(HoleA.ReEnableCollider());
        }
    }

}
