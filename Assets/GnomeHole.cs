using System.Collections.Generic;
using UnityEngine;

public class GnomeHole : MonoBehaviour
{
    [SerializeField] GnomeBurrow gnomeBurrow;




    public IEnumerator<WaitForSeconds> ReEnableCollider() 
    {
        yield return new WaitForSeconds(2.0f);
        GetComponent<Collider>().enabled = true;   
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.attachedRigidbody == null) return;

        if (other.CompareTag("Player") && other.TryGetComponent<PlayerController>(out PlayerController playerController)) 
        {
            gnomeBurrow.OnHoleEnter(transform, other.attachedRigidbody);
        }
        else
        {
            if (other.CompareTag("Player")) return;

            gnomeBurrow.OnHoleEnter(transform, other.attachedRigidbody);
        }
    }



}
