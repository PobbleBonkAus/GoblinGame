using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("FullScreenMode")]
    [SerializeField] FullScreenMode[] fullScreenMode;
    private int fullscreenCounter = 0;

    [SerializeField] TextMeshProUGUI windowModeText;

    [Header("Main Theme")]
    [SerializeField] AudioClip[] gameThemes;
    private int themeCounter = 0;

    [SerializeField] TextMeshProUGUI themeText;
    [SerializeField] AudioSource audioSource;

    [Header("Volume Sliders")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;



    private void Start()
    {
        //Window Mode Startup
        windowModeText.text = Screen.fullScreenMode.ToString();
        
        //Theme Startup
        themeText.text = themeToString(0);
        
        //Music Startup
        musicSlider.value = audioSource.volume;
        
    }
    #region FullScreenMode
    public void nextFSC()
    {
        if (fullscreenCounter != fullScreenMode.Length - 1)
        {
            fullscreenCounter++;
        }
        else
        {
            fullscreenCounter = 0;
        }
        windowModeText.text = windowModeString(fullScreenMode[fullscreenCounter]);
        Screen.fullScreenMode = fullScreenMode[fullscreenCounter];
    } 
    public void prevFSC()
    {
        if (fullscreenCounter != 0)
        {
            fullscreenCounter--;
        }
        else
        {
            fullscreenCounter = fullScreenMode.Length - 1;
            
        }
        windowModeText.text = windowModeString(fullScreenMode[fullscreenCounter]);
        Screen.fullScreenMode = fullScreenMode[fullscreenCounter];
    }
    public string windowModeString(FullScreenMode currentMode)
    {
        if (currentMode == FullScreenMode.Windowed)
        {
            return "Windowed";
        }
        else if (currentMode == FullScreenMode.FullScreenWindow)
        {
            return "Borderless";
        }
        else if (currentMode == FullScreenMode.ExclusiveFullScreen)
        {
            return "Fullscreen";
        }
        else if (currentMode == FullScreenMode.MaximizedWindow)
        {
            return "Max Windowed";
        }

        return "No Window";
    }
    #endregion

    #region MainTheme
    public void nextTheme()
    {
        if (themeCounter != gameThemes.Length - 1)
        {
            themeCounter++;
        }
        else
        {
            themeCounter = 0;
        }
        playNewTheme();
    }
    public void prevTheme()
    {
        if (themeCounter != 0)
        {
            themeCounter--;
        }
        else
        {
            themeCounter = gameThemes.Length - 1;

        }
       
        playNewTheme();
    }
    public string themeToString(int currentTheme)
    {
        if (currentTheme == 0)
        {
            return "Main Menu";
        }
        else if (currentTheme == 1)
        {
            return "Junkyard";
        }
        else if (currentTheme == 2)
        {
            return "Mushroom";
        }
        else if (currentTheme == 3)
        {
            return "Wizard";
        }
        else if (currentTheme == 4)
        {
            return "Radio Goblin";
        }


        return "No Theme";
    }

    public void playNewTheme()
    {
        themeText.text = themeToString(themeCounter);
        audioSource.clip = gameThemes[themeCounter];
        audioSource.Play();
    }
    #endregion

    #region Volume
    public void musicVolumeChange()
    {
        audioSource.volume = musicSlider.value;
    }


    #endregion
}
