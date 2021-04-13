using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagement : MonoBehaviour
{
    public static ResourceManagement Instance { get; private set; }
    public event Action OnResounceAmmountChanged;
    public event Action OnIncomeChanged;


    private LifeEnergyResource lifeEnergy;
    private WoodResource wood;
    private ThirdResource thirdResource;
    private float lifeEnergyIncomePerSpirit;
    private float woodIncomePerSpirit;
    private float thirdResourceIncomePerSpirit;

    public float WoodIncomePerSecond { get; private set; }
    public float LifeEnergyIncomePerSecond { get; private set; }
    public float ThirdResourceIncomePerSecond { get; private set; }
    /*
    private int spiritsCollectingResources;
    public int SpiritsCollectingResources 
    {
        get { return spiritsCollectingResources; }
        private set { spiritsCollectingResources = value; OnSpiritsCollectingChange?.Invoke(); }
    }
    */
    public event Action OnSpiritsCollectingChange;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        lifeEnergy = new LifeEnergyResource(BalancePanel.Instance.LifeEnergy.timerCooldown);
        wood = new WoodResource(BalancePanel.Instance.Wood.timerCooldown);
        thirdResource = new ThirdResource(BalancePanel.Instance.ThirdResource.timerCooldown);
        OnSpiritsCollectingChange += UpdateIncomePerSpirit;
    }

    private void Start()
    {
        StageProgress();
        //StartCoroutine(Timer(0.5f, delegate { UpdateIncomePerSpirit(); }, true)); //true to keep coroutine alive all the time
        StartCoroutine(Timer(1f, delegate { AddAllResources(); }, true));
    }

    private void OnDestroy()
    {
        OnSpiritsCollectingChange -= UpdateIncomePerSpirit;
    }

    private void UpdateIncomePerSpirit()
    {
        CountIncomePerSecond<LifeEnergyResource>();
        CountIncomePerSecond<WoodResource>();
        CountIncomePerSecond<ThirdResource>();

        OnIncomeChanged?.Invoke();
    }

    private void AddAllResources()
    {
        AddResources<LifeEnergyResource>(LifeEnergyIncomePerSecond);
        AddResources<WoodResource>(WoodIncomePerSecond);
        AddResources<ThirdResource>(ThirdResourceIncomePerSecond);
    }
    private void CountIncomePerSecond<Type>() where Type : Resource
    {
        if (typeof(Type) == lifeEnergy.GetType())
        {
            LifeEnergyIncomePerSecond = 0;
            
            List<ResourceBuilding> buildings = new List<ResourceBuilding>(FindObjectsOfType<Altar>());
            foreach (ResourceBuilding building in buildings)
            {
                if (building.IsOperating)
                {
                    LifeEnergyIncomePerSecond += BalancePanel.Instance.LifeEnergy.incomePerSpirit / (lifeEnergy.GetTimerCooldown() / building.WorkersAmount);
                }
            }

        }
        if (typeof(Type) == wood.GetType())
        {
            WoodIncomePerSecond = 0;

            List<ResourceBuilding> buildings = new List<ResourceBuilding>(FindObjectsOfType<WeaversHut>());
            foreach (ResourceBuilding building in buildings)
            {
                if (building.IsOperating) WoodIncomePerSecond += BalancePanel.Instance.Wood.incomePerSpirit / (wood.GetTimerCooldown() / building.WorkersAmount);
            }
        }
        if (typeof(Type) == thirdResource.GetType())
        {
            ThirdResourceIncomePerSecond = 0;

            List<ResourceBuilding> buildings = new List<ResourceBuilding>(FindObjectsOfType<Generator>());
            foreach (ResourceBuilding building in buildings)
            {
                if (building.IsOperating) ThirdResourceIncomePerSecond += BalancePanel.Instance.ThirdResource.incomePerSpirit / (thirdResource.GetTimerCooldown() / building.WorkersAmount);
            }

        }
    }
    /// <summary>
    /// Add amount of defined resource
    /// </summary>
    /// <typeparam name="Type">Resource that is added</typeparam>
    /// <param name="amount">Amount that is added (when not specified add value from balance panel)</param>
    /// <returns></returns>
    public bool AddResources<Type>(float amount = -1) where Type : Resource
    {
        bool result = false;
        if (typeof(Type) == lifeEnergy.GetType())
        {
            result = lifeEnergy.Add(amount == -1 ? lifeEnergyIncomePerSpirit : amount);
        }
        else if (typeof(Type) == wood.GetType())
        {
            result = wood.Add(amount == -1 ? woodIncomePerSpirit : amount);
        }
        else if (typeof(Type) == thirdResource.GetType())
        {
            result = thirdResource.Add(amount == -1 ? thirdResourceIncomePerSpirit : amount);
        }
        if(result) OnResounceAmmountChanged?.Invoke();
        return result;
    }

    public void UseResource<Type>(float amount = 1) where Type : Resource
    {
        if (typeof(Type) == lifeEnergy.GetType())
        {
            lifeEnergy.Use(amount);
        }
        else if (typeof(Type) == wood.GetType())
        {
            wood.Use(amount);
        }
        else if (typeof(Type) == thirdResource.GetType())
        {
            thirdResource.Use(amount);
        }

        OnResounceAmmountChanged?.Invoke();
    }

    public float GetResourceAmount<Type>() where Type : Resource
    {
        if (typeof(Type) == lifeEnergy.GetType())
        {
            return Mathf.Round(lifeEnergy.GetAmount());
        }
        else if (typeof(Type) == wood.GetType())
        {
            return Mathf.Round(wood.GetAmount());
        }
        else if (typeof(Type) == thirdResource.GetType())
        {
            return Mathf.Round(thirdResource.GetAmount());
        }
        else return 0;
    }

    public float GetResourceTimerCooldown<Type>() where Type : Resource
    {
        if (typeof(Type) == lifeEnergy.GetType())
        {
            return lifeEnergy.GetTimerCooldown();
        }
        else if (typeof(Type) == wood.GetType())
        {
            return wood.GetTimerCooldown();
        }
        else if (typeof(Type) == thirdResource.GetType())
        {
            return thirdResource.GetTimerCooldown();
        }
        else return 0;
    }

    public bool EnoughResource<Type>(float neededAmount) where Type : Resource
    {
        if (typeof(Type) == lifeEnergy.GetType())
        {
            return lifeEnergy.GetIntAmount() - Mathf.RoundToInt(neededAmount) >= 0;
        }
        if (typeof(Type) == wood.GetType())
        {
            return wood.GetIntAmount() - Mathf.RoundToInt(neededAmount) >= 0;
        }
        if (typeof(Type) == thirdResource.GetType())
        {
            return thirdResource.GetIntAmount() - Mathf.RoundToInt(neededAmount) >= 0;
        }
        else return false;
    }

    public void StageProgress()
    {
        lifeEnergyIncomePerSpirit = BalancePanel.Instance.LifeEnergy.incomePerSpirit;
        woodIncomePerSpirit = BalancePanel.Instance.Wood.incomePerSpirit;
        thirdResourceIncomePerSpirit = BalancePanel.Instance.ThirdResource.incomePerSpirit;
    }

    public void ChangedSpiritsCollectingResources(bool started)
    {
        //SpiritsCollectingResources += started ? 1 : -1;
        OnSpiritsCollectingChange?.Invoke();
    }

    /// <summary>
    /// Universal timer for void delegates
    /// </summary>
    /// <param name="time">Time needed to perform method in seconds</param>
    /// <param name="method">Delegate that is performed after time seconds</param>
    /// <param name="stopClause">Condition that defines when/if stop the timer (false to stop timer)</param>
    /// <returns></returns>
    private IEnumerator Timer(float time, Action method, bool stopClause)
    {
        while (stopClause)
        {
            yield return new WaitForSeconds(time);
            method();
        }
    }
}
