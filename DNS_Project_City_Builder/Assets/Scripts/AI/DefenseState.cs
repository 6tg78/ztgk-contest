using UnityEngine;

public class DefenseState : IAI
{
    private Spirit spirit;
    private Turret normalTurret;
    private LongRangeTurret longRangeTurret;
    private Building turret;
    private bool foundTurret;
    private bool inTurret;

    public DefenseState(Spirit spirit)
    {
        this.spirit = spirit;
        foundTurret = false;
        inTurret = false;
    }
    public void UpdateActions()
    {
        if(!foundTurret)
        {
            FindTower();
        }
        else if(!inTurret)
        {
            if (Mathf.Sqrt(Mathf.Pow(spirit.transform.position.x - spirit.placeToStay.position.x, 2)) < 1f
                    && Mathf.Sqrt(Mathf.Pow(spirit.transform.position.z - spirit.placeToStay.position.z, 2)) < 1f)
            {
                spirit.SpiritAnimation = SpiritAnimationState.Idle;
                inTurret = true;
                spirit.agent.SetDestination(spirit.transform.position);
                _ = turret is Turret ? normalTurret.IsOperative = true : longRangeTurret.IsOperative = true;
                spirit.IsVisible = false;
                //spirit.gameObject.SetActive(false); //spirit is sitting in turret
            }
        }

        if (!spirit.Working)
        {
            ToIdle();
        }
    }
    /*public void ToEscape()
    {
        //there is no escape hehe
    }*/

    public void ToIdle()
    {
        spirit.IsVisible = true;
        inTurret = false;
        //spirit.gameObject.SetActive(true);
        spirit.SpiritAnimation = SpiritAnimationState.Idle;
        spirit.SpiritWork = SpiritWorkState.Idle;
        spirit.placeToStay = null;
        //turret.IsOperative = false;
        _ = turret is Turret ? normalTurret.IsOperative = false : longRangeTurret.IsOperative = false;
        spirit.agent.SetDestination(spirit.transform.position);
        foundTurret = false;
        spirit.CurrentState = spirit.IdleState;
    }

    private void FindTower()
    {
        turret = spirit.workPlace;
        if (turret is Turret)
            normalTurret = turret as Turret;
        else
            longRangeTurret = turret as LongRangeTurret;

        if (!spirit.placeToStay)
        {
            int index = Random.Range(0, turret.PlacesToStay.Count - 1);
            spirit.placeToStay = turret.TakePlace(index);
        }
        spirit.SpiritAnimation = SpiritAnimationState.Walking;
        spirit.agent.SetDestination(spirit.placeToStay.position);
        foundTurret = true;
    }
}

