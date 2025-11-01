
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine.UI;
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

  

    [Header("Settings")]
    [SerializeField] CinemachineCamera settingsCamera;
    [SerializeField] GameObject settingsMenuObject;
    [SerializeField] Selectable settingsDefaultSelected;

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
        //Main Menu
        mainCamera.gameObject.SetActive(true);
        mainMenuObject.SetActive(true);

        //Gallery
        galleryCamera.gameObject.SetActive(false);
        galleryMenuObject.SetActive(false);

        //Settings
        settingsCamera.gameObject.SetActive(false);
        settingsMenuObject.SetActive(false);

        eventSystem.SetSelectedGameObject(menuDefaultSelected.gameObject);
    }
    public void GalleryMenu( )
    {
        //Gallery
        galleryCamera.gameObject.SetActive(true);
        galleryMenuObject.SetActive(true);

        //Main Menu
        mainCamera.gameObject.SetActive(false);
        mainMenuObject.SetActive(false);

        //Settings
        settingsCamera.gameObject.SetActive(false);
        settingsMenuObject.SetActive(false);

        eventSystem.SetSelectedGameObject(galleryDefaultSelected.gameObject);
    }
    public void Settings()
    {
        //Settings
        settingsCamera.gameObject.SetActive(true);
        settingsMenuObject.SetActive(true);

        //Main Menu
        mainCamera.gameObject.SetActive(false);
        mainMenuObject.SetActive(false);

        //Gallery
        galleryCamera.gameObject.SetActive(false);
        galleryMenuObject.SetActive(false);

        eventSystem.SetSelectedGameObject(settingsDefaultSelected.gameObject);
    }

}
