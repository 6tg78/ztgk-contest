using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeKeyHandler : MonoBehaviour
{
    public static EscapeKeyHandler Instance {get; private set;}
    public GameObject InGameMenu {get {return inGameMenu;} private set {inGameMenu = value;}}


    [SerializeField]
    private GameObject inGameMenu;
    [SerializeField]
    private GameObject menuButton;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) && !inGameMenu.activeInHierarchy && NotificationManager.Instance.TutorialEnquiryAnswered)
        {
            if(BuildingsManager.Instance.InConstrucionPlanningMode)
            {
                BuildingsManager.Instance.QuitConstrucionPlanningMode();
            }
            else
            {
                menuButton.GetComponent<Button>().onClick.Invoke();
            }
        }
    }
}
