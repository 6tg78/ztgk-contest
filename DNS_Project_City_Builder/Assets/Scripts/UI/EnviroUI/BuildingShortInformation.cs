using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingShortInformation : MonoBehaviour
{
    #region Variables
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI assignedSpiritsText;

    public TextMeshProUGUI productionText; //for some buildings (storage for example) it can be other thing with resources
    public Image resourceIcon;

    public TextMeshProUGUI additionalInfoText; //additional info (if needed)

    #endregion

    #region Component

    public void SetPanel(Building refBuilding)
    {
        
        // First child - Name
        nameText.text = refBuilding.BuildingName;
        additionalInfoText.text = "";

        if(refBuilding.MaxSpirits > 0)
        {
            assignedSpiritsText.text = "Spirits: " + refBuilding.Spirits.Count + "/" + refBuilding.MaxSpirits;
        }

        if(refBuilding.MaxSpirits == 0 || refBuilding is Shelter)
        {
            additionalInfoText.text += "You cannot assign spirits to this building.";
            assignedSpiritsText.gameObject.SetActive(false);
        }

        if(refBuilding is ResourceBuilding)
        {
            switch( ((ResourceBuilding)refBuilding).TypeOfResource )
            {
                case ResourceBuilding.ResourceCode.LifeEnergy:
                    resourceIcon = UIManager.Instance.resourcesIcons[0];
                    productionText.text = "+" + (BalancePanel.Instance.LifeEnergy.incomePerSpirit / BalancePanel.Instance.LifeEnergy.timerCooldown * refBuilding.CurrentSpirits).ToString("f2") + "/sec";
                    break;
                case ResourceBuilding.ResourceCode.Wood:
                    resourceIcon = UIManager.Instance.resourcesIcons[1];
                    productionText.text = "+" + (BalancePanel.Instance.Wood.incomePerSpirit / BalancePanel.Instance.Wood.timerCooldown * refBuilding.CurrentSpirits).ToString("f2") + "/sec";
                    break;
                case ResourceBuilding.ResourceCode.ThirdResource:
                    resourceIcon = UIManager.Instance.resourcesIcons[2];
                    productionText.text = "+" + (BalancePanel.Instance.ThirdResource.incomePerSpirit / BalancePanel.Instance.ThirdResource.timerCooldown * refBuilding.CurrentSpirits).ToString("f2") + "/sec";
                    break;
                default:
                    break;
            }

            
        }
        
    }

    #endregion
}
