using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpiritCostDisplayer : MonoBehaviour
{   
    [SerializeField]
    private TextMeshProUGUI textField; 

    void Update()
    {
        textField.text = AIManager.Instance.LifeEnergyNeededToAddSpirit.ToString();
    }
}
