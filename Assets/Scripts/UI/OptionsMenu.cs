using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public void DoToggleMenu(InputAction.CallbackContext ctx) 
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }


}
