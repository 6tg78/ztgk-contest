using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Description of building
/// </summary>
public class BuildingButtonDescription : MonoBehaviour
{
    #region Variables
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    #endregion

    #region Mono Behaviour

    #endregion

    #region Component
    public void Setup(Building refBuilding)
    {
        nameText.text = refBuilding.BuildingName;
        //TODO change this to buildings description field if it will exist
        descriptionText.text = refBuilding.BuildingDescription;
            //"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
    }
    #endregion
}
