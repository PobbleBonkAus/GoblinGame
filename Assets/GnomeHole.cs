using System.Collections.Generic;
using UnityEngine;

public class GnomeHole : MonoBehaviour
{
    [SerializeField] GnomeBurrow gnomeBurrow;

    private void OnTriggerEnter(Collider other)
    {

        if (other.attachedRigidbody == null) return;

        if (other.CompareTag("Player") && other.TryGetComponent<PlayerController>(out PlayerController playerController)) 
        {
            //the player has entered
            gnomeBurrow.OnHoleEnter(transform, other.attachedRigidbody);
        }
        else
        {
            if (other.CompareTag("Player")) return;
            //an object that is not the player has entered
            gnomeBurrow.OnHoleEnter(transform, other.attachedRigidbody);
        }
    }

    public IEnumerator<WaitForSeconds> ReEnableCollider() 
    {
        yield return new WaitForSeconds(3.0f);
        GetComponent<Collider>().enabled = true;
    }
}
