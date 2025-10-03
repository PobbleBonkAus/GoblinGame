using UnityEngine;

public class ShrinkPotion : potionBottle
{
   

    public override void ApplyAffect(Rigidbody body)
    {
        base.ApplyAffect(body);
        body.transform.localScale /= 2.0f;
    }

    public override void ReverseAffect(Rigidbody body)
    {
        base.ReverseAffect(body);
        body.transform.localScale *= 2.0f;
    }
}
