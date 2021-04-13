using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public Spirit SpiritWarrior { get; set; }
    private float _hitPoints;
    public float HitPoints {
        get { return _hitPoints; } 
        private set { _hitPoints = value; OnHitPointsChanged?.Invoke(); } 
    }
    public float MaxHitPoints { get; private set; }
    public event Action OnHitPointsChanged;
    private Spirit nearestSpirit;
    private Building nearestBuilding;
    private bool spiritIsCloser = false;
    public bool IsDead { get; private set; }
    private EnemyAnimationState enemyAnimation;
    public EnemyAnimationState EnemyAnimation
    {
        get => enemyAnimation;
        set { SetAnimationState(value); }
    }
    public event Action OnAnimationStateChange;
    public Animator Animator { get; private set; }

    [HideInInspector]
    public IEnemyAI currentState;
    [HideInInspector]
    public AttackState attackState;
    [HideInInspector]
    public DestroyState destroyState;
    [HideInInspector]
    public FightState fightState;
    [HideInInspector]
    public NavMeshAgent agent;

    public float AttackDamage { get; private set; }
    public float AttackSpeed { get; private set; }

    [SerializeField]
    private AudioSource gettingDamageSound;

    private void Awake()
    {
        attackState = new AttackState(this);
        destroyState = new DestroyState(this);
        fightState = new FightState(this);
        agent = GetComponent<NavMeshAgent>();
        SpiritWarrior = null;
        IsDead = false;
        Animator = GetComponentInChildren<Animator>();
        GetComponent<EnemyUI>().Bind(this);
    }

    private void Start()
    {
        OnAnimationStateChange += ChangeAnimation;
        UpdateStatistics();
        GetOrder();
    }
    public void UpdateStatistics()
    {
        HitPoints = BalancePanel.Instance.EnemyWarrior.hitPoints;
        MaxHitPoints = HitPoints;
        agent.speed = BalancePanel.Instance.EnemyWarrior.movementSpeed;
        AttackDamage = BalancePanel.Instance.EnemyWarrior.attackDamage;
        AttackSpeed = 2f; //TODO: change for animation duration
    }

    private void Update()
    {
        currentState.UpdateActions();
    }

    public void GetOrder()
    {
        //if (UnityEngine.Random.Range(0, 100) % 2 == 0)
        //{
        //    currentState = attackState;
        //}
        //else
        //{
        //    currentState = destroyState;
        //}
        FindNearestTarget();
        if (spiritIsCloser)
        {
            currentState = attackState;
        }
        else
        {
            currentState = destroyState;
        }

    }

    public void GetDamage(float dmgAmount) //TODO: ~KS~ I'm using it in Trap.cs. Remember to implement it please.
    {
        if (IsDead) return;
        HitPoints -= dmgAmount;
        if(gettingDamageSound != null)
        {
            gettingDamageSound.Play();
        }
        else
        {
            Debug.Log("GettingDamageSound in Enemy.cs isn't assigned.");
        }
        if (HitPoints <= 0f)
        {

            IsDead = true;
            Death(); //TODO: make a delay of destroy to have some time for animation of dying
        }
    }

    private void Death()
    {
        GetComponent<EnemyUI>().Unbind();
        EnemyManager.Instance.enemies.Remove(this);
        OnAnimationStateChange -= ChangeAnimation;
        Destroy(gameObject);
    }
    public void BecomeStunned(float duration) //TODO: ~KS~ I'm using it in Trap.cs. Remember to implement it please.
    {

        agent.isStopped = true; //needs testing
        this.Delay(duration, () =>
        {
            agent.isStopped = false;
        });
    }

    private void FindNearestTarget()
    {
        float nearestSpiritDistance = Mathf.Infinity;

        float nearestBuildingDistance = Mathf.Infinity;
        float distance;

        foreach (Spirit spirit in AIManager.Instance.spirits)
        {
            if (spirit.atHome) continue;
            distance = Vector3.Distance(gameObject.transform.position, spirit.transform.position);
            if (distance < nearestSpiritDistance)
            {
                nearestSpiritDistance = distance;
                nearestSpirit = spirit;
            }
        }
        foreach (Building building in FindObjectsOfType<Building>())
        {
            if (building.InConstrucionPlanningMode || building.IsBeingDestroyed) continue;
            distance = Vector3.Distance(gameObject.transform.position, building.transform.position);
            if (distance < nearestBuildingDistance)
            {
                nearestBuildingDistance = distance;
                nearestBuilding = building;
            }
        }

        if (nearestBuildingDistance > nearestSpiritDistance) spiritIsCloser = true;
        else spiritIsCloser = false;
    }

    private void SetAnimationState(EnemyAnimationState state)
    {
        if (enemyAnimation != state)
        {
            enemyAnimation = state;
            OnAnimationStateChange?.Invoke();
        }
    }
    public virtual void ChangeAnimation()
    {
        Animator.speed = enemyAnimation == EnemyAnimationState.Walk ? 1.6f : 1f;//temporary solution - thinking of better one 
        Animator.SetInteger("ChangeAnimation", (int)enemyAnimation);
    }
}
