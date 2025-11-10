using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void DoToggleMenu(InputAction.CallbackContext ctx) 
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }


}
