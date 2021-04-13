using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource
{
    protected float amount;
    [SerializeField]
    private float timerCooldown;

    protected Resource(float timerCooldown)
    {
        this.timerCooldown = timerCooldown;
        amount = GetInitialAmount();
    }
    /// <summary>
    /// Returns true if any value is added
    /// </summary>
    /// <param name="amount">Amount of resource to add</param>
    /// <returns></returns>
    public bool Add(float amount)
    {
        float capacity = GetMaxStorageCapacity();
        float oldAmount = this.amount;
        this.amount = Mathf.Min(this.amount + amount, capacity);
        return this.amount - oldAmount > 0;
    }

    public void Use(float amount)
    {
        this.amount -= amount;
    }

    public float GetAmount()
    {
        return amount;
    }

    public int GetIntAmount()
    {
        return Mathf.RoundToInt(amount);
    }

    public float GetTimerCooldown()
    {
        return timerCooldown;
    }

    protected float GetInitialAmount()
    {
        if(this is LifeEnergyResource)
        {
            return BalancePanel.Instance.LifeEnergy.initialValue;
        }
        else if(this is WoodResource)
        {
            return BalancePanel.Instance.Wood.initialValue;
        }
        else if(this is ThirdResource)
        {
            return BalancePanel.Instance.ThirdResource.initialValue;
        }

        return 0.0f;
    }

    protected float GetMaxStorageCapacity()
    {
        if (this is LifeEnergyResource)
        {
            return StorageManager.Instance.Limits.lifeEnergy;
        }
        else if (this is WoodResource)
        {
            return StorageManager.Instance.Limits.wood;
        }
        else if (this is ThirdResource)
        {
            return StorageManager.Instance.Limits.thirdResource;
        }

        return 0.0f;
    }
}
