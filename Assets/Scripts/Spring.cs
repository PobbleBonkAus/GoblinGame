using Unity;
using UnityEngine;

public static class Spring
{
    /// <summary>
    /// Works like Vector3.Lerp but with a spring effect.
    /// t is assumed to be normalized [0..1].
    /// </summary>
    public static Vector3 Lerp(Vector3 from, Vector3 to, float t, float frequency = 0.3f, float damping = 0.04f)
    {
        if (t <= 0f) return from;
        if (t >= 1f) return to;

        // Base linear interpolation
        Vector3 linear = Vector3.Lerp(from, to, t);

        // Spring offset factor
        float oscillation = Mathf.Sin(t * frequency * Mathf.PI);
        float decay = Mathf.Exp(-damping * t);
        float springFactor = oscillation * decay;

        // Apply spring in the direction of motion
        return linear + (to - from) * springFactor;
    }
}