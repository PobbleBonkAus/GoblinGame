using System.Collections;
using UnityEngine;

public class GatchaBall : InteractableRigidbody
{
    [SerializeField] GameObject particleObject;
    [SerializeField] AudioClip openAudio;
    public override void ActivateObject(PhysicsGrabber grabber)
    {
        base.ActivateObject(grabber);
        //spawn hat
        AudioController.instance.PlayAudioClip(openAudio,transform);

        GameObject instance = Instantiate(HatBoard.instance.RetrieveHat());
        instance.transform.position = transform.position;

        GameObject particle = Instantiate(particleObject);
        particle.transform.position = transform.position + transform.up * 0.2f;

        StartCoroutine(DestroyAferTime());
    }

    IEnumerator DestroyAferTime() 
    {
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
    
}
