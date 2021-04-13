using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopPanel : BuildingUIPanel
{
    [SerializeField] private NameLabel _nameLabel;
    [SerializeField]private Counter _counter;
    [SerializeField] private BuildingDescription _description;
    [SerializeField] private RepairButton _repair;
    [SerializeField] private ProgressBar _repairProgress;
    [SerializeField] private ProgressBar _trapProgress;
    [SerializeField] private ProgressBar _researchProgress;
    [SerializeField] private WorkshopMenuButton workshopMenu;
    [SerializeField] private WorkshopMenuPanel workshopMenuPanel;

    private Workshop _workshop;

    void Awake()
    {
        // Subscribe to events
        _nameLabel.OnCloseCallback += Close;
        _counter.OnAddCallback += AssignUnit;
        _counter.OnSubtractCallback += RemoveUnit;
        workshopMenu.OnWorshopMenuOpen += OpenMenu;
        if (_repair != null)
            _repair.OnRepair += Repair;

        _workshop = (Workshop)BoundBuilding;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        _nameLabel.OnCloseCallback -= Close;
        _counter.OnAddCallback -= AssignUnit;
        _counter.OnSubtractCallback -= RemoveUnit;
        workshopMenu.OnWorshopMenuOpen -= OpenMenu;
        Refresh();
        if (_repair != null)
            _repair.OnRepair -= Repair;
    }

    public override void Bind(Building building)
    {
        BoundBuilding = building;
        _workshop = (Workshop)BoundBuilding;
        _nameLabel.LabelString = BoundBuilding.BuildingName;
        var buildingUI = building.gameObject.GetComponent<BuildingUI>();
        _description.DescriptionString = buildingUI.Description;
        //if (_repairProgress != null)
        StartCoroutine(ProgressBarDisplay());
    }
    public void AssignUnit()
    {
        AIManager.Instance.AddSpiritToWork(BoundBuilding);
        Refresh();
    }

    public void RemoveUnit()
    {
        AIManager.Instance.RemoveSpiritFromWork(BoundBuilding);
        Refresh();
    }
    public void Refresh()
    {
        _counter.LabelString = "Spirits: " + BoundBuilding.Spirits.Count;
    }
    public void Close()
    {
        StopCoroutine(ProgressBarDisplay());
        UIManager.Instance.CloseBuildingUI();
    }

    public void Repair()
    {
        BoundBuilding.Repair();
    }

    public void OpenMenu()
    {
        var menu = Instantiate(workshopMenuPanel, UIManager.Instance.ScreenSpaceCanvas.transform);
        menu.Bind(BoundBuilding);
        Close();
    }
    private IEnumerator ProgressBarDisplay()
    {
        while (true)
        {
            if (_repairProgress != null)
            {
                float percentOfRepair = BoundBuilding.CurrHitPoints / BoundBuilding.MaxHitPoints;
                _repairProgress.Progress = percentOfRepair;
                percentOfRepair *= 100f;
                _repairProgress.Label = "Progress: " + percentOfRepair.ToString() + "%";
            }

            float progressPercent = _workshop.TrapCraftingProgress / 100f;
            _trapProgress.Progress = progressPercent;
            progressPercent *= 100f;
            _trapProgress.Label = "Trap Progress: " + progressPercent.ToString() + "%";

            progressPercent = ResearchManager.Instance.ResearchProgress / 100f;
            _researchProgress.Progress = progressPercent;
            progressPercent *= 100f;
            _researchProgress.Label = "Research Progress: " + progressPercent.ToString() + "%";

            yield return new WaitForEndOfFrame();
        }
    }
}
