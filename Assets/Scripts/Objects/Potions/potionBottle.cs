using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class potionBottle : InteractableRigidbody
{
    [Header("Liquid Settings")]
    [SerializeField] float liquidAmount = 4.0f;          // how much liquid total
    [SerializeField] float liquidPourSpeed = 0.3f;       // how fast it drains per second
    [SerializeField] float potionDuration = 5.0f;        // how long effect 

    [Header("References")]
    [SerializeField] Transform liquid;                   // mesh/transform inside bottle
    [SerializeField] ParticleSystem pouringParticle;
    [SerializeField] GameObject affectParticle;
    [SerializeField] Renderer liquidMaterial;
    bool pouring = false;
    

    Dictionary<Rigidbody, float> affectedBodies = new Dictionary<Rigidbody, float>();


    private void Awake()
    {
        if (pouringParticle != null)
            pouringParticle.Stop();
    }

    private void Update()
    {
        TrackAffectedBodies();

        if (!isActivated) return;

        // check angle: how far "up" is pointing away from world-up
        float angle = Vector3.Angle(transform.up, Vector3.up);

        // consider tipped if more than ~90 degrees from upright
        pouring = angle > 100f && liquidAmount > 0f;

        if (pouring && liquidAmount > 0f)
        {
            if (pouringParticle != null && pouringParticle.isStopped)
                pouringParticle.Play();

            // drain
            float drain = liquidPourSpeed * Time.deltaTime;
            liquidAmount = Mathf.Max(0f, liquidAmount - drain);

            // scale whole transform down proportionally
            if (liquid != null)
            {
                float t = liquidAmount / 5f; // assumes 5 is max
                //liquid.localScale = Vector3.one * t;
                liquidMaterial.material.SetFloat("_Fill_Amount", t * 1f);
                //print(t);
            }

        }
        else
        {
            if (pouringParticle != null && pouringParticle.isPlaying)
                pouringParticle.Stop();
        }
    }

    void TrackAffectedBodies()
    {
        // make a copy of keys so we can safely remove entries
        List<Rigidbody> keys = new List<Rigidbody>(affectedBodies.Keys);

        foreach (Rigidbody body in keys)
        {
            affectedBodies[body] -= Time.deltaTime;
            if (affectedBodies[body] <= 0.0f)
            {
                ReverseAffect(body);
                affectedBodies.Remove(body);
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
       
        if (other.GetComponent<Rigidbody>() && other != gameObject)
        {
            if (!affectedBodies.ContainsKey(other.GetComponent<Rigidbody>()))
            {
                affectedBodies.Add(other.GetComponent<Rigidbody>(), potionDuration);
                ApplyAffect(other.GetComponent<Rigidbody>());
            }
            else
            {
                // reset the timer if still being poured on
                affectedBodies[other.GetComponent<Rigidbody>()] = potionDuration;
            }
        }
        
    }

    public virtual void ApplyAffect(Rigidbody body)
    {
        //body.useGravity = false;
        //body.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);

        if(affectParticle != null) 
        {
            GameObject affect = Instantiate(affectParticle);
            affect.transform.parent = body.transform;
            affect.transform.position = body.transform.position;


            var main = affect.GetComponent<ParticleSystem>().main;
            main.duration = potionDuration;

            affect.GetComponent<ParticleSystem>().Play();
        }
    }

    public virtual void ReverseAffect(Rigidbody body)
    {
        //body.useGravity = true;
    }

    private void OnDestroy()
    {
        // clean up all affected bodies when bottle is destroyed
        foreach (Rigidbody body in new List<Rigidbody>(affectedBodies.Keys))
        {
            ReverseAffect(body);
        }
    }
}
