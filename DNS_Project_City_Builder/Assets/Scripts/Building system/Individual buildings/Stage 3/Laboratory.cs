using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : Building
{
    Laboratory()
    {
        BuildingName = "Using wrong script - laboratory doesn't exist anymore";
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        /*
        LEGACY CODE - laboratory doesn't exist anymore (fused with workshop)

        if(ResearchManager.Instance.ResearchStarted && IsOperating)
        {
            ResearchManager.Instance.AdvanceResearch(CurrentSpirits * Time.deltaTime * 100.0f / ResearchManager.Instance.TimeNeededForCurrentResearch);
        }
        */
    }
}
