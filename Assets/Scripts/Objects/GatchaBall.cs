using UnityEngine;

public class GatchaBall : InteractableRigidbody
{
    [SerializeField] GameObject[] hats;

    public override void ActivateObject()
    {
        base.ActivateObject();
        //spawn hat
        GameObject instance = Instantiate(hats[Random.Range(0, hats.Length)]);
        instance.transform.position = transform.position;

        Destroy(gameObject);
    }
}
