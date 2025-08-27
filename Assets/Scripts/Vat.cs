using UnityEngine;
using UnityEngine.Events;

public class Vat : MonoBehaviour
{
    [SerializeField] float targetHeight;
    [SerializeField] Transform vatLiquidTransform;
    [SerializeField] float vatDrainSpeed = 0.5f;

    [SerializeField] UnityEvent OnCompletion;
    bool draining;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            AddToVat(1);
        }

        if (draining) 
        {
            DrainVat();
        }
    }

    public void AddToVat(int amount) 
    {
        vatLiquidTransform.localPosition = new Vector3(vatLiquidTransform.localPosition.x, vatLiquidTransform.localPosition.y + amount, vatLiquidTransform.localPosition.z);
        if(vatLiquidTransform.localPosition.y > targetHeight) 
        {
            CompleteVat();
        }
    }

    void DrainVat() 
    {
        vatLiquidTransform.localPosition = new Vector3(vatLiquidTransform.localPosition.x,
        vatLiquidTransform.localPosition.y - vatDrainSpeed,
        vatLiquidTransform.localPosition.z);

        if (vatLiquidTransform.localPosition.y < -8)
        {
            draining = false;
        }
    }

    void CompleteVat() 
    {
        draining = true;
        OnCompletion.Invoke();
    }

}
