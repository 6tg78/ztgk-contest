using System.Collections;
using UnityEngine;

public class LongRangeTurretPanel : BuildingUIPanel
{
    public Counter _counter;
    public NameLabel _nameLabel;
    public BuildingDescription _description;
    public RepairButton _repair;
    public SetAttackPlaceButton _setAttackPlace;
    public ProgressBar _progress;

    void OnEnable()
    {
        // Subscribe to events
        _nameLabel.OnCloseCallback += Close;
        //_counter.OnAddCallback += AssignUnit;
        //_counter.OnSubtractCallback += RemoveUnit;

        if (_repair != null)
            _repair.OnRepair += Repair;
        if (_setAttackPlace != null)
            _setAttackPlace.OnSetAttackPlace += SetAttackPlace;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        _nameLabel.OnCloseCallback -= Close;
        _setAttackPlace.OnSetAttackPlace -= SetAttackPlace;
        //_counter.OnAddCallback -= AssignUnit;
        //_counter.OnSubtractCallback -= RemoveUnit;

        if (_repair != null)
            _repair.OnRepair -= Repair;
    }

    public override void Bind(Building building)
    {
        BoundBuilding = building;
        _nameLabel.LabelString = BoundBuilding.BuildingName;
        var buildingUI = building.gameObject.GetComponent<BuildingUI>();
        _description.DescriptionString = buildingUI.Description;
        Refresh();
        if (_progress != null)
            StartCoroutine(ProgressBarDisplay());
    }

    public void AssignUnit()
    {
        AIManager.Instance.AddSpiritToWork(BoundBuilding);
        //Refresh();
    }

    public void RemoveUnit()
    {
        AIManager.Instance.RemoveSpiritFromWork(BoundBuilding);
        //Refresh();
    }

    public void Close()
    {
        if (_progress != null)
            StopCoroutine(ProgressBarDisplay());

        UIManager.Instance.CloseBuildingUI();
    }

    public void Refresh()
    {
        //_counter.LabelString = "Spirits: " + BoundBuilding.Spirits.Count;
    }

    public void Repair()
    {
        BoundBuilding.Repair();
    }

    public void SetAttackPlace()
    {
        LongRangeTurret turret = BoundBuilding as LongRangeTurret;
        turret.SetAttackPlace();
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
