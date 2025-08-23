using UnityEngine;

public class TutorialStage : MonoBehaviour
{

    [SerializeField] Transform leftDoor;
    [SerializeField] Transform rightDoor;

    [SerializeField] float doorOpenSpeed = 0.4f;

    [SerializeField] Vector3 doorOpenOffset;
    Vector3 leftDoorTarget;
    Vector3 rightDoorTarget;

    public bool tutorialComplete;

    private void Start()
    {
        leftDoorTarget = leftDoor.position + doorOpenOffset;
        rightDoorTarget = rightDoor.position - doorOpenOffset;
    }

    private void Update()
    {
        if (tutorialComplete) 
        {
            rightDoor.position = Vector3.Lerp(rightDoor.position, rightDoorTarget, doorOpenSpeed);
            leftDoor.position = Vector3.Lerp(leftDoor.position, leftDoorTarget, doorOpenSpeed);
        }   
    }



    public void CompleteTutorial() 
    {
        tutorialComplete = true;
    }
    

}
