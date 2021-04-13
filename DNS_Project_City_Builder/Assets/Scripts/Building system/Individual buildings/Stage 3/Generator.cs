using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : ResourceBuilding
{
    Generator()
    {
        BuildingName = "Mine";
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void SetWorkType()
    {
        WorkType = WorkTypeEnum.Collecting;
    }
}
