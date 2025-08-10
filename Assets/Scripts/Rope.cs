using UnityEngine;

public class Rope : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody[] segements;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segements = GetComponentsInChildren<Rigidbody>();

        lineRenderer.positionCount = segements.Length;
    }

    private void Update()
    {
        UpdateLineRenderer();
    }

    void UpdateLineRenderer() 
    {
        for(int i = 0; i < lineRenderer.positionCount; i++) 
        {
            lineRenderer.SetPosition(i, segements[i].position);
        }
    }
}
