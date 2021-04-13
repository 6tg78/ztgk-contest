using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OutroDescriptionChanger : MonoBehaviour
{
    public GameObject victoryDescription, defeatDescription, victoryImage, defeatImage;

    void Awake()
    {
        victoryDescription.SetActive(false);
        defeatDescription.SetActive(false);
        victoryImage.SetActive(false);
        defeatImage.SetActive(false);
    }

    void Start()
    {
        if(GameManager.Instance.VictoryAchieved)
        {
            victoryDescription.SetActive(true);
            victoryDescription.SetActive(true);
        }
        else
        {
            defeatDescription.SetActive(true);
            defeatImage.SetActive(true);
        }
    }
}
