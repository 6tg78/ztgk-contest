using UnityEngine;

public class ResearchState : IAI
{
    private Spirit spirit;
    private Building workshop;
    private bool foundWorkshop;
    private bool inWorkshop;

    public ResearchState(Spirit spirit)
    {
        this.spirit = spirit;
        workshop = null;
        foundWorkshop = false;
        inWorkshop = false;
    }

    public void UpdateActions()
    {
        if (!foundWorkshop)
        {
            FindWorkshop();
        }
        else if (!inWorkshop)
        {
            if (Mathf.Sqrt(Mathf.Pow(spirit.transform.position.x - spirit.placeToStay.position.x, 2)) < 1f
                    && Mathf.Sqrt(Mathf.Pow(spirit.transform.position.z - spirit.placeToStay.position.z, 2)) < 1f)
            {
                spirit.SpiritAnimation = SpiritAnimationState.Idle;
                inWorkshop = true;
                workshop.CountWorkers();
                spirit.agent.SetDestination(spirit.transform.position);
                spirit.IsVisible = false;
            }
        }

        if (!spirit.Working)
        {
            ToIdle();
        }
    }/*
    public void ToEscape()
    {
        spirit.IsVisible = true;
        spirit.agent.isStopped = true;
        spirit.LastState = this;
        workshop = null;
        foundWorkshop = false;
        inWorkshop = false;
        spirit.CurrentState = spirit.EscapeState;
    }*/

    public void ToIdle()
    {
        spirit.IsVisible = true;
        inWorkshop = false;
        spirit.SpiritAnimation = SpiritAnimationState.Idle;
        spirit.SpiritWork = SpiritWorkState.Idle;
        workshop.CountWorkers();
        spirit.placeToStay = null;
        spirit.agent.SetDestination(spirit.transform.position);
        foundWorkshop = false;
        spirit.CurrentState = spirit.IdleState;
    }

    private void FindWorkshop()
    {
        workshop = spirit.workPlace;

        if (!spirit.placeToStay)
        {
            int index = Random.Range(0, workshop.PlacesToStay.Count - 1);
            spirit.placeToStay = workshop.TakePlace(index);
        }
        spirit.SpiritAnimation = SpiritAnimationState.Walking;
        spirit.agent.SetDestination(spirit.placeToStay.position);
        foundWorkshop = true;
    }
}

