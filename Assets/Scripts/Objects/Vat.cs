using UnityEngine;
using UnityEngine.Events;

public class Vat : MonoBehaviour
{
    [Header("Vat Settings")]
    [SerializeField] Transform vatLiquidTransform;
    [SerializeField] float vatDrainSpeed = 0.5f;   //per second
    [SerializeField] float vatFillSpeed = 2.0f;
    [SerializeField] int maxLiquid = 10;

    [SerializeField] Transform marker;

    [Header("Events")]
    [SerializeField] UnityEvent OnCompletion;
    [SerializeField] UnityEvent OnFirstMilestone;
    [SerializeField] UnityEvent OnSecondMilestone;
    [SerializeField] UnityEvent OnThirdMilestone;

    [SerializeField] private int firstMilestone = 3;
    [SerializeField] private int secondMilestone = 6;
    [SerializeField] private int thirdMilestone = 9;

    private bool draining = false;

    private float currentLiquid = 0f;
    private float targetLiquid = 0f;

    private bool firstTriggered = false;
    private bool secondTriggered = false;
    private bool thirdTriggered = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddToVat(1);
        }

        
        currentLiquid = Mathf.MoveTowards(currentLiquid, targetLiquid, vatFillSpeed * Time.deltaTime);

        if (draining)
        {
            DrainVat();
        }

        UpdateVisuals();
        CheckMilestones();
    }

    public void AddToVat(int input)
    {
        targetLiquid = Mathf.Clamp(targetLiquid + input, 0, maxLiquid);

        if (Mathf.Approximately(targetLiquid, maxLiquid))
        {
            CompleteVat();
        }
    }

    void DrainVat()
    {
        targetLiquid -= vatDrainSpeed * Time.deltaTime;
        if (targetLiquid <= 0f)
        {
            targetLiquid = 0f;
            draining = false;
        }
    }

    void CompleteVat()
    {
        draining = true;
        Debug.Log("Complete");
        OnCompletion.Invoke();
    }

    void UpdateVisuals()
    {
        if (!vatLiquidTransform) return;

        float fillPercent = Mathf.InverseLerp(0, maxLiquid, currentLiquid);

        vatLiquidTransform.localScale = new Vector3(
            vatLiquidTransform.localScale.x,
            Mathf.Lerp(0.1f, 1.0f, fillPercent),
            vatLiquidTransform.localScale.z
        );

        if (marker)
        {
            marker.localPosition = new Vector3(
                marker.localPosition.x,
                Mathf.Lerp(1.5f, 15f, fillPercent),
                marker.localPosition.z
            );
        }
    }

    void CheckMilestones()
    {
        if (!firstTriggered && currentLiquid >= firstMilestone)
        {
            firstTriggered = true;
            OnFirstMilestone.Invoke();
        }

        if (!secondTriggered && currentLiquid >= secondMilestone)
        {
            secondTriggered = true;
            OnSecondMilestone.Invoke();
        }

        if (!thirdTriggered && currentLiquid >= thirdMilestone)
        {
            thirdTriggered = true;
            OnThirdMilestone.Invoke();
        }
    }
}
