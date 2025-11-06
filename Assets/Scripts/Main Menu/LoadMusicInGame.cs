using UnityEngine;

public class LoadMusicInGame : MonoBehaviour
{
    
    void Start()
    {
        Debug.Log(MenuStatic.musicClipGlobal);
        gameObject.GetComponent<AudioSource>().volume = MenuStatic.musicVolumeGlobal;    
        gameObject.GetComponent<AudioSource>().clip = MenuStatic.musicClipGlobal;   
        gameObject.GetComponent<AudioSource>().Play();
        
    }

    
}
