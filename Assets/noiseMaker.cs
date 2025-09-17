using UnityEngine;

public class noiseMaker : MonoBehaviour
{
    [SerializeField] AudioClip noise;

    private void Start()
    {
        AudioController.instance.PlayAudioClipLooped(noise, transform);
    }
}
