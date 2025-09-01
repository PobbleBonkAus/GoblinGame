using UnityEngine;
using Unity.Cinemachine;
public class CameraRegister : MonoBehaviour
{
    private void OnEnable()
    {
        CameraSwitcher.Register(GetComponent<CinemachineCamera>());

    }
    private void OnDisable()
    {
        CameraSwitcher.Unregister(GetComponent<CinemachineCamera>());

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
