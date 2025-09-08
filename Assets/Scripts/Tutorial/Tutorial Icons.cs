using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class TutorialIcons : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite[] controllerIcon;
    [SerializeField] Sprite[] contorllerAction;

    [Header("GameObjects")]
    [SerializeField] Image imageIcon;
    [SerializeField] Image imageAction;
    [SerializeField] Image imageStrikeout;
    [SerializeField] GameObject tutorialPanel;

    [Header("Timing")]
    [SerializeField] float strikeoutSpeed;
    [SerializeField] float resetCountdown;

    [Header("State")]
    [SerializeField] private iconState iconCurrentState;
    private enum iconState { Walk, Jump, Grab, Complete }

    [Header("Player Controller")]
    [SerializeField] PlayerController playerController;

    void IconStateChanger(iconState iconPassThrough)
    {
        imageStrikeout.enabled = true;
        StartCoroutine(resetTutorial(iconPassThrough));
    }


    IEnumerator resetTutorial(iconState newState)
    {
        yield return new WaitForSeconds(resetCountdown);

        if (newState == iconState.Complete) 
        {
            tutorialPanel.gameObject.SetActive(false);
            yield return null;
        }
        else
        {
            imageIcon.sprite = controllerIcon[(int)newState];
            imageAction.sprite = contorllerAction[(int)newState];

            imageStrikeout.enabled = false;
        }
    }

    void UpdateTutorialState() 
    {
        if(iconCurrentState != iconState.Complete) 
        {
            iconCurrentState += 1;
            IconStateChanger(iconCurrentState);
        }
    }

    public void DoJumpAction(InputAction.CallbackContext ctx) 
    {
        if (iconCurrentState == iconState.Jump && !imageStrikeout.enabled) 
        {
            UpdateTutorialState();
        }
    }

    public void DoWalkAction(InputAction.CallbackContext ctx) 
    {
        if (iconCurrentState == iconState.Walk && !imageStrikeout.enabled)
        {
            UpdateTutorialState();
        }
    }

    public void DoGrabAction(InputAction.CallbackContext ctx) 
    {
        if (iconCurrentState == iconState.Grab && !imageStrikeout.enabled)
        {
            UpdateTutorialState();
        }
    }

}
