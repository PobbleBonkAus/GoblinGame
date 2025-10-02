using System.Collections.Generic;
using UnityEngine;

public class HatBoard : MonoBehaviour
{

    [SerializeField] GameObject[] hats;
    [SerializeField] GameObject[] hatStickers;

    public static HatBoard instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach(GameObject hatSticker in hatStickers) 
        {
            hatSticker.SetActive(false);
        }
    }

    public GameObject RetrieveHat() 
    {
        int hatIndex = Random.Range(0, hats.Length);
        hatStickers[hatIndex].SetActive(true);
        return hats[hatIndex];
    }


}
