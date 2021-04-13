using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState : IAI
{
    private Spirit spirit;

    public EscapeState(Spirit spirit)
    {
        this.spirit = spirit;
    }

    public void UpdateActions()
    {
        if (!spirit.waitUntilEnemiesLeave)
        {
            ToIdle();
            //spirit.currentState = spirit.lastState;
        }

        if (!spirit.nearestEnemy)
        {
            ToIdle();
            //spirit.currentState = spirit.lastState;
        }

        if (spirit.homeless || Vector3.Distance(spirit.transform.position, spirit.nearestEnemy.transform.position) > Vector3.Distance(spirit.home.transform.position, spirit.nearestEnemy.transform.position))
        {
            Vector3 dir = (spirit.transform.position - spirit.nearestEnemy.transform.position).normalized;
            spirit.agent.Move(dir * spirit.escapeSpeed * Time.deltaTime);
            spirit.transform.Rotate(new Vector3(0, Vector3.SignedAngle(dir, spirit.transform.forward, Vector3.up) < 0 ? 1 : -1, 0) * Time.deltaTime * 100f);

            spirit.SpiritAnimation = SpiritAnimationState.Walking;


            if (Vector3.Distance(spirit.transform.position, spirit.nearestEnemy.transform.position) > spirit.distanceToTriggerEscape)
            {
                spirit.agent.isStopped = false;

                spirit.SpiritAnimation = SpiritAnimationState.Idle;

                ToIdle();
                //spirit.currentState = spirit.lastState;
            }
        }
        else if (!spirit.atHome)
        {
            spirit.agent.isStopped = false;

            spirit.SpiritAnimation = SpiritAnimationState.Walking;

            spirit.agent.SetDestination(spirit.home.transform.position);

            if (Vector3.Distance(spirit.transform.position, spirit.home.transform.position) < 2f)
            {
                spirit.atHome = true;
            }
        }
        else
        {
            spirit.SpiritAnimation = SpiritAnimationState.Walking;
            spirit.gameObject.SetActive(false);
        }


    }

    public void ToIdle()
    {
        spirit.CurrentState = spirit.IdleState;
    }

    public void ToEscape()
    {

    }
}
