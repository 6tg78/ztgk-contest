using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>k
/// Script for buttons used for placing buildings in game, version 2.0
/// </summary>
public class BuildingButtonNew : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Variables

    public GameObject descriptionPrefab;

    /// <summary>
    /// Building for which the button is created.
    /// </summary>
    [Header("Auto assigned")]
    public Building refBuilding;

    [Header("UI Object children")]
    public TextMeshProUGUI nameText;
    public GameObject woodCost;
    public GameObject energyCost;
    public GameObject trCost;
    public TextMeshProUGUI constructTimeText;
    public GameObject income;
    [SerializeField]
    private Image buildingImage;

    private GameObject descriptionInstance;

    public Transform ParentTransform;

    #endregion

    #region MonoBehaviour
    private void Start()
    {
        //BuildingsManager.Instance.OnBuildingNotBuild += DisplayMessageCantBeBuild;
    }
    private void Update() //temporary solution, i'll find out smth better
    {
        CheckRequirements();
    }
    #endregion

    #region Interface Implementations
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!UIManager.Instance.BuildingsPanelAnimation)
        {
            descriptionInstance = Instantiate(descriptionPrefab, ParentTransform);

            var objectRect = gameObject.GetComponent<RectTransform>().rect;
            var objectPos = transform.position;
            var canvasScale = UIManager.Instance.ScreenSpaceCanvas.transform.localScale;

            descriptionInstance.transform.position = new Vector3(objectRect.width * canvasScale.x * 1.1f, objectPos.y, objectPos.z);

            descriptionInstance.GetComponent<BuildingButtonDescription>().Setup(refBuilding);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(descriptionInstance);
        descriptionInstance = null;
    }
    #endregion

    #region Component

    public void Setup()
    {
        UIManager.Instance.PrepareBuildingForStage3(refBuilding);
        nameText.text = refBuilding.BuildingName;
        constructTimeText.text = (refBuilding.TimeNeededForConstruction).ToString("f0") + "s";

        if (refBuilding.BuildingCost.wood > 0.0001f)
        {
            woodCost.GetComponentInChildren<TextMeshProUGUI>().text = refBuilding.BuildingCost.wood.ToString("f0");
        }
        else
        {
            woodCost.SetActive(false);
        }

        if (refBuilding.BuildingCost.lifeEnergy > 0.0001f)
        {
            energyCost.GetComponentInChildren<TextMeshProUGUI>().text = refBuilding.BuildingCost.lifeEnergy.ToString("f0");
        }
        else
        {
            energyCost.SetActive(false);
        }

        if (refBuilding.BuildingCost.thirdResource > 0.0001f)
        {
            trCost.GetComponentInChildren<TextMeshProUGUI>().text = refBuilding.BuildingCost.thirdResource.ToString("f0");
        }
        else
        {
            trCost.SetActive(false);
        }

        ShiftResources();

        if (refBuilding is ResourceBuilding)
        {
            var tempBuild = (ResourceBuilding)refBuilding;
            string incomeNew = "+ ";
            switch (tempBuild.TypeOfResource)
            {
                case ResourceBuilding.ResourceCode.Wood:
                    income.GetComponentInChildren<Image>().sprite = woodCost.GetComponentInChildren<Image>().sprite;
                    income.GetComponentInChildren<Image>().color = woodCost.GetComponentInChildren<Image>().color;
                    income.GetComponentInChildren<Image>().material = woodCost.GetComponentInChildren<Image>().material;
                    incomeNew += (BalancePanel.Instance.Wood.incomePerSpirit / BalancePanel.Instance.Wood.timerCooldown).ToString("f2");
                    break;
                case ResourceBuilding.ResourceCode.LifeEnergy:
                    income.GetComponentInChildren<Image>().sprite = energyCost.GetComponentInChildren<Image>().sprite;
                    income.GetComponentInChildren<Image>().color = energyCost.GetComponentInChildren<Image>().color;
                    income.GetComponentInChildren<Image>().material = energyCost.GetComponentInChildren<Image>().material;
                    incomeNew += (BalancePanel.Instance.LifeEnergy.incomePerSpirit / BalancePanel.Instance.LifeEnergy.timerCooldown).ToString("f2");
                    break;
                case ResourceBuilding.ResourceCode.ThirdResource:
                    income.GetComponentInChildren<Image>().sprite = trCost.GetComponentInChildren<Image>().sprite;
                    income.GetComponentInChildren<Image>().color = trCost.GetComponentInChildren<Image>().color;
                    income.GetComponentInChildren<Image>().material = trCost.GetComponentInChildren<Image>().material;
                    incomeNew += (BalancePanel.Instance.ThirdResource.incomePerSpirit / BalancePanel.Instance.ThirdResource.timerCooldown).ToString("f2");
                    break;
                default:
                    break;
            }
            incomeNew += "/s per Spirit";
            income.GetComponentInChildren<TextMeshProUGUI>().text = incomeNew;
        }
        else
        {
            income.SetActive(false);
        }

        if (refBuilding.BuildingImage != null)
            buildingImage.sprite = refBuilding.BuildingImage;
    }
    public void BuildOnClick()
    {
        //check if we have enough resources to built - event if not 
        BuildingsManager.Instance.Build(refBuilding.gameObject);

    }
    /*
    public void DisplayMessageCantBeBuild(GameObject building, bool lifeEnergy, bool wood, bool thirdResource)//event testing purpose
    {
        if (building != refBuilding.gameObject)
            return;
        if (lifeEnergy) NotificationManager.Instance.AddNotification("Not enough resources.", "Collect more Life Energy to build " + refBuilding.name + ".");
        if (wood) NotificationManager.Instance.AddNotification("Not enough resources.", "Collect more Wood to build " + refBuilding.name + ".");
        if (thirdResource) NotificationManager.Instance.AddNotification("Not enough resources.", "Collect more Crystals to build " + refBuilding.name + "."); //TODO: check what is third resource name
    }
    */
    private void ShiftResources()
    {
        if (!energyCost.activeSelf)
        {
            if (woodCost.activeSelf)
            {
                woodCost.transform.position = energyCost.transform.position;
            }
            else
            {
                trCost.transform.position = energyCost.transform.position;
            }
        }
        else if (!woodCost.activeSelf)
        {
            if (trCost.activeSelf)
            {
                trCost.transform.position = woodCost.transform.position;
            }
        }
    }

    private void CheckRequirements()
    {
        if (ResourceManagement.Instance.EnoughResource<LifeEnergyResource>(refBuilding.BuildingCost.lifeEnergy))
        {
            energyCost.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            energyCost.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }

        if (ResourceManagement.Instance.EnoughResource<WoodResource>(refBuilding.BuildingCost.wood))
        {
            woodCost.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            woodCost.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }

        if (ResourceManagement.Instance.EnoughResource<ThirdResource>(refBuilding.BuildingCost.thirdResource))
        {
            trCost.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            trCost.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }
    }

    #endregion
}
