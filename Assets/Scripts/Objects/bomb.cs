using UnityEngine;

public class bomb : InteractableRigidbody
{
    [SerializeField] float bombTime = 3.0f;
    [SerializeField] float bombExplosionForce = 50.0f;
    [SerializeField] float bombExplosionRadius = 30.0f;

    [SerializeField] Transform wick;
    [SerializeField] ParticleSystem fuseParticle;
    [SerializeField] GameObject explosion;
    private float currentBombTime = 0.0f;


    private void Awake()
    {
        currentBombTime = 3.0f;
    }

    public override void ActivateObject()
    {
        base.ActivateObject();
        fuseParticle.gameObject.active = true;
        fuseParticle.Play();
    }


    private void Update()
    {
        if (isActivated) 
        {
            currentBombTime -= Time.deltaTime;
            Countdown();
        }

    }

    void Countdown() 
    {
        Vector3 targetScale = new Vector3(wick.transform.localScale.x, 0.01f, wick.transform.localScale.z);

        wick.transform.localScale = Vector3.MoveTowards(wick.transform.localScale, targetScale, currentBombTime);

        if(currentBombTime < 0.0f)
        {
            Explode();
        }
    }

    void Explode() 
    {
        // get all colliders in radius that are on the damageLayerMask
        Collider[] hits = Physics.OverlapSphere(transform.position, bombExplosionRadius);
        foreach (Collider hit in hits)
        {
            // apply physics force if rigidbody present
            Rigidbody hitRb = hit.attachedRigidbody;
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(bombExplosionForce, transform.position, bombExplosionRadius, 1.0f, ForceMode.Impulse);
            }
        }

        GameObject explosionObj = Instantiate(explosion);
        explosionObj.transform.position = transform.position;

        Destroy(gameObject);
    }
    

}
