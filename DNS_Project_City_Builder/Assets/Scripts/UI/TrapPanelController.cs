using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrapPanelController : MonoBehaviour
{
    public static TrapPanelController Instance;

    public TextMeshProUGUI explosingCounter, stunningCounter;
    public Button explosingTrapButton, stunningTrapButton;
    public GameObject explosingTrapPrefab, stunningTrapPrefab;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        gameObject.SetActive(false);
    }

    void Update()
    {
        explosingCounter.text = TrapManager.Instance.numberOfReadyExplosingTraps.ToString();
        stunningCounter.text = TrapManager.Instance.numberOfReadyStunningTraps.ToString();
        if(TrapManager.Instance.numberOfReadyExplosingTraps > 0)
        {
            explosingTrapButton.interactable = true;
        }
        else
        {
            explosingTrapButton.interactable = false;
        }
        if(TrapManager.Instance.numberOfReadyStunningTraps > 0)
        {
            stunningTrapButton.interactable = true;
        }
        else
        {
            stunningTrapButton.interactable = false;
        }
    }

    public void TryToBuildExplosingTrap()
    {
        if(TrapManager.Instance.numberOfReadyExplosingTraps > 0)
        {
            BuildingsManager.Instance.Build(explosingTrapPrefab);//TODO: fix this, its not working
        }
        else
        {
            Debug.Log("We have some serious errors with exploding traps counting.");
        }
    }

    public void TryToBuildStunningTrap()
    {
        if(TrapManager.Instance.numberOfReadyStunningTraps > 0)
        {
            BuildingsManager.Instance.Build(stunningTrapPrefab);
        }
        else
        {
            Debug.Log("We have some serious errors with stunning traps counting.");
        }
    }

    public void EnableButtons()
    {
        explosingTrapButton.interactable = true;
        stunningTrapButton.interactable = true;
    }
    
    public void DisableButtons()
    {
        explosingTrapButton.interactable = false;
        stunningTrapButton.interactable = false;
    }
}
