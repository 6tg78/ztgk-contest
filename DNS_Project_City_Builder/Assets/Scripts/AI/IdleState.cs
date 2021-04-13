using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IAI
{
    private Spirit spirit;

    private float distanceToHome;

    public IdleState(Spirit spirit)
    {
        this.spirit = spirit;
        distanceToHome = 2f;
    }

    public void UpdateActions()
    {
        if (spirit.Working)
        {
            if (spirit.ConstructingWork) ToBuild();
            if (spirit.CollectingWork) ToCollect();
            if (spirit.PrayingWork) ToPray();
            if (spirit.GuardingWork) ToBeingWarrior();
            if (spirit.ShootingWork) ToDefense();
            if (spirit.ResearchingWork) ToResearch();
        }
        else
        {
            if (!spirit.home)
            {
                FindHome();
            }
            else
            {
                if (!spirit.atHome)
                {
                    spirit.SpiritAnimation = SpiritAnimationState.Walking;

                    spirit.agent.SetDestination(spirit.home.transform.position);

                    if (Vector3.Distance(spirit.transform.position, spirit.home.transform.position) < distanceToHome)
                    {
                        spirit.atHome = true;
                        spirit.home.PlayEnteringShelterSound();
                    }
                }
                else
                {
                    spirit.SpiritAnimation = SpiritAnimationState.Idle;
                    spirit.gameObject.SetActive(false);
                }
            }
        }

    }

    public void ToPray()
    {
        spirit.gameObject.SetActive(true);
        spirit.atHome = false;
        spirit.CurrentState = spirit.PrayState;
    }

    public void ToBuild()
    {
        spirit.gameObject.SetActive(true);
        spirit.atHome = false;
        spirit.CurrentState = spirit.BuildState;
    }

    public void ToCollect()
    {
        spirit.gameObject.SetActive(true);
        spirit.atHome = false;
        spirit.CurrentState = spirit.CollectState;
    }

    public void ToBeingWarrior()
    {
        spirit.gameObject.SetActive(true);
        spirit.atHome = false;
        spirit.CurrentState = spirit.WarriorState;
    }
    public void ToDefense()
    {
        spirit.gameObject.SetActive(true);
        spirit.atHome = false;
        spirit.CurrentState = spirit.DefenseState;
    }
    public void ToResearch()
    {
        spirit.gameObject.SetActive(true);
        spirit.atHome = false;
        spirit.CurrentState = spirit.ResearchState;
    }
    public void ToIdle()
    {
        Debug.Log("Im here");
    }

    /*public void ToEscape()
    {
        spirit.agent.isStopped = true;
        spirit.LastState = this;
        spirit.CurrentState = spirit.EscapeState;
    }
    */
    private void FindHome()
    {
        foreach (Shelter home in GameManager.Instance.houses)
        {
            if (home.WorkType == WorkTypeEnum.Constructing) continue;
            if (home.Spirits.Count < home.MaxSpirits)
            {
                home.Spirits.Add(this.spirit);
                spirit.home = home;
                spirit.atHome = false;
                return;
            }
        }

    }
}
