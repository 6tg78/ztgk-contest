using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceBuilding : Building
{
    public float TimeToGetResource { get; protected set; }
    public float TimerCooldown {get; protected set; }

    [SerializeField]
    protected ResourceCode resourceType;

    public ResourceCode TypeOfResource { get => resourceType; }

    protected override void Start()
    {
        base.Start();
        SetTimerCooldown();
        TimeToGetResource = TimerCooldown;
    }


    protected override void UpdateActions()
    {
        base.UpdateActions();
        if(IsOperating)
        {
            UseTimer();
        }
    }


    private void Timer(int spiritsAmount, Func<float, bool> addResource) // HACK: Test in game
    {
        if (spiritsAmount > 0)
        {
            TimeToGetResource -= Time.deltaTime * spiritsAmount;
            if (TimeToGetResource <= 0)
            {
                if(addResource(0))
                    TimeToGetResource = TimerCooldown;
            }
        }
    }

    private void SetTimerCooldown()
    {
        switch(resourceType)
        {
            case ResourceCode.LifeEnergy:
            {
                TimerCooldown = ResourceManagement.Instance.GetResourceTimerCooldown<LifeEnergyResource>();
                break;
            }
            case ResourceCode.Wood:
            {
                TimerCooldown = ResourceManagement.Instance.GetResourceTimerCooldown<WoodResource>();
                break;
            }
            case ResourceCode.ThirdResource:
            {
                TimerCooldown = ResourceManagement.Instance.GetResourceTimerCooldown<ThirdResource>();
                break;
            }
            default: 
            {
                Debug.Log("Error in ResourceBuilding.SetTimerCooldown()");
                break;
            }
        }
    }

    //TODO Fix this, for now it's making one resource for i dunno what time, it must depend on buildings statistics, goddamnit
    private void UseTimer()
    {
        switch(resourceType)
        {
            case ResourceCode.LifeEnergy:
            {
                  //  Timer(CurrentSpirits, ResourceManagement.Instance.AddResources<LifeEnergyResource>);
                    break;
            }
            case ResourceCode.Wood:
            {
               // Timer(CurrentSpirits, ResourceManagement.Instance.AddResources<WoodResource>);
                break;
            }
            case ResourceCode.ThirdResource:
            {
              //  Timer(CurrentSpirits, ResourceManagement.Instance.AddResources<ThirdResource>);
                break;
            }
            default: 
            {
                Debug.Log("Error in ResourceBuilding.UseTimer()");
                break;
            }
        }
    }



    public enum ResourceCode
    {
        LifeEnergy = 1,
        Wood = 2,
        ThirdResource = 3
    }
}
