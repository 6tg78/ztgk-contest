
using System.Collections;
using UnityEngine;

public class BuildingInfoPanel : BuildingUIPanel
{
    public NameLabel _nameLabel;
    public BuildingDescription _description;
    public RepairButton _repair;
    public ProgressBar _progress;

    void Awake()
    {
        // Subscribe to events
        _nameLabel.OnCloseCallback += Close;
        if (_repair != null)
            _repair.OnRepair += Repair;
        
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        _nameLabel.OnCloseCallback -= Close;
        if (_repair != null && BoundBuilding != null)
            _repair.OnRepair -= Repair;
    }

    public override void Bind(Building building)
    {
        BoundBuilding = building;
        _nameLabel.LabelString = BoundBuilding.BuildingName;
        var buildingUI = building.gameObject.GetComponent<BuildingUI>();
        _description.DescriptionString = buildingUI.Description;
        if (_progress != null)
            StartCoroutine(ProgressBarDisplay());

    }

    public void Close()
    {
        if (_progress != null)
            StopCoroutine(ProgressBarDisplay());
        UIManager.Instance.CloseBuildingUI();
    }

    private void Spawn()
    {
        AIManager.Instance.AddSpirit();
    }

    public void Repair()
    {
        BoundBuilding.Repair();
    }

    private IEnumerator ProgressBarDisplay()
    {
        while (true)
        {
            float percentOfRepair = BoundBuilding.CurrHitPoints / BoundBuilding.MaxHitPoints;
            _progress.Progress = percentOfRepair;
            percentOfRepair *= 100f;
            _progress.Label = "Progress: " + percentOfRepair.ToString() + "%";

            yield return new WaitForEndOfFrame();
        }
    }
}
