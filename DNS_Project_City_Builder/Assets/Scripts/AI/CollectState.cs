using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectState : IAI
{
    private float distanceToGetResource = 2f;
    private float distanceToPutDownResource = 4f;

    private Spirit spirit;
    private ResourceBuilding building;
    private WeaversHut weaversHut;
    private ResourceField resourceField;
    private bool carryingResource;
    private bool foundResourceField;
    private bool weaversHutExists
    {
        get { return buildingType == BuildingType.weawersHut; }
    }
    private bool isGoingToWeaversHut;
    private BuildingType buildingType;
    private bool CollectingResources;
    enum BuildingType
    {
        nullState,
        weawersHut,
        generator
    }

    public CollectState(Spirit spirit)
    {
        this.spirit = spirit;
        carryingResource = false;
        foundResourceField = false;
        weaversHut = null;
        building = null;
        resourceField = null;
        isGoingToWeaversHut = false;
        buildingType = BuildingType.nullState;
        CollectingResources = false;
    }

    public void UpdateActions()
    {
        if (!spirit.Working) ToIdle();
        
        else if (buildingType == BuildingType.nullState)
        {
            FindWeaversHut();
        }
        else
        {
            if (buildingType == BuildingType.generator)
            {
                if (Vector3.Distance(spirit.transform.position, spirit.placeToStay.position) < 2f)
                //if(spirit.agent.remainingDistance < 2f)
                {
                    spirit.agent.SetDestination(spirit.transform.position);
                    spirit.SpiritAnimation = SpiritAnimationState.Idle;
                    if (!CollectingResources)
                    {
                        building.CountWorkers();
                        //building.WorkersAmount++;
                        ResourceManagement.Instance.ChangedSpiritsCollectingResources(true);
                        CollectingResources = true;
                    }
                    spirit.IsVisible = false;
                }


                return;
            }
            else if (weaversHut.resourceFields.Count <= 0)
            {
                if (Vector3.Distance(spirit.transform.position, building.transform.position) < 2f)
                {
                    spirit.agent.SetDestination(spirit.transform.position);
                    spirit.SpiritAnimation = SpiritAnimationState.Idle;
                    if (!CollectingResources)
                    {
                        building.CountWorkers();
                        //building.WorkersAmount++;
                        ResourceManagement.Instance.ChangedSpiritsCollectingResources(true);
                        CollectingResources = true;
                    }
                    if (weaversHut != building)
                    {
                        spirit.IsVisible = false;
                    }
                }
                return;
            }
            if (!carryingResource)
            {
                if (!CollectingResources)
                {
                    building.CountWorkers();
                   // building.WorkersAmount++;
                    ResourceManagement.Instance.ChangedSpiritsCollectingResources(true);
                    CollectingResources = true;
                }

                if (!foundResourceField)
                {
                    int indexOfResourceField = UnityEngine.Random.Range(0, weaversHut.resourceFields.Count - 1);
                    resourceField = weaversHut.resourceFields[indexOfResourceField];

                    spirit.SpiritAnimation = SpiritAnimationState.Walking;

                    spirit.agent.SetDestination(weaversHut.resourceFields[indexOfResourceField].transform.position);
                    foundResourceField = true;
                }
                else
                {
                    if (Vector3.Distance(spirit.transform.position, resourceField.transform.position) < distanceToGetResource)
                    {
                        //animation raising resource
                        spirit.SpiritAnimation = SpiritAnimationState.CarringResource;
                        spirit.Delay(0.1f, () => spirit.TakeResource(true));


                        carryingResource = true;
                    }
                }
            }
            else
            {
                if (!isGoingToWeaversHut)
                {
                    spirit.SpiritAnimation = SpiritAnimationState.CarringResource;

                    spirit.agent.SetDestination(weaversHut.transform.position);
                    isGoingToWeaversHut = true;
                }
                else
                {
                    if (Vector3.Distance(spirit.transform.position, weaversHut.transform.position) < distanceToPutDownResource)
                    {
                        spirit.SpiritAnimation = SpiritAnimationState.Idle;

                        //animation raising resource
                        spirit.TakeResource(false);
                        carryingResource = false;
                        isGoingToWeaversHut = false;
                        foundResourceField = false;
                    }
                }
            }
        }
    }

    public void ToIdle()
    {
        if (carryingResource)
        {
            spirit.SpiritAnimation = SpiritAnimationState.Idle;
            spirit.TakeResource(false);
            carryingResource = false;
        }

        spirit.IsVisible = true;
        spirit.SpiritAnimation = SpiritAnimationState.Idle;

        building.CountWorkers();
       // building.WorkersAmount--;
        ResourceManagement.Instance.ChangedSpiritsCollectingResources(false);
        CollectingResources = false;

        foundResourceField = false;
        isGoingToWeaversHut = false;
        spirit.SpiritWork = SpiritWorkState.Idle;
        spirit.placeToStay = null;
        spirit.agent.SetDestination(spirit.transform.position);
        building = null;
        weaversHut = null;
        buildingType = BuildingType.nullState;
        spirit.CurrentState = spirit.IdleState;
    }

   /* public void ToEscape()
    {
        if (carryingResource)
        {

            spirit.SpiritAnimation = SpiritAnimationState.Idle;

            spirit.TakeResource(false);
            carryingResource = false;
        }
        if (weaversHut != building)
        {
            spirit.IsVisible = true;
        }
        building.WorkersAmount--;
        ResourceManagement.Instance.ChangedSpiritsCollectingResources(false);
        CollectingResources = false;

        foundResourceField = false;
        isGoingToWeaversHut = false;
        spirit.agent.isStopped = true;
        spirit.LastState = this;
        buildingType = BuildingType.nullState;
        building = null;
        weaversHut = null;
        spirit.CurrentState = spirit.EscapeState;
    }*/

    private void FindWeaversHut()
    {
        if (spirit.workPlace is WeaversHut)
        {
            weaversHut = (WeaversHut)spirit.workPlace;
            building = spirit.workPlace as ResourceBuilding;
            buildingType = BuildingType.weawersHut;
        }
        else if (spirit.workPlace is Generator)
        {
            building = spirit.workPlace as ResourceBuilding;
            buildingType = BuildingType.generator;
        }
        else
            buildingType = BuildingType.nullState;

        if (!spirit.placeToStay)
        {
            int index = UnityEngine.Random.Range(0, building.PlacesToStay.Count - 1);
            spirit.placeToStay = building.TakePlace(index);
        }

        try
        {
            this.spirit.agent.SetDestination(spirit.placeToStay.position);
        }
        catch (NullReferenceException)
        {
            buildingType = BuildingType.nullState;
        };
        spirit.SpiritAnimation = SpiritAnimationState.Walking;

    }
}
