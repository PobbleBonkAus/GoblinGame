using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class PlayerUI : MonoBehaviour
{
    [Header("Physics Grabber")]
    public PhysicsGrabber physicsGrabber;

    [Header("UI Meter Elements")]
    public UnityEngine.UI.Image meterImage;
    public UnityEngine.UI.Image meterBase;

    float meterScale;
    private void Update()
    {
        if (physicsGrabber.throwForceTimer <= 0)
        {   meterBase.enabled = false;   }
        else
        {   meterBase.enabled = true;   }

        meterScale = physicsGrabber.throwForceTimer / physicsGrabber.maxThrowForceTime;
        meterImage.rectTransform.localScale = new Vector3(1, meterScale, 1);
    }
    
}
