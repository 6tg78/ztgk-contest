using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workshop : Building
{
    public float TrapCraftingProgress { get; private set; }
    public bool CraftingStarted { get; private set; }
    public Trap CurrentlyCraftedTrap { get; private set; }


    Workshop()
    {
        BuildingName = "Oasis of Miracles";
    }

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    protected override void Update()
    {
        base.Update();
        if (CraftingStarted)
        {
            var step = WorkersAmount * Time.deltaTime * 100.0f / CurrentlyCraftedTrap.timeToCraft;
                Debug.Log("Crafting trap " + TrapCraftingProgress + " trap: " + CurrentlyCraftedTrap + " step: " + step);
            if (TrapCraftingProgress + step < 100.0f)
            {
                TrapCraftingProgress += step;
            }
            else
            {
                Debug.Log("Finished crafting");
                CraftingStarted = false;
                TrapCraftingProgress = 0.0f;
                TrapManager.Instance.TrapCrafted(CurrentlyCraftedTrap.trapType);
                CurrentlyCraftedTrap = null;
            }
        }
        if (ResearchManager.Instance.ResearchStarted && IsOperating)
        {
            ResearchManager.Instance.AdvanceResearch(WorkersAmount * Time.deltaTime * 100.0f / ResearchManager.Instance.TimeNeededForCurrentResearch);
        }
    }

    protected override void SetWorkType()
    {
        WorkType = WorkTypeEnum.Researching;
    }
    public void BeginCrafting(Trap trap) // For UI.
    {
        var man = ResourceManagement.Instance;
        if (man.EnoughResource<LifeEnergyResource>(trap.cost.lifeEnergy) && man.EnoughResource<WoodResource>(trap.cost.wood) &&
            man.EnoughResource<ThirdResource>(trap.cost.thirdResource) && CraftingStarted == false)
        {
            man.UseResource<LifeEnergyResource>(trap.cost.lifeEnergy);
            man.UseResource<WoodResource>(trap.cost.wood);
            man.UseResource<ThirdResource>(trap.cost.thirdResource);
            CurrentlyCraftedTrap = trap;
            CraftingStarted = true;
        }
        else
        {
            Debug.Log("[sound of not possible action]");
            //event for ui that trap is not crafting
        }
    }

    public void CancelCrafting() // For UI.
    {
        var man = ResourceManagement.Instance;
        var cctc = CurrentlyCraftedTrap.cost;
        man.AddResources<LifeEnergyResource>(cctc.lifeEnergy);
        man.AddResources<WoodResource>(cctc.wood);
        man.AddResources<ThirdResource>(cctc.thirdResource);
        CurrentlyCraftedTrap = null;
        TrapCraftingProgress = 0.0f;
        CraftingStarted = false;
    }


    private void Initialize()
    {
        CurrentlyCraftedTrap = null;
        TrapCraftingProgress = 0.0f;
        CraftingStarted = false;
    }
}
