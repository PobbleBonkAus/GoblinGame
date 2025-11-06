using UnityEngine;
using UnityEngine.UI;

public class GalleryScrolling : MonoBehaviour
{
    [SerializeField] Sprite[] galleryImageCollection;
    [SerializeField] SpriteRenderer galleryImage;
    private int galleryCounter;

    private void Start()
    {

        galleryCounter = Mathf.RoundToInt(Random.RandomRange(0, galleryImageCollection.Length - 1));
    }
    private void Update()
    {
        galleryImage.sprite = galleryImageCollection[galleryCounter];
    }
    public void nextImage()
    {
        if (galleryCounter != galleryImageCollection.Length -1)
        {
            galleryCounter++;
        }
        else
        {
            galleryCounter = 0;
        }
    }
    public void prevImage()
    {
        if (galleryCounter != 0)
        {
            galleryCounter--;
        }
        else
        {
            galleryCounter = galleryImageCollection.Length - 1;
        }
    }
}
