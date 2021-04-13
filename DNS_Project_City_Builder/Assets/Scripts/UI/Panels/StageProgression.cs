using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgression : MonoBehaviour
{
    public ObjectivePanel objective;
    public GameObject timeManipulationRefObject;

    [SerializeField]
    private AudioSource goalReachedSound, progressSound;

    void Awake()
    {
        objective.OnGoalReached += GoalReached;
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        objective.OnGoalReached -= GoalReached;
    }

    // TODO: Add per button action
    public void OptionA()
    {
        UIManager.Instance.SelectStage3Variant(1);
        Progress();
    }

    public void OptionB()
    {

        UIManager.Instance.SelectStage3Variant(2);
        Progress();
    }

    // TODO: Redesign
    private void Progress()
    {
        if(progressSound.clip != null)
        {
            progressSound.Play();
        }
        else
        {
            Debug.Log("Progress sound hasn't been assigned (StageProgression.cs).");
        }
        timeManipulationRefObject.GetComponent<MenuButtons>().SetNormalGameSpeed();
        timeManipulationRefObject.GetComponent<MenuButtons>().EnableButtons();
        TrapPanelController.Instance.gameObject.SetActive(true);//test this
        TrapPanelController.Instance.EnableButtons();
        GameManager.Instance.StageProgress();
        objective.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    // TODO: Redesign
    public void GoalReached()
    {
        // STORY
        NotificationManager.Instance.AddNotification("The Bloom", "The Bloom has to persist... Dark forces are very close. It is time to choose your destiny, strengthen the Spirits and defend The Forest.", false);

        // TUTORIAL
        NotificationManager.Instance.AddNotification("Next stage", "It is time for you to decide. You can choose your main element. You Spirits and buildings are going to be infused with its power and upgraded. Whatever you choose, choose wisely.");
        NotificationManager.Instance.AddNotification("Further events", "When you choose the element, the game is going to advance into another stage. Your objective will be TO SURVIVE. You're going to be attacked by the dark forces. New buildings will be available to construct. Try to use them well and survive for as long as you can.");

        ////////////////////////////        
        gameObject.SetActive(true);
        timeManipulationRefObject.GetComponent<MenuButtons>().PauseGame();
        timeManipulationRefObject.GetComponent<MenuButtons>().DisableButtons();
        TrapPanelController.Instance.DisableButtons();
        if(goalReachedSound.clip != null)
        {
            goalReachedSound.Play();
        }
        else
        {
            Debug.Log("Goal reached sound hasn't been assigned (StageProgression.cs).");
        }
    }
}
