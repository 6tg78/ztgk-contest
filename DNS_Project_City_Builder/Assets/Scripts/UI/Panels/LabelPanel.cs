using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelPanel : BuildingUIPanel
{
    public override void Bind(Building building)
    {
        BoundBuilding = building;
    }

    public void OnClose()
    {
        Debug.Log("On Close!");
        UIManager.Instance.CloseBuildingUI();
    }
}
