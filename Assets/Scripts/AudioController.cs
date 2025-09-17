using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] AudioSource source;

    [SerializeField] float maxAudioRange = 100.0f;
    [SerializeField] float minAudioRange = 1.0f;

    public static AudioController instance { get; private set; }
    int maxAudioSources = 30;

    Dictionary<AudioSource,Transform> audioSources;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        audioSources = new Dictionary<AudioSource, Transform>();

        for(int i = 0; i < maxAudioSources; i++) 
        {
            audioSources[gameObject.AddComponent<AudioSource>()] = transform;
        }
    }

    private void Update()
    {
        UpdateAudioSourceVolumes();
    }

    private void UpdateAudioSourceVolumes() 
    {
        foreach(AudioSource source in audioSources.Keys) 
        {
            if (source.isPlaying) 
            {
                source.volume = GetVolumeFromSource(audioSources[source]);
            }
        }
    }

    public void PlayAudioClip(AudioClip clip, Transform audioOrigin) 
    {
        foreach (AudioSource audioSource in audioSources.Keys)
        {
            if (audioSource.isPlaying)
            {
                continue;
            }
            else
            {
                audioSource.volume = GetVolumeFromSource(audioSources[audioSource]);
                audioSource.PlayOneShot(clip);
                audioSources[audioSource] = audioOrigin;
                break;
            }
        }
    }

    public void PlayAudioClipLooped(AudioClip clip, Transform audioOrigin) 
    {
        foreach (AudioSource audioSource in audioSources.Keys)
        {
            if (audioSource.isPlaying)
            {
                continue;
            }
            else
            {
                audioSource.loop = true;
                audioSource.clip = clip;
                audioSource.volume = GetVolumeFromSource(audioSources[audioSource]);
                audioSource.Play();
                audioSources[audioSource] = audioOrigin;

                break;
            }
        }
    }


    public float DistanceToNearestPlayer(Vector3 audioOrigin) 
    {
        float distanceToNearestPlayer = maxAudioRange + 1.0f;

        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            if (GameManager.instance.players[i] == null) continue;

            float distance = Vector3.Distance(audioOrigin, GameManager.instance.players[i].transform.position);
            if (distance < distanceToNearestPlayer)
            {
                distanceToNearestPlayer = distance;
            }
        }

        return distanceToNearestPlayer;
    }


    public float GetVolumeFromSource(Transform sourceTransform)
    {
        float distance = DistanceToNearestPlayer(sourceTransform.position);

        if (distance <= minAudioRange)
            return 1f; // Full volume up close

        if (distance >= maxAudioRange)
            return 0f; // Out of range

        // Linear falloff
        float t = (distance - minAudioRange) / (maxAudioRange - minAudioRange);
        return Mathf.Clamp01(1f - t);
    }


}
