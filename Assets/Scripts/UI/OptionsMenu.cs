using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] TMP_Text volumeAmountText;
    [SerializeField] TMP_Text sensitivityAmountText;

    [SerializeField] Scrollbar volumeSlider;
    [SerializeField] Scrollbar sensitivitySlider;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptionsMenu();
        }
    }

    public void DoToggleMenu(InputAction.CallbackContext ctx) 
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void CloseOptionsMenu() 
    {
        gameObject.SetActive(false);
    }

    public void UpdateVolumeUI() 
    {
        volumeAmountText.text = volumeSlider.value.ToString("f1");
    }

    public void UpdateSensitivityUI() 
    {
        sensitivityAmountText.text = sensitivitySlider.value.ToString("f1");
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
