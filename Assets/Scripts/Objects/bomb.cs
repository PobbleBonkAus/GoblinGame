using UnityEngine;

public class bomb : InteractableRigidbody
{
    [SerializeField] float bombTime = 3.0f;
    [SerializeField] float bombExplosionForce = 50.0f;
    [SerializeField] float bombExplosionRadius = 30.0f;

    [SerializeField] Transform wick;
    [SerializeField] Transform fusePoint;
    [SerializeField] ParticleSystem fuseParticle;
    [SerializeField] GameObject explosion;

    [SerializeField] AudioClip sparkNoise;

    private float currentBombTime = 0.0f;


    private void Awake()
    {
        currentBombTime = bombTime;
        fuseParticle.Stop();
    }

    public override void ActivateObject(PhysicsGrabber grabber)
    {
        base.ActivateObject(grabber);
        fuseParticle.Play();
    }

    public override void DeactivateObject()
    {
        //override so we dont deactivate it
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
        float t = 1f - (currentBombTime / bombTime); // 0 → 1
        wick.localScale = new Vector3(
            wick.localScale.x,
            Mathf.Lerp(1f, 0.01f, t),
            wick.localScale.z
        );

        if(currentBombTime < 0.3f) 
        {
            transform.localScale = new Vector3(
            Mathf.Lerp(1f, 1.5f, t),
            Mathf.Lerp(1f, 1.5f, t),
            Mathf.Lerp(1f, 1.5f, t)
            );
        }

        if (currentBombTime < 0.0f)
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > 30.0f) 
        {
            fuseParticle.Play();
            isActivated = true;
        }
    }
}
