using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaversHut : ResourceBuilding
{
    [HideInInspector]
    public List<ResourceField> resourceFields = new List<ResourceField>();
    
    [SerializeField] 
    private float distanceToResources = 50f;


    WeaversHut()
    {
        BuildingName = "Weaver's Hut";
    }

    protected override void Awake()
    {
        base.Awake();
    }


    protected override void SetWorkType()
    {
        WorkType = WorkTypeEnum.Collecting;
    }

    protected override void OnFinishedConstruction()
    {
        base.OnFinishedConstruction();
        FindResourceFields();
    }

    private void FindResourceFields()
    {
        List<ResourceField> allResourceFields = new List<ResourceField>();
        allResourceFields = FindObjectsOfType<ResourceField>().ToList();

        foreach (ResourceField resourceField in allResourceFields)
        {
            if (Vector3.Distance(resourceField.transform.position, transform.position) < distanceToResources)
            {
                resourceFields.Add(resourceField);
            }
        }
    }
}
