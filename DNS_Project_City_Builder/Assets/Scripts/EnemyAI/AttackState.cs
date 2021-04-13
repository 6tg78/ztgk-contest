using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IEnemyAI
{
    private Enemy enemy;
    private Spirit spiritToAttack;
    private bool isAttacking;

    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
        spiritToAttack = null;
        isAttacking = false;
    }

    public void UpdateActions()
    {
        if (AIManager.Instance.spirits.Count > 0)
        {
            spiritToAttack = FindSpiritToAttack();
        }

        if (spiritToAttack)
        {
            enemy.EnemyAnimation = EnemyAnimationState.Walk;
            enemy.agent.SetDestination(spiritToAttack.transform.position);

            if (Vector3.Distance(enemy.transform.position, spiritToAttack.transform.position) < 2f)
            {
                if (!spiritToAttack.Warrior)
                {
                    if (!isAttacking)
                    {
                        enemy.EnemyAnimation = EnemyAnimationState.Attack;
                        enemy.Delay(enemy.Animator.GetCurrentAnimatorStateInfo(0).length, delegate { isAttacking = true; }, delegate { Attack(); isAttacking = false; });
                    }
                }
                else
                {
                    enemy.SpiritWarrior = spiritToAttack;
                    ToFight();
                }

            }
        }
        else
        {
            enemy.EnemyAnimation = EnemyAnimationState.Idle;
            enemy.agent.SetDestination(enemy.transform.position);

            ToDestroy();
        }

    }

    public void ToAttack()
    {
        Debug.Log("Im here");
    }

    public void ToDestroy()
    {
        spiritToAttack = null;
        enemy.currentState = enemy.destroyState;
    }

    public void ToFight()
    {
        spiritToAttack = null;
        enemy.currentState = enemy.fightState;
    }

    private Spirit FindSpiritToAttack()
    {
        Spirit nearestSpirit = null;
        float minDistance = Mathf.Infinity;

        foreach (Spirit spirit in AIManager.Instance.spirits)
        {
            if (spirit.atHome) continue;

            float distance = Vector3.Distance(enemy.transform.position, spirit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSpirit = spirit;
            }
        }

        return nearestSpirit;
    }

    private void Attack()
    {
        //animation

        //wait for end animation
        if (!spiritToAttack) return;

        if(!spiritToAttack.IsDead) spiritToAttack.GetDamage(enemy.AttackDamage);
        enemy.GetOrder();

        enemy.EnemyAnimation = EnemyAnimationState.Idle;
    }
}