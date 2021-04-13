using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class WarriorState : IAI
{
    private Spirit spirit;
    private Barracks guardHouse;
    private bool isGuardHouse;
    private bool atGuardHouse;
    private float distanceToGuardHouse;
    private bool isAttacking = false;
    private float range;

    public WarriorState(Spirit spirit)
    {
        this.spirit = spirit;
        isGuardHouse = false;
        atGuardHouse = false;
        distanceToGuardHouse = 2f;
    }

    public void UpdateActions()
    {
        if (!isGuardHouse)
        {
            FindGuardHouse();
        }
        if (spirit.WarriorIdle)
        {
            if (!atGuardHouse)
            {
                spirit.agent.SetDestination(spirit.placeToStay.transform.position);
                spirit.SpiritAnimation = SpiritAnimationState.Walking;
                if (Vector3.Distance(spirit.transform.position, spirit.placeToStay.transform.position) < distanceToGuardHouse)
                {
                    atGuardHouse = true;
                    spirit.Warrior = true;
                    guardHouse.Warriors.Add(spirit);
                    spirit.SpiritAnimation = SpiritAnimationState.Idle;
                    spirit.agent.SetDestination(spirit.transform.position);
                    spirit.IsVisible = false;
                }
            }
        }
        else
        {
            if(spirit.nearestEnemy == null) 
                FindNearestEnemy();
            if (spirit.nearestEnemy == null)
            {
                spirit.WarriorIdle = true;
                return;
            }
            else
                spirit.gameObject.transform.LookAt(spirit.nearestEnemy.transform);//probably hack but worth of test
            atGuardHouse = false;
            spirit.agent.SetDestination(spirit.nearestEnemy.transform.position);
            spirit.SpiritAnimation = SpiritAnimationState.Walking;

            if (Vector3.Distance(spirit.transform.position, spirit.nearestEnemy.transform.position) < 2f)
            {

                spirit.SpiritAnimation = SpiritAnimationState.Idle;

                spirit.agent.SetDestination(spirit.transform.position);
                if (!isAttacking)
                {
                    spirit.SpiritAnimation = SpiritAnimationState.Attack;
                    spirit.Delay(spirit.Animator.GetCurrentAnimatorStateInfo(0).length, delegate { isAttacking = true; }, delegate { Attack(); isAttacking = false; });
                }
                spirit.nearestEnemy.currentState.ToFight();
                spirit.nearestEnemy.SpiritWarrior = spirit;
            }

        }
    }

    public void ToIdle()
    {
        Debug.Log("Useless");
    }

    public void ToEscape()
    {
        Debug.Log("Useless");
    }

    private void FindGuardHouse()
    {
        guardHouse = (Barracks)spirit.workPlace;
        range = guardHouse.Ray;
        if (!spirit.workPlace)
        {
            return;
        }

        if (!spirit.placeToStay)
        {
            int index = Random.Range(0, guardHouse.PlacesToStay.Count - 1);
            spirit.placeToStay = guardHouse.TakePlace(index);
        }

        spirit.SpiritAnimation = SpiritAnimationState.Walking;
        spirit.agent.SetDestination(spirit.placeToStay.transform.position);
        isGuardHouse = true;
    }

    private void Attack()
    {
        spirit.nearestEnemy?.GetDamage(spirit.AttackDamage);
        if (spirit.nearestEnemy == null || spirit.nearestEnemy.IsDead)
            FindNearestEnemy();
        spirit.SpiritAnimation = SpiritAnimationState.Idle;
        //TODO: here should be placed Animation & HitDamage to nearestEnemy
    }

    private void FindNearestEnemy()
    {
        if (EnemyManager.Instance.enemies.Count > 0)
        {
            float distanceToEnemy = Mathf.Infinity;
            foreach (Enemy enemy in EnemyManager.Instance.enemies)
            {

                float distance = Vector3.Distance(enemy.transform.position, spirit.transform.position);
                if (distance > range) continue;
                if (distance < distanceToEnemy)
                {
                    distanceToEnemy = distance;
                    spirit.nearestEnemy = enemy;
                }
            }
            if (spirit.nearestEnemy != null)
            {
                spirit.WarriorIdle = false;
                spirit.SpiritAnimation = SpiritAnimationState.Walking;
                spirit.agent.SetDestination(spirit.nearestEnemy.transform.position);
            }
            //else
            //    FindNearestEnemy();
        }
        else
        {
            spirit.WarriorIdle = true;
            spirit.SpiritAnimation = SpiritAnimationState.Walking;
            spirit.agent.SetDestination(spirit.placeToStay.transform.position);
        }
        
    }
}

