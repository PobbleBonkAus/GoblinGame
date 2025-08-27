using UnityEngine;
using Unity.UI;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.LowLevel;
public class TutorialIcons : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite[] controllerIcon;
    [SerializeField] Sprite[] contorllerAction;

    [Header("GameObjects")]
    [SerializeField] Image imageIcon;
    [SerializeField] Image imageAction;
    [SerializeField] Image imageStrikeout;

    [Header("Timing")]
    [SerializeField] float strikeoutSpeed;
    [SerializeField] float resetCountdown;

    [Header("State")]
    [SerializeField] private iconState iconCurrentState;
    private enum iconState { Walk, Jump, Grab, Complete }

    [Header("Player Controller")]
    [SerializeField] PlayerController playerController;
    private bool tutorialWalk = false;
    private bool tutorialJump = false;
    private bool tutorialGrab = false;
    private void Awake()
    {
        iconCurrentState = iconState.Walk;
    }
    private void Update()
    {
        IconStateChanger(iconCurrentState);
        tutorialWalk = playerController.playerWalk;
        tutorialJump = playerController.playerJump;
        tutorialGrab = playerController.playerGrab;
    }
    void IconStateChanger(iconState iconPassThrough)
    {
        switch (iconCurrentState)
        {
            case iconState.Walk:
                imageIcon.sprite = controllerIcon[0];
                imageAction.sprite = contorllerAction[0];
                if (tutorialWalk)
                {
                    imageTransition(iconState.Jump);
                }
                break;
            case iconState.Jump:
                imageIcon.sprite = controllerIcon[1];
                imageAction.sprite = contorllerAction[1];
                if (tutorialJump)
                {
                    imageTransition(iconState.Grab);
                }
                break;
            case iconState.Grab:
                imageIcon.sprite = controllerIcon[2];
                imageAction.sprite = contorllerAction[2];
                if (tutorialGrab)
                {
                    imageTransition(iconState.Complete);
                }
                break;
            case iconState.Complete:
                imageIcon.enabled = false;
                imageAction.enabled = false;
                break;
        }
    }
    void imageTransition(iconState newState)
    {
        if (!imageStrikeout.enabled)
        {
            imageStrikeout.enabled = true;    
        }
        StartCoroutine(resetTutorial(newState));
        
    }
    IEnumerator resetTutorial(iconState newState)
    {
        yield return new WaitForSeconds(resetCountdown);
        iconCurrentState = newState;

        imageStrikeout.enabled = false;

    }
}
