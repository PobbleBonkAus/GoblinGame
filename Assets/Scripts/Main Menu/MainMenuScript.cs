using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor.UI;
using UnityEngine.EventSystems;



public class MainMenuScript : MonoBehaviour
{
    [Header("StartUp")]
    [SerializeField] EventSystem eventSystem;

    [Header("Main Menu")]
    [SerializeField] CinemachineCamera mainCamera;
    [SerializeField] GameObject mainMenuObject;
    [SerializeField] Selectable menuDefaultSelected;
   


    [Header("Gallery")]
    [SerializeField] CinemachineCamera galleryCamera;
    [SerializeField] GameObject galleryMenuObject;
    [SerializeField] Selectable galleryDefaultSelected;

    [SerializeField] Sprite[] goblinPhotoList;






    private void Awake()
    {
        MainMenu();
        
    }

    public void PlayGame() 
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void MainMenu( )
    {
        mainCamera.gameObject.SetActive(true);
        mainMenuObject.SetActive(true);

        galleryCamera.gameObject.SetActive(false);
        galleryMenuObject.SetActive(false);

        eventSystem.SetSelectedGameObject(menuDefaultSelected.gameObject);
    }
    public void GalleryMenu( )
    {
        galleryCamera.gameObject.SetActive(true);
        galleryMenuObject.SetActive(true);

        mainCamera.gameObject.SetActive(false);
        mainMenuObject.SetActive(false);

        eventSystem.SetSelectedGameObject(galleryDefaultSelected.gameObject);

    }

    
}
