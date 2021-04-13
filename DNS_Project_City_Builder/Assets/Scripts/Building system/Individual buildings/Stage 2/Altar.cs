using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : ResourceBuilding
{
    Altar()
    {
        BuildingName = "Altar";
    }

    protected override void Awake()
    {
        base.Awake();
        BuildingName = "Altar";
    }


    protected override void SetWorkType()
    {
        WorkType = WorkTypeEnum.Praying;
    }
}
