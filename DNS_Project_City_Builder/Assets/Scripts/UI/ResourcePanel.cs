using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Script for resource panel
/// </summary>
public class ResourcePanel : MonoBehaviour
{
    #region Variables

    public TextMeshProUGUI LifeEnergyAmountText;
    public TextMeshProUGUI LifeEnergyIncomeText;

    public TextMeshProUGUI WoodAmountText;
    public TextMeshProUGUI WoodIncomeText;

    public TextMeshProUGUI ThirdAmountText;
    public TextMeshProUGUI ThirdIncomeText;

    public TextMeshProUGUI SpiritsAmountText;
    public TextMeshProUGUI SpiritsFreeText;

    public Color plusIncomeColor;
    public Color minusIncomeColor;
    public Color zeroIncomeColor;


    private float lifeEnergyLastAmount;
    private float lifeEnergyLastIncome;
    
    private float woodLastAmount;
    private float woodLastIncome;
    private float woodLastCapacity;

    private float thirdLastAmount;
    private float thirdLastIncome;
    private float thirdLastCapacity;

    private int spiritsLastAmount;
    private int spiritsLastCapacity;
    private int spiritsLastFree;

    private ResourceManagement rmRef;
    private StorageManager smRef;
    private AIManager aiRef;


    /* Amount panels */
    [SerializeField] private AmountPanel _spiritsAmount;

    #endregion

    #region MonoBehaviour

    void OnDestory()
    {
        AIManager.Instance.OnMaxSpiritsCountChanged -= UpdateSpiritsAmount;
        AIManager.Instance.OnFreeSpiritsCountChanged -= UpdateSpiritsAmount;
        AIManager.Instance.OnSpiritsCountChanged -= UpdateSpiritsAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        rmRef = ResourceManagement.Instance;
        smRef = StorageManager.Instance;
        aiRef = AIManager.Instance;

        AIManager.Instance.OnMaxSpiritsCountChanged += UpdateSpiritsAmount;
        AIManager.Instance.OnFreeSpiritsCountChanged += UpdateSpiritsAmount;
        AIManager.Instance.OnSpiritsCountChanged += UpdateSpiritsAmount;

        updateLifeEnergyAmount();
        updateLifeEnergyIncome();

        updateWoodAmount();
        updateWoodIncome();

        updateThirdAmount();
        updateThirdIncome();

        UpdateSpiritsAmount();

        // updateSpiritsAmount();
        // updateSpiritsFree();
    }

    // Update is called once per frame
    void Update()
    {
        //Checking for changes ( to prevent from changing text every frame - very ineffiecient )
        //Life Energy
        if ( !Mathf.Approximately(lifeEnergyLastAmount, rmRef.GetResourceAmount<LifeEnergyResource>()) )
        {
            updateLifeEnergyAmount();
        }

        if( !Mathf.Approximately(lifeEnergyLastIncome, rmRef.LifeEnergyIncomePerSecond) )
        {
            updateLifeEnergyIncome();
        }


        //Wood
        if ( !Mathf.Approximately(woodLastAmount, rmRef.GetResourceAmount<WoodResource>()) || !Mathf.Approximately(woodLastCapacity, smRef.Limits.wood) )
        {
            updateWoodAmount();
        }

        if (!Mathf.Approximately(woodLastIncome, rmRef.WoodIncomePerSecond))
        {
            updateWoodIncome();
        }


        //Third Resource
        if (!Mathf.Approximately(thirdLastAmount, rmRef.GetResourceAmount<ThirdResource>()) || !Mathf.Approximately(thirdLastCapacity, smRef.Limits.thirdResource))
        {
            updateThirdAmount();
        }

        if (!Mathf.Approximately(thirdLastIncome, rmRef.ThirdResourceIncomePerSecond))
        {
            updateThirdIncome();
        }
    }

    #endregion

    #region Component

    #region Life Energy Update
    private void updateLifeEnergyAmount()
    {
        lifeEnergyLastAmount = rmRef.GetResourceAmount<LifeEnergyResource>();
        LifeEnergyAmountText.text = lifeEnergyLastAmount.ToString("f0"); //float without digits after comma

        var newSizeX = LifeEnergyAmountText.preferredWidth; // width of string in unity units
        var sizeY = LifeEnergyAmountText.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        //changing width of text gameobject to be more optimal
        LifeEnergyAmountText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newSizeX, sizeY);

        var nextTextPositionX = LifeEnergyAmountText.gameObject.GetComponent<RectTransform>().anchoredPosition.x + newSizeX + 10.0f;
        var positionY = LifeEnergyIncomeText.gameObject.GetComponent<RectTransform>().anchoredPosition.y;

        //shifting income text to match size of amount text
        LifeEnergyIncomeText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(nextTextPositionX, positionY);
    }
    private void updateLifeEnergyIncome()
    {
        lifeEnergyLastIncome = rmRef.LifeEnergyIncomePerSecond;
        if (lifeEnergyLastIncome > 0.0001f)
        {
            LifeEnergyIncomeText.text = "+" + lifeEnergyLastIncome.ToString("f2"); //float with 2 digits after comma
            LifeEnergyIncomeText.color = plusIncomeColor;
        }
        else if (lifeEnergyLastIncome < -0.0001f)
        {
            LifeEnergyIncomeText.text = "-" + lifeEnergyLastIncome.ToString("f2");
            LifeEnergyIncomeText.color = minusIncomeColor;
        }
        else
        {
            LifeEnergyIncomeText.text = lifeEnergyLastIncome.ToString("f0");
            LifeEnergyIncomeText.color = zeroIncomeColor;
        }
        LifeEnergyIncomeText.text += "/sec";

        var newSizeX = LifeEnergyIncomeText.preferredWidth;
        var sizeY = LifeEnergyIncomeText.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        //changing width of text gameobject to be more optimal
        LifeEnergyIncomeText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newSizeX, sizeY);
    }

    #endregion

    #region Wood Update
    private void updateWoodAmount()
    {
        woodLastAmount = rmRef.GetResourceAmount<WoodResource>();
        woodLastCapacity = smRef.Limits.wood;

        WoodAmountText.text = woodLastAmount.ToString("f0") + "/" + woodLastCapacity.ToString("f0");

        var newSizeX = WoodAmountText.preferredWidth; // width of string in unity units
        var sizeY = WoodAmountText.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        //changing width of text gameobject to be more optimal
        WoodAmountText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newSizeX, sizeY);

        var nextTextPositionX = WoodAmountText.gameObject.GetComponent<RectTransform>().anchoredPosition.x + newSizeX + 10.0f;
        var positionY = WoodAmountText.gameObject.GetComponent<RectTransform>().anchoredPosition.y;

        //shifting income text to match size of amount text
        WoodIncomeText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(nextTextPositionX, positionY);
    }
    private void updateWoodIncome()
    {
        woodLastIncome = rmRef.WoodIncomePerSecond;
        if (woodLastIncome > 0.0001f)
        {
            WoodIncomeText.text = "+" + woodLastIncome.ToString("f2"); //float with 2 digits after comma
            WoodIncomeText.color = plusIncomeColor;
        }
        else if (woodLastIncome < -0.0001f)
        {
            WoodIncomeText.text = "-" + woodLastIncome.ToString("f2");
            WoodIncomeText.color = minusIncomeColor;
        }
        else
        {
            WoodIncomeText.text = woodLastIncome.ToString("f0");
            WoodIncomeText.color = zeroIncomeColor;
        }
        WoodIncomeText.text += "/sec";

        var newSizeX = WoodIncomeText.preferredWidth;
        var sizeY = WoodIncomeText.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        //changing width of text gameobject to be more optimal
        WoodIncomeText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newSizeX, sizeY);
    }

    #endregion s

    #region Third Resource Update
    private void updateThirdAmount()
    {
        thirdLastAmount = rmRef.GetResourceAmount<ThirdResource>();
        thirdLastCapacity = smRef.Limits.thirdResource;

        ThirdAmountText.text = thirdLastAmount.ToString("f0") + "/" + thirdLastCapacity.ToString("f0");

        var newSizeX = ThirdAmountText.preferredWidth; // width of string in unity units
        var sizeY = ThirdAmountText.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        //changing width of text gameobject to be more optimal
        ThirdAmountText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newSizeX, sizeY);

        var nextTextPositionX = ThirdAmountText.gameObject.GetComponent<RectTransform>().anchoredPosition.x + newSizeX + 10.0f;
        var positionY = ThirdAmountText.gameObject.GetComponent<RectTransform>().anchoredPosition.y;

        //shifting income text to match size of amount text
        ThirdIncomeText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(nextTextPositionX, positionY);
    }
    private void updateThirdIncome()
    {
        thirdLastIncome = rmRef.ThirdResourceIncomePerSecond;
        if(thirdLastIncome > 0.0001f)
        {
            ThirdIncomeText.text = "+" + thirdLastIncome.ToString("f2"); //float with 2 digits after comma
            ThirdIncomeText.color = plusIncomeColor;
        }
        else if(thirdLastIncome < -0.0001f)
        {
            ThirdIncomeText.text = "-" + thirdLastIncome.ToString("f2");
            ThirdIncomeText.color = minusIncomeColor;
        }
        else
        {
            ThirdIncomeText.text = thirdLastIncome.ToString("f0");
            ThirdIncomeText.color = zeroIncomeColor;
        }
        ThirdIncomeText.text += "/sec";

        var newSizeX = ThirdIncomeText.preferredWidth;
        var sizeY = ThirdIncomeText.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        //changing width of text gameobject to be more optimal
        ThirdIncomeText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newSizeX, sizeY);
    }

    #endregion

    #region Spirits Update
    
    public void UpdateSpiritsAmount()
    {
        string amountString = string.Format("{1} / {2} ({0}) ", aiRef.FreeSpiritsCount, aiRef.SpiritsCount, aiRef.MaxPossbileSpirits);
        _spiritsAmount.AmountString = amountString;
        _spiritsAmount.IncomeString = "";
    }
    #endregion

    #endregion
}
