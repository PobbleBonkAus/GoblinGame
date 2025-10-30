using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectingUIObject : MonoBehaviour
{
    [Header("StartUp")]
    [SerializeField] MainMenuScript mainMenu;
    [SerializeField] Selectable selectToElement;

    private void Start()
    {
        
    }
}
