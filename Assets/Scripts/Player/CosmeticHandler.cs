using UnityEngine;

public class CosmeticHandler : MonoBehaviour
{

    [SerializeField] private Transform cosmeticTransform;


    private GameObject currentCosmetic;

    public void Update()
    {
        if (currentCosmetic != null)
        {
            currentCosmetic.transform.SetPositionAndRotation(cosmeticTransform.position, cosmeticTransform.rotation);
        }
    }

    public void TryEquipCosmetic(GameObject cosmetic)
    {
        if (currentCosmetic == null)
        {
            EquipCosmetic(cosmetic);
        }
        else
        {
            UnequipCosmetic();
        }
    }

    public void EquipCosmetic(GameObject cosmetic) 
    {
        currentCosmetic = cosmetic;
        currentCosmetic.GetComponent<Rigidbody>().isKinematic = true;
        currentCosmetic.gameObject.layer = LayerMask.NameToLayer("Cosmetic");
        currentCosmetic.GetComponent<Collider>().enabled = false;
        currentCosmetic.transform.SetPositionAndRotation(cosmeticTransform.position + Vector3.up, cosmeticTransform.rotation);
    }



    public void UnequipCosmetic() 
    {
        currentCosmetic.GetComponent<Rigidbody>().isKinematic = false;
        currentCosmetic.gameObject.layer = LayerMask.NameToLayer("Grabbable");
        currentCosmetic.GetComponent<Collider>().enabled = true;
        currentCosmetic.transform.SetPositionAndRotation(transform.position + transform.forward * 1.0f, cosmeticTransform.rotation);

        currentCosmetic = null;
    }

}
