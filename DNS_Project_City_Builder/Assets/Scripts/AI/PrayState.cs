using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayState : IAI
{
    private Spirit spirit;

    private Altar altar;
    private bool altarExists;
    private bool isPraying;
    private bool collecting;

    public PrayState(Spirit spirit)
    {
        this.spirit = spirit;
        altarExists = false;
        isPraying = false;
        altar = null;
        collecting = false;
    }

    public void UpdateActions()
    {
        if (!altarExists)
        {
            FindAltar();
        }
        else
        {
            if (!isPraying)
            {
                if (Mathf.Sqrt(Mathf.Pow(spirit.transform.position.x - spirit.placeToStay.position.x, 2)) < 1f
                    && Mathf.Sqrt(Mathf.Pow(spirit.transform.position.z - spirit.placeToStay.position.z, 2)) < 1f)
                {
                    spirit.SpiritAnimation = SpiritAnimationState.Idle;
                    spirit.agent.SetDestination(spirit.transform.position);
                    isPraying = true;
                }
            }
            else
            {
                //animations
                if (Mathf.Abs(Vector3.SignedAngle(spirit.transform.forward, altar.transform.position - spirit.transform.position, Vector3.up)) > 3f)
                {
                    Vector3 dir = (altar.transform.position - spirit.transform.position).normalized;
                    spirit.transform.rotation = Quaternion.RotateTowards(spirit.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 100f);

                }
                else
                {
                    spirit.SpiritAnimation = SpiritAnimationState.Praying;
                    if(!collecting)
                    {
                        //altar.WorkersAmount++;
                        altar.CountWorkers();
                        ResourceManagement.Instance.ChangedSpiritsCollectingResources(true);
                        collecting = true;
                    }
                }
            }

        }

        if (!spirit.Working)
        {
            ToIdle();
        }
    }

    public void ToIdle()
    {
        spirit.SpiritAnimation = SpiritAnimationState.Idle;

        altarExists = false;
        isPraying = false;

        altar.CountWorkers();
        //altar.WorkersAmount--;
        ResourceManagement.Instance.ChangedSpiritsCollectingResources(false);
        collecting = false;

        spirit.SpiritWork = SpiritWorkState.Idle;
        spirit.placeToStay = null;
        spirit.agent.SetDestination(spirit.transform.position);
        spirit.CurrentState = spirit.IdleState;
    }
    /*
    public void ToEscape()
    {
        spirit.SpiritAnimation = SpiritAnimationState.Idle;

        altar.WorkersAmount--;
        ResourceManagement.Instance.ChangedSpiritsCollectingResources(false);
        collecting = false;

        altarExists = false;
        isPraying = false;
        spirit.agent.isStopped = true;
        spirit.LastState = this;
        spirit.CurrentState = spirit.EscapeState;
    }*/

    private void FindAltar()
    {
        altar = (Altar)spirit.workPlace;

        if (!spirit.placeToStay)
        {
            int index = Random.Range(0, altar.PlacesToStay.Count - 1);
            spirit.placeToStay = altar.TakePlace(index);
        }
        spirit.SpiritAnimation = SpiritAnimationState.Walking;

        this.spirit.agent.SetDestination(spirit.placeToStay.position);
        altarExists = true;
    }
}
