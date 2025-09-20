using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody targetRb;   // sphere rigidbody
    [SerializeField] private Transform model;      // visual mesh

    [Header("Settings")]
    [SerializeField] private float impactSquashFactor = 0.3f;
    [SerializeField] private float spring = 8f;
    [SerializeField] private float minImpactSpeed = 1f;
    [SerializeField] private float maxScale = 3.5f;


    Vector3 originalScale;

    private Vector3 currentScale;
    private Vector3 targetScale;
    private Vector3 scaleVel;

    private void Awake()
    {
        if (!targetRb) targetRb = GetComponent<Rigidbody>();
        if (!model) model = transform;
        originalScale = model.localScale;
        currentScale = Vector3.one;
        targetScale = Vector3.one;
    }

    private void LateUpdate()
    {
        // Smooth spring interpolation
        currentScale = Vector3.SmoothDamp(
            currentScale,
            targetScale,
            ref scaleVel,
            1f / spring,
            Mathf.Infinity,
            Time.deltaTime
        );

        targetScale = Vector3.Lerp(targetScale, originalScale, 0.1f);

        model.localScale = currentScale;
    }

    public void SquashOnImpact(Collision impact)
    {
        float impactSpeed = impact.relativeVelocity.magnitude;

        if (impactSpeed < minImpactSpeed) return;

        Vector3 normal = impact.GetContact(0).normal;

        // squash factor
        float squash = Mathf.Clamp(1f - impactSpeed * impactSquashFactor, 0.3f, 1f);
        float stretch = 1f / Mathf.Sqrt(squash); // volume preserve

        // Build a scale vector in local space that squashes along normal
        Vector3 axis = transform.InverseTransformDirection(normal);
        Vector3 scale = Vector3.one * stretch;
        scale += (squash - stretch) * axis; // squash along the normal axis

        targetScale = Vector3.ClampMagnitude(scale, maxScale);
    }



    private void OnCollisionEnter(Collision collision)
    {
        SquashOnImpact(collision);
    }
}
