using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public static MenuButtons instance {get; private set;}

    #region Variables
    [SerializeField]
    private GameObject pauseButton = null;
    [SerializeField]
    private GameObject speedX1Button = null;
    [SerializeField]
    private GameObject speedX2Button = null;
    [SerializeField]
    private GameObject settingsButton = null;
    [SerializeField]
    private GameObject buildingsPanelSlidingButton = null;

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color pressedColor;

    private UIManager uiRef;

    public bool buttonsEnabled {get; private set;}
    private bool timeStopped;

    #endregion

    #region Mono Behaviour
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        uiRef = UIManager.Instance;
        PauseGame();
        settingsButton.GetComponent<Button>().interactable = false;
        StartCoroutine(WaitForTutorialEnquiryAnswer());
        DisableButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && buttonsEnabled)
        {
            if(timeStopped)
            {
                SetNormalGameSpeed();
            }
            else
            {
                PauseGame();
            }
        }
    }
    #endregion

    #region Component

    public void PauseGame()
    {
        uiRef.GamePaused = true;
        uiRef.SetGameSpeed(0.0f);
        pauseButton.GetComponent<Image>().color = pressedColor;
        speedX1Button.GetComponent<Image>().color = normalColor;
        speedX2Button.GetComponent<Image>().color = normalColor;
        timeStopped = true;
    }
    public void SetNormalGameSpeed()
    {
        uiRef.GamePaused = false;
        uiRef.SetGameSpeed(1.0f);
        pauseButton.GetComponent<Image>().color = normalColor;
        speedX1Button.GetComponent<Image>().color = pressedColor;
        speedX2Button.GetComponent<Image>().color = normalColor;
        timeStopped = false;
    }
    public void SetFasterGameSpeed()
    {
        uiRef.GamePaused = false;
        uiRef.SetGameSpeed(2.0f);
        pauseButton.GetComponent<Image>().color = normalColor;
        speedX1Button.GetComponent<Image>().color = normalColor;
        speedX2Button.GetComponent<Image>().color = pressedColor;
        timeStopped = false;
    }

    public void DisableButtons()
    {
        pauseButton.GetComponent<Button>().interactable = false;
        speedX1Button.GetComponent<Button>().interactable = false;
        speedX2Button.GetComponent<Button>().interactable = false;
        settingsButton.GetComponent<Button>().interactable = false;
        buildingsPanelSlidingButton.GetComponent<Button>().interactable = false;
        buttonsEnabled = false;
    }

    public void EnableButtons()
    {
        pauseButton.GetComponent<Button>().interactable = true;
        speedX1Button.GetComponent<Button>().interactable = true;
        speedX2Button.GetComponent<Button>().interactable = true;
        settingsButton.GetComponent<Button>().interactable = true;
        buildingsPanelSlidingButton.GetComponent<Button>().interactable = true;
        buttonsEnabled = true;
    }

    private IEnumerator WaitForTutorialEnquiryAnswer()
    {
        while (NotificationManager.Instance.TutorialEnquiryAnswered == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        EnableButtons();
    }

    #endregion
}
