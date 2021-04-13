using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildState : IAI
{
    private Spirit spirit;
    private Building buildingToConstruct;
    private bool isConstructing;
    private bool building;

    public BuildState(Spirit spirit)
    {
        this.spirit = spirit;
        isConstructing = false;
        buildingToConstruct = null;
        building = false;
    }

    public void UpdateActions()
    {
        if (!spirit.Working)
        {
            ToIdle();
            return;
        }
        if (!building) // if construction for this spirit doesn't exist
        {
            FindBuildingToConstruct();
        }
        else
        {
            if (!isConstructing)
            {
                if (Mathf.Sqrt(Mathf.Pow(spirit.transform.position.x - spirit.placeToStay.position.x, 2)) < 1f
                    && Mathf.Sqrt(Mathf.Pow(spirit.transform.position.z - spirit.placeToStay.position.z, 2)) < 1f)
                {
                    spirit.SpiritAnimation = SpiritAnimationState.Idle;

                    spirit.agent.SetDestination(spirit.transform.position);
                    buildingToConstruct.SpiritsConstructingMe++;
                    isConstructing = true;
                }
            }
            else
            {
                // here will be the animation scripts etc.
                if (Mathf.Abs(Vector3.SignedAngle(spirit.transform.forward, buildingToConstruct.transform.position - spirit.transform.position, Vector3.up)) > 5f)
                {
                    Vector3 dir = (buildingToConstruct.transform.position - spirit.transform.position).normalized;
                    spirit.transform.rotation = Quaternion.RotateTowards(spirit.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 100f);

                }
                else
                    spirit.SpiritAnimation = SpiritAnimationState.Building;


                if (buildingToConstruct.FinishedConstructing)
                {
                    //buildingToConstruct.SpiritsConstructingMe--;
                    buildingToConstruct.Spirits.Remove(spirit);
                    buildingToConstruct.GiveBackPlace(spirit.placeToStay);
                    ToIdle();
                    //AIManager.Instance.RemoveSpiritFromWork(spirit.workPlace, spirit);
                }

            }
        }

        
    }

    public void ToIdle()
    {
        spirit.SpiritAnimation = SpiritAnimationState.Idle;

        isConstructing = false;
        if (buildingToConstruct && !buildingToConstruct.FinishedConstructing && buildingToConstruct.SpiritsConstructingMe > 0)
            buildingToConstruct.SpiritsConstructingMe--;
        buildingToConstruct = null;
        building = false;

        spirit.SpiritWork = SpiritWorkState.Idle;
        spirit.placeToStay = null;
        spirit.agent.SetDestination(spirit.transform.position);
        spirit.CurrentState = spirit.IdleState;
    }
    /*
    public void ToEscape()
    {
        spirit.SpiritAnimation = SpiritAnimationState.Idle;

        isConstructing = false;
        if (buildingToConstruct != null && !buildingToConstruct.FinishedConstructing && buildingToConstruct.SpiritsConstructingMe > 0) buildingToConstruct.SpiritsConstructingMe--; //FIXME: Excpetion raised here, when spirits are escaping.
        buildingToConstruct = null;
        building = false;
        spirit.agent.isStopped = true;
        spirit.LastState = this;
        spirit.CurrentState = spirit.EscapeState;
    }*/

    private void FindBuildingToConstruct()
    {
        buildingToConstruct = spirit.workPlace;
        if (!spirit.workPlace)
        {
            return;
        }

        if (!spirit.placeToStay)
        {
            int index = Random.Range(0, buildingToConstruct.PlacesToStay.Count - 1);
            spirit.placeToStay = buildingToConstruct.TakePlace(index);
        }
        spirit.SpiritAnimation = SpiritAnimationState.Walking;
        spirit.agent.SetDestination(spirit.placeToStay.position);
        building = true;
    }
}
