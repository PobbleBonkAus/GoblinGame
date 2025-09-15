using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;

    public void StartGame() 
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void OpenOptions() 
    {
        optionsMenu.SetActive(true);
    }

    public void ExitGame() 
    {
        Application.Quit();
    }
}
