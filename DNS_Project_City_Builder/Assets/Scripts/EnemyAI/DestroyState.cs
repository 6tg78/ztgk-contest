using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyState : IEnemyAI
{
    private Enemy enemy;

    private Building buildingToDestroy;

    private bool startedDestroyingBuilding = false;

    public DestroyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void UpdateActions()
    {
        if (GameManager.Instance.buildings.Count > 0 && buildingToDestroy == null)
        {
            buildingToDestroy = FindBuildingToDestroy();
        }

        if (buildingToDestroy)
        {
            if (Vector3.Distance(enemy.transform.position, buildingToDestroy.transform.position) < 4f)
            {
                if (!startedDestroyingBuilding)
                {
                    enemy.EnemyAnimation = EnemyAnimationState.Attack;
                    enemy.StartCoroutine(DestroyBuilding());
                }
            }
        }
        else
        {
            enemy.EnemyAnimation = EnemyAnimationState.Idle;
            enemy.agent.SetDestination(enemy.transform.position);

            ToAttack();
        }
    }

    public void ToAttack()
    {
        buildingToDestroy = null;
        enemy.currentState = enemy.attackState;
    }

    public void ToDestroy()
    {
        Debug.Log("Im here");
    }

    public void ToFight()
    {
        buildingToDestroy = null;
        enemy.currentState = enemy.fightState;
    }

    private Building FindBuildingToDestroy()
    {
        Building nearestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (Building building in GameManager.Instance.buildings)
        {
            if (building.InConstrucionPlanningMode) continue;
            float distance = Vector3.Distance(enemy.transform.position, building.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestBuilding = building;
            }
        }
        enemy.agent.SetDestination(nearestBuilding.transform.position);
        enemy.EnemyAnimation = EnemyAnimationState.Walk;

        return nearestBuilding;
    }

    private IEnumerator DestroyBuilding()
    {
        //animations place here
        enemy.EnemyAnimation = EnemyAnimationState.Attack;
        startedDestroyingBuilding = true;
        buildingToDestroy?.GetDamage(enemy.AttackDamage);

        if (buildingToDestroy != null && !buildingToDestroy.IsBeingDestroyed)
        {
            yield return new WaitForSeconds(enemy.Animator.GetCurrentAnimatorStateInfo(0).length);
            buildingToDestroy?.GetDamage(enemy.AttackDamage);
        }
        else
            enemy.GetOrder();
        startedDestroyingBuilding = false;
        enemy.EnemyAnimation = EnemyAnimationState.Idle;
    }
}
