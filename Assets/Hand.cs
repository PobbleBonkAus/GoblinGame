using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] float clapScale = 2.0f;
    [SerializeField] float clapSpeed = 0.5f;
    [SerializeField] ParticleSystem grabParticleEffect;
    bool clap = false;
    private void Update()
    {
        if (clap) 
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * clapSpeed);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        transform.localScale = Vector3.one * clapScale;
        clap = true;
        grabParticleEffect.Play();
    }
}
