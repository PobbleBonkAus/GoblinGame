using UnityEngine;

public class AntiGravityPotion : potionBottle
{
    public override void ApplyAffect(Rigidbody body)
    {
        base.ApplyAffect(body);
        body.useGravity = false;
        body.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
    }

    public override void ReverseAffect(Rigidbody body)
    {
        base.ReverseAffect(body);
        body.useGravity = true;
    }
}
