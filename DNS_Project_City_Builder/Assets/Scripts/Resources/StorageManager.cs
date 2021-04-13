using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance {get; private set;}

    public ResourcePack CapacityGainPerStorage {get {return capacityGainPerStorage;} private set {capacityGainPerStorage = value;}}

    public ResourcePack Limits { get; private set; }
    private ResourcePack capacityGainPerStorage;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        Initialize();
    }


    public void StageProgress()
    {
        BalancePanel.Instance.BalanceUpdate(out capacityGainPerStorage);
        int alreadyBuiltStorages = 0;
        foreach(Storage storage in GameManager.Instance.storages)
        {
            if(storage.FinishedConstructing)
            {
                alreadyBuiltStorages++;
            }
        }
        ResourcePack newLimits;
        newLimits.lifeEnergy = BalancePanel.Instance.LifeEnergy.initialMaxValue + capacityGainPerStorage.lifeEnergy * alreadyBuiltStorages;
        newLimits.wood = BalancePanel.Instance.Wood.initialMaxValue + capacityGainPerStorage.wood * alreadyBuiltStorages;
        newLimits.thirdResource = BalancePanel.Instance.ThirdResource.initialMaxValue + capacityGainPerStorage.thirdResource * alreadyBuiltStorages;
    }

    public void IncreaseLimits()
    {
        ResourcePack oldLimits = Limits;
        Limits = new ResourcePack(oldLimits.lifeEnergy + capacityGainPerStorage.lifeEnergy,
            oldLimits.wood + capacityGainPerStorage.wood,
            oldLimits.thirdResource + capacityGainPerStorage.thirdResource);
    }

    private void Initialize()
    {
        BalancePanel.Instance.BalanceUpdate(out capacityGainPerStorage);
        Limits = new ResourcePack(BalancePanel.Instance.LifeEnergy.initialMaxValue,
            BalancePanel.Instance.Wood.initialMaxValue,
            BalancePanel.Instance.ThirdResource.initialMaxValue);
    }
}
