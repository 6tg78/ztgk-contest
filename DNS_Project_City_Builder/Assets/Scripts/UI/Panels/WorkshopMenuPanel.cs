using TMPro;
using UnityEngine;

public class WorkshopMenuPanel : BuildingUIPanel
{
    public NameLabel _nameLabel;
    public TrapsLabel trapsLabel;
    public ResearchLabel researchLabel;

    void Awake()
    {
        // Subscribe to events
        _nameLabel.OnCloseCallback += Close;
        trapsLabel.OnTrapSelected += StartTrapConstructing;
        researchLabel.OnResearchSeleccted += StartResearch;
        SelectionController.OnSelectedBuildingChanged += Close;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        _nameLabel.OnCloseCallback -= Close;
        trapsLabel.OnTrapSelected -= StartTrapConstructing;
        researchLabel.OnResearchSeleccted -= StartResearch;
        SelectionController.OnSelectedBuildingChanged -= Close;
    }

    public override void Bind(Building building)
    {
        BoundBuilding = building;
        _nameLabel.LabelString = BoundBuilding.BuildingName;

        researchLabel.researchs[0].Name.text = BalancePanel.Instance.ResearchStats.research1.name;
        researchLabel.researchs[0].Description.text = BalancePanel.Instance.ResearchStats.research1.description;
        researchLabel.researchs[0].TimeNeeded.text = BalancePanel.Instance.ResearchStats.research1.timeNeededForCompletion.ToString("f0");
        researchLabel.researchs[1].Name.text = BalancePanel.Instance.ResearchStats.research2.name;
        researchLabel.researchs[1].Description.text = BalancePanel.Instance.ResearchStats.research2.description;
        researchLabel.researchs[1].TimeNeeded.text = BalancePanel.Instance.ResearchStats.research2.timeNeededForCompletion.ToString("f0");
        researchLabel.researchs[2].Name.text = BalancePanel.Instance.ResearchStats.research3.name;
        researchLabel.researchs[2].Description.text = BalancePanel.Instance.ResearchStats.research3.description;
        researchLabel.researchs[2].TimeNeeded.text = BalancePanel.Instance.ResearchStats.research3.timeNeededForCompletion.ToString("f0");
        researchLabel.researchs[3].Name.text = BalancePanel.Instance.ResearchStats.research4.name;
        researchLabel.researchs[3].Description.text = BalancePanel.Instance.ResearchStats.research4.description;
        researchLabel.researchs[3].TimeNeeded.text = BalancePanel.Instance.ResearchStats.research4.timeNeededForCompletion.ToString("f0");
        researchLabel.researchs[4].Name.text = BalancePanel.Instance.ResearchStats.research5.name;
        researchLabel.researchs[4].Description.text = BalancePanel.Instance.ResearchStats.research5.description;
        researchLabel.researchs[4].TimeNeeded.text = BalancePanel.Instance.ResearchStats.research5.timeNeededForCompletion.ToString("f0");
        researchLabel.researchs[5].Name.text = BalancePanel.Instance.ResearchStats.research6.name;
        researchLabel.researchs[5].Description.text = BalancePanel.Instance.ResearchStats.research6.description;
        researchLabel.researchs[5].TimeNeeded.text = BalancePanel.Instance.ResearchStats.research6.timeNeededForCompletion.ToString("f0");

        //TODO add image if exists
        trapsLabel.traps[0].Name.text = BalancePanel.Instance.ExplosingTrap.name;
        trapsLabel.traps[0].Description.text = BalancePanel.Instance.ExplosingTrap.description;
        trapsLabel.traps[0].WoodCost.text = BalancePanel.Instance.ExplosingTrap.cost.wood.ToString();
        trapsLabel.traps[0].LifeEnergyCost.text = BalancePanel.Instance.ExplosingTrap.cost.lifeEnergy.ToString();
        trapsLabel.traps[0].ThirdResourceCost.text = BalancePanel.Instance.ExplosingTrap.cost.thirdResource.ToString();

        trapsLabel.traps[1].Name.text = BalancePanel.Instance.StunningTrap.name;
        trapsLabel.traps[1].Description.text = BalancePanel.Instance.StunningTrap.description;
        trapsLabel.traps[1].WoodCost.text = BalancePanel.Instance.StunningTrap.cost.wood.ToString();
        trapsLabel.traps[1].LifeEnergyCost.text = BalancePanel.Instance.StunningTrap.cost.lifeEnergy.ToString();
        trapsLabel.traps[1].ThirdResourceCost.text = BalancePanel.Instance.StunningTrap.cost.thirdResource.ToString();

    }
    public void StartTrapConstructing(Trap trap)
    {
        Workshop workshop = BoundBuilding as Workshop;
        BalancePanel.Instance.BalanceUpdate(trap);
        workshop.BeginCrafting(trap);
    }

    public void StartResearch(int research)
    {
        ResearchManager.Instance.StartNewResearch(research);
    }

    //TODO: maybe some research and trap cancelation
    public void Close()
    {
        //UIManager.Instance.SetGameSpeed(1f);
        // UIManager.Instance.CloseBuildingUI();
        UIManager.Instance.HandleSelection(BoundBuilding);
        Destroy(gameObject);
    }
    public void Close(Building building)
    {
        //UIManager.Instance.SetGameSpeed(1f);
        // UIManager.Instance.CloseBuildingUI();
        UIManager.Instance.HandleSelection(BoundBuilding);
        Destroy(gameObject);
    }
}
