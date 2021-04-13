using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightState : IEnemyAI
{
    private Enemy enemy;
    private bool isAttacking;
    public FightState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void UpdateActions()
    {

        if (enemy.SpiritWarrior)
        {
            enemy.EnemyAnimation = EnemyAnimationState.Walk;
            enemy.agent.SetDestination(enemy.SpiritWarrior.transform.position);

            if (Vector3.Distance(enemy.transform.position, enemy.SpiritWarrior.transform.position) < 2f)
            {
                enemy.EnemyAnimation = EnemyAnimationState.Idle;
                enemy.agent.SetDestination(enemy.transform.position);
                if (!isAttacking)
                {
                    enemy.EnemyAnimation = EnemyAnimationState.Attack;
                    enemy.Delay(enemy.Animator.GetCurrentAnimatorStateInfo(0).length, delegate { isAttacking = true; }, delegate { Attack(); isAttacking = false; });
                }
            }
        }
        else
        {
            enemy.EnemyAnimation = EnemyAnimationState.Idle;
            enemy.agent.SetDestination(enemy.transform.position);

            enemy.GetOrder();
        }

    }

    public void ToAttack()
    {
        enemy.currentState = enemy.attackState;
    }

    public void ToDestroy()
    {
        enemy.currentState = enemy.destroyState;
    }

    public void ToFight()
    {

    }

    private void Attack()
    {
        //animation

        //wait for end animation
        if (!enemy.SpiritWarrior)
        {
            enemy.EnemyAnimation = EnemyAnimationState.Idle;
            return;
        }

        if (!enemy.SpiritWarrior.IsDead) 
            enemy.SpiritWarrior.GetDamage(enemy.AttackDamage);
        enemy.GetOrder();
        enemy.EnemyAnimation = EnemyAnimationState.Idle;
    }
}
