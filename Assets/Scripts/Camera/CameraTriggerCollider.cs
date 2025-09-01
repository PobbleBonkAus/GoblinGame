using UnityEngine;
using Unity.Cinemachine;
public class CameraTriggerCollider : MonoBehaviour
{
    public CinemachineCamera cam;
    public BoxCollider boxCollider;
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (CameraSwitcher.ActiveCamera != cam) CameraSwitcher.SwitchCamera(cam);
            Debug.Log(cam + "activated");
        }
    }
}
