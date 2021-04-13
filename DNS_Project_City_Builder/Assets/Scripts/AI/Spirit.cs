using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Spirit : MonoBehaviour
{
    [SerializeField]
    private GameObject resource;
    [SerializeField]
    private GameObject spiritModel;
    [SerializeField]
    private GameObject warriorModel;

    private bool warrior = false;
    private Renderer[] renderers;

    [Header("Data to modify")]
    public int lifeEnergyConsumingPerDay;
    public float distanceToTriggerEscape;
    public float escapeSpeed;

    [Space]
    [Header("Data to analyze")]
    public int spiritID;
    public bool homeless;
    public bool waitUntilEnemiesLeave;
    public bool atHome;
    public float maxHitPoints;
    public float hitPoints;
    public bool Warrior
    {
        get { return warrior; }
        set { warrior = value; UpdateStatistics(); ChangeModelOnWarrior(value); }
    }
    public bool WarriorIdle { get; set; }
    public Shelter home;
    public Building workPlace;
    public Transform placeToStay;
    public Enemy nearestEnemy;

    public float AttackDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public Animator Animator { get; private set; }
    public bool IsWalking { get { return SpiritAnimation == SpiritAnimationState.Walking; } }
    public bool IsDead { get { return SpiritAnimation == SpiritAnimationState.Dead; } }
    public bool IsPraying { get { return SpiritAnimation == SpiritAnimationState.Praying; } }
    public bool IsWorking { get { return SpiritAnimation == SpiritAnimationState.Building; } }
    public bool IsCarringResource { get { return SpiritAnimation == SpiritAnimationState.CarringResource; } }
    public bool IsVisible
    {
        get
        {
            return gameObject.activeInHierarchy;
            //return renderers[0].enabled;
        }
        set
        {
            gameObject.SetActive(value);
            //foreach (Renderer renderer in renderers)
              //  renderer.enabled = value;
        }
    }

    private SpiritAnimationState spiritAnimation;
    public SpiritAnimationState SpiritAnimation { get => spiritAnimation; set { SetAnimationState(value); } }
    public event Action OnAnimationStateChange;



    public bool Working { get { return SpiritWork != SpiritWorkState.Idle; } }
    public bool GuardingWork { get { return SpiritWork == SpiritWorkState.Guarding; } }
    public bool ConstructingWork { get { return SpiritWork == SpiritWorkState.Constructing; } }
    public bool CollectingWork { get { return SpiritWork == SpiritWorkState.Collecting; } }
    public bool PrayingWork { get { return SpiritWork == SpiritWorkState.Praying; } }
    public bool ShootingWork { get { return SpiritWork == SpiritWorkState.UsingTurret; } }
    public bool ResearchingWork { get { return SpiritWork == SpiritWorkState.Researching; } }


    public BuildState BuildState { get; private set; }
    public IdleState IdleState { get; private set; }
    public CollectState CollectState { get; private set; }
    public PrayState PrayState { get; private set; }
   // public EscapeState EscapeState { get; private set; }
    public WarriorState WarriorState { get; private set; }
    public DefenseState DefenseState { get; private set; }
    public ResearchState ResearchState { get; private set; }

    public IAI CurrentState { get; set; }

    public IAI LastState { get; set; }

    public NavMeshAgent agent;

    public SpiritWorkState SpiritWork { get; set; }

    [Header("Sounds")]
    [SerializeField]
    private List<AudioSource> continuousSounds;
    [SerializeField] 
    private List<AudioSource> disposableSounds;
    private static bool fewFirstSecondsPassed = false;

    private void Awake()
    {
        StartCoroutine(CheckingIfFewFirstSecondsPassed());
        SetSpiritID();
        resource.SetActive(false);

        renderers = GetComponentsInChildren<Renderer>();

        IdleState = new IdleState(this);
        BuildState = new BuildState(this);
        CollectState = new CollectState(this);
        PrayState = new PrayState(this);
     //   EscapeState = new EscapeState(this);
        WarriorState = new WarriorState(this);
        DefenseState = new DefenseState(this);
        ResearchState = new ResearchState(this);
        agent = GetComponent<NavMeshAgent>();
        Animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        Animator.SetTrigger("startAnimation");
    }
    private void Start()
    {
        SpiritWork = SpiritWorkState.Idle;
        homeless = true;
        waitUntilEnemiesLeave = false;
        atHome = false;
        Warrior = false;
        WarriorIdle = true;
        CurrentState = IdleState;
        SpiritAnimation = SpiritAnimationState.Idle;
        UpdateStatistics();
        AttackSpeed = 2f;//TODO: change to attack animation duration
        resource.SetActive(false);

        OnAnimationStateChange += ChangeAnimation;

        // Playing the sound of spawning a spirit.
        if(disposableSounds[3].clip != null && fewFirstSecondsPassed)
        {
            disposableSounds[3].Play();
        }
    }

    private void Update()
    {
        CurrentState.UpdateActions();
        /*
        if (!Warrior)
        {
            if (IsEnemyNearby() && SpiritWork != SpiritWorkState.UsingTurret)
            {
                if (!waitUntilEnemiesLeave)
                {
                    waitUntilEnemiesLeave = true;
                    CurrentState.ToEscape();
                }
            }
        }
        */

        if (home)
        {
            homeless = false;
        }
        else
        {
            atHome = false;
            homeless = true;
        }
    }

    private void SetSpiritID()
    {
        spiritID = AIManager.Instance.spirits.Count;
    }

    public void Death()
    {
        if (Warrior)
        {
            Barracks guardHouse = workPlace as Barracks;
            guardHouse.Warriors.Remove(this);
        }
        if (workPlace)
        {
            AIManager.Instance.RemoveSpiritFromWork(workPlace, this);
            //workPlace.Spirits.Remove(this);
            //workPlace.CurrentSpirits--;
            //workPlace.GiveBackPlace(placeToStay);
        }
        if (home)
        {
            home.Spirits.Remove(this);
            home.CurrentSpirits--;
        }

        AIManager.Instance.RemoveSpirit(this);
        OnAnimationStateChange -= ChangeAnimation;
        Destroy(gameObject);
    }

    private bool IsEnemyNearby()
    {
        if (EnemyManager.Instance.enemies.Count > 0)
        {
            foreach (Enemy enemy in EnemyManager.Instance.enemies)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < distanceToTriggerEscape)
                {
                    if (!nearestEnemy)
                    {
                        nearestEnemy = enemy;
                    }
                    if (Vector3.Distance(nearestEnemy.transform.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position))
                    {
                        nearestEnemy = enemy;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public void GetDamage(float damage)
    {
        if (IsDead) return;
        hitPoints -= damage;
        if (hitPoints <= 0f)
        {
            SpiritAnimation = SpiritAnimationState.Dead;
            Death();
        }
    }

    public void UpdateStatistics()
    {
        if (!Warrior)
        {
            hitPoints = BalancePanel.Instance.Spirit.hitPoints;
            maxHitPoints = BalancePanel.Instance.Spirit.hitPoints;
            agent.speed = BalancePanel.Instance.Spirit.movementSpeed;
            AttackDamage = BalancePanel.Instance.Spirit.attackDamage;
        }
        else
        {
            hitPoints = BalancePanel.Instance.Warrior.hitPoints;
            maxHitPoints = BalancePanel.Instance.Warrior.hitPoints;
            agent.speed = BalancePanel.Instance.Warrior.movementSpeed;
            AttackDamage = BalancePanel.Instance.Warrior.attackDamage;
        }
    }

    private IEnumerator CheckingIfFewFirstSecondsPassed()
    {
        yield return new WaitForSeconds(3.0f);
        fewFirstSecondsPassed = true;
        yield return null;
    }

    public void TakeResource(bool take)
    {
        resource.SetActive(take);
    }

    private void SetAnimationState(SpiritAnimationState state)
    {
        if (spiritAnimation != state)
        {
            spiritAnimation = state;
            OnAnimationStateChange?.Invoke();
        }
    }
    
    private void ChangeModelOnWarrior(bool warrior)
    {
        if (warriorModel.activeInHierarchy)
            return;
        spiritModel.SetActive(!warrior);
        warriorModel.SetActive(warrior);
        if(warrior)
        {
            Animator = warriorModel.GetComponent<Animator>();
            Animator.SetTrigger("startAnimation");
        }
        if(warrior)
        {
            workPlace.GetComponent<Barracks>()?.PlayTurningIntoWarriorSound();
        }
    }

    public virtual void ChangeAnimation()
    {
        Animator.speed = spiritAnimation == SpiritAnimationState.Walking || spiritAnimation == SpiritAnimationState.CarringResource ? 1.6f : 1f;//temporary solution - thinking of better one 
        Animator.SetInteger("ChangeAnimation", (int)spiritAnimation);
        foreach(var _sound in continuousSounds)
        {
            _sound.Stop();
        }
        var index = (int)spiritAnimation;
        AudioSource sound;
        if(index <= 4)
        {
            sound = continuousSounds[index];
            // Checks if it's carrying resource animation.
            if(index == 4)
            {
                // Checks if collecting sticks sound is assigned and if is, then plays it in the beggining of carrying resource animation.
                if(disposableSounds[2].clip != null)
                {
                    disposableSounds[2].Play();
                }
            }
        }
        else
        {
            sound = disposableSounds[index - 5];
        }
        if(sound.clip != null)
        {
            sound.Play();
        }
    }
}
