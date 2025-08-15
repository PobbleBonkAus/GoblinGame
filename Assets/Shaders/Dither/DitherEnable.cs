using UnityEngine;
using UnityEngine.InputSystem;

public class DitherEnable : MonoBehaviour
{
    private Renderer ditherMaterial;
    private Renderer ditherMaterialReference;
    [Range(0f,1f)] public float alphaChange;
   
    private void Start()
    {
        
        ditherMaterial = GetComponent<Renderer>();
       
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            colorAlpha(alphaChange);
        }
        else 
        {
            colorAlpha(1f);
        }

    }
    
    private void colorAlpha(float alphaValue)
    {
        //temp += 1 * Time.deltaTime;
        //float lerpCounter = 0;

        //float newAlpha = 1;



        //if (lerpCounter <= 1)
        //{
        //    lerpCounter += 0.01f * Time.deltaTime;
        //}

        //newAlpha = Mathf.Lerp(1, alphaValue, lerpCounter);

        foreach (Renderer render in gameObject.GetComponentsInChildren<Renderer>())
        {
            render.material.SetVector("_BaseValue", new Vector4(1, 1, 1, alphaValue));

        }



    }
}