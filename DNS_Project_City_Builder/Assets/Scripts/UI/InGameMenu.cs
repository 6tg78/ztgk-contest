using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject timeButtons;

    private static InGameMenu Instance;

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
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        timeButtons.GetComponent<MenuButtons>().SetNormalGameSpeed();
        gameObject.SetActive(false);
    }
}
