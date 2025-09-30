using System.Collections.Generic;
using UnityEngine;

public class Gyser : MonoBehaviour
{

    [SerializeField]
    private float timer = 1.0f;
    [SerializeField]
    private float gyserForce = 1000.0f;
    [SerializeField]
    private Transform gyserDirectionTransform;
    [SerializeField] ParticleSystem gyserParticles;

    bool shooting = false;
    void Start()
    {
        StartCoroutine(ShootWater());   
    }


    IEnumerator<WaitForSeconds> ShootWater() 
    {
        shooting = false;
        yield return new WaitForSeconds(timer);
        shooting = true;
        gyserParticles.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        if (other.attachedRigidbody != null)
        {
            if (shooting)
            {
                other.attachedRigidbody.AddForce(transform.up * gyserForce, ForceMode.Impulse);
                if(other.TryGetComponent<PlayerController>(out var player)) 
                {
                    player.StartRagdoll();
                }
                StartCoroutine(ShootWater());
            }          
        }
    }
}
