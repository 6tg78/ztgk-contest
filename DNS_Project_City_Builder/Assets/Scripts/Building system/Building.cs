using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

// The base class for all buildings.
public abstract class Building : MonoBehaviour
{
    public string BuildingName { get; protected set; }
    [HideInInspector]
    public ResourcePack BuildingCost;
    public List<Spirit> Spirits => spirits;
    public List<Transform> PlacesToStay => placesToStay;
    public WorkTypeEnum WorkType { get; protected set; } // Determines the type of work that spirits do in this building.
    public GameObject Obstacles { get { return obstacles; } set { obstacles = value; } }
    public GameObject SelectionRing { get { return selectionRing; } private set { selectionRing = value; } }
    public int BuildingID { get; private set; }
    public int MaxSpirits { get; set; } // TODO: Set this value based on state
    //public int currentSpirits;
    public int CurrentSpirits
    {
        get;// { return currentSpirits; }
        set;// { currentSpirits = value; OnCurrentSpiritsChanged?.Invoke(); }
    }
    public int WorkersAmount { get; set; }
    public int SpiritsConstructingMe { get; set; }
    public string BuildingDescription { get; set; }
    public Sprite BuildingImage { get; set; }
    private float _maxHitPoints;
    public float MaxHitPoints
    {
        get { return _maxHitPoints; }
        set { _maxHitPoints = value; OnHealthChanged?.Invoke(); }
    }
    [SerializeField]
    private float _currentHitPoints;
    public float CurrHitPoints
    {
        get { return _currentHitPoints; }
        set { _currentHitPoints = value; OnHealthChanged?.Invoke(); }
    }
    public event Action OnHealthChanged;
    public float TimeNeededForConstruction { get; set; }

    private float _degreeOfConstructionCompletion;
    public float DegreeOfConstructionCompletion
    {
        get { return _degreeOfConstructionCompletion; }
        private set { _degreeOfConstructionCompletion = value; OnConstructionProgress?.Invoke(); }
    }
    public event Action OnConstructionProgress;
    public static float ConstructionSpeedModifier { get; set; }
    // public bool InConstructionMode { get; set; }
    public bool InConstructionMode { get { return _buildingState == BuildingState.Construction; } }
    // public bool InConstrucionPlanningMode { get; set; }
    public bool InConstrucionPlanningMode { get { return _buildingState == BuildingState.Planning; } }
    // public bool FinishedConstructing { get; private set; }
    public bool FinishedConstructing { get { return _buildingState == BuildingState.Built; } }
    public bool IsBeingDestroyed { get; private set; } // TODO: Figure out if we need state for this
    public bool IsRepairing { get { return _buildingState == BuildingState.Repairing; } }
    public bool IsDamaged { get { return _buildingState == BuildingState.Damaged; } }
    public bool IsOperating { get { return _buildingState == BuildingState.Built || _buildingState == BuildingState.Damaged || _buildingState == BuildingState.Repairing; } }

    [SerializeField]
    private Transform parentOfPlacesToStay;
    [SerializeField]
    protected GameObject stage2model, stage3variant1model, stage3variant2model, models, obstacles, selectionRing;
    private int collisionCount = 0;
    [SerializeField]
    private BuildingState _buildingState;
    public BuildingState BuildingState { get { return _buildingState; } set { SetState(value); } }
    public event Action<BuildingState> OnStateChanged;
    private NavMeshObstacle obstacle;
    private List<Spirit> spirits = new List<Spirit>();
    private List<Transform> placesToStay = new List<Transform>();
    private Material[] materials;
    private AudioSource onClickSound;

    public event Action OnCurrentSpiritsChanged;


    #region MonoBehaviour

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Start()
    {
        BuildingID = GameManager.Instance.buildings.Count;
        BalancePanel.Instance.BalanceUpdate(this);
        CurrHitPoints = MaxHitPoints;

        foreach (Transform child in parentOfPlacesToStay)
        {
            placesToStay.Add(child);
        }
    }

    protected virtual void Update()
    {
        UpdateActions();
        if (Input.GetButtonDown("Fire1"))
        {
            // If clicked UI - do nothing
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (this == SelectionController.SelectedBuilding && this.IsOperating)
            {
                selectionRing.SetActive(true);
                if (onClickSound.clip != null && !onClickSound.isPlaying)
                {
                    onClickSound.Play();
                }
                else if (onClickSound.clip == null)
                {
                    Debug.Log(BuildingName + " doesn't have on click sound assigned.");
                }
            }
            else
            {
                selectionRing.SetActive(false);
            }
        }
    }

    void OnTriggerEnter()
    {
        collisionCount++;
    }

    void OnTriggerExit()
    {
        collisionCount--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (Transform child in parentOfPlacesToStay)
        {
            Gizmos.DrawCube(child.position, new Vector3(0.3f, 0.3f, 0.3f));
        }
    }

    #endregion


    // [UI] Method called by enemy spirits.
    public void GetDamage(float damage)
    {
        if (BuildingState != BuildingState.Construction)
        {
            if (damage < CurrHitPoints)
            {
                CurrHitPoints -= damage;
            }
            else
            {
                CurrHitPoints = 0.0f;
            }

            if (CurrHitPoints < MaxHitPoints)
            {
                if (BuildingState != BuildingState.Damaged && BuildingState != BuildingState.Repairing)
                {
                    BuildingState = BuildingState.Damaged;//need testing
                                                          //Change material for damaged
                }
            }

            if (CurrHitPoints <= 0.0f && !IsBeingDestroyed)
            {
                IsBeingDestroyed = true;
                Destroyed();
            }
        }
        else
        {
            DegreeOfConstructionCompletion -= damage;
            
            if (DegreeOfConstructionCompletion < 0f && !IsBeingDestroyed)
            {
                IsBeingDestroyed = true;
                Destroyed();
            }
        }
    }


    public virtual void ChangeModel(int variant)
    {
        if (variant == 0)
        {
            stage2model.SetActive(true);
            stage3variant1model.SetActive(false);
            stage3variant2model.SetActive(false);
        }
        else if (variant == 1)
        {
            stage2model.SetActive(false);
            stage3variant1model.SetActive(true);
            stage3variant2model.SetActive(false);
        }
        else
        {
            stage2model.SetActive(false);
            stage3variant1model.SetActive(false);
            stage3variant2model.SetActive(true);
        }
    }

    public void ChangeMaterial(int arg)
    {
        var renderers = models.GetComponentsInChildren<MeshRenderer>(true);
        if (arg != 0)
        {
            string path = "ShadedMaterials/";
            switch (arg)
            {
                case 1:
                    path += "HologramBlue";
                    break;
                case 2:
                    path += "HologramRed";
                    break;
                case 3:
                    path += "BeingBuilt";
                    break;
            }
            var newMaterial = Resources.Load(path, typeof(Material)) as Material;
            foreach (var renderer in renderers)
            {
                renderer.material = newMaterial;
            }
        }
        else
        {
            int index = 0;
            foreach (var renderer in renderers)
            {
                renderer.material = materials[index++];
            }
        }
    }

    public Transform TakePlace(int index)
    {
        Transform m_place = PlacesToStay[index];
        PlacesToStay.Remove(m_place);
        return m_place;
    }

    public void GiveBackPlace(Transform place)
    {
        PlacesToStay.Add(place);
    }

    public virtual void Destroyed()
    {
        BuildingState = BuildingState.Destroyed;
        GameManager.Instance.buildings.Remove(this);

        if (this is WeaversHut) GameManager.Instance.weaversHuts.Remove((WeaversHut)this);
        else if (this is Altar) GameManager.Instance.altars.Remove((Altar)this);
        else if (this is Shelter)
        {
            GameManager.Instance.houses.Remove((Shelter)this);

            foreach (Spirit spirit in Spirits)
            {
                spirit.gameObject.SetActive(true);
                spirit.homeless = true;
                spirit.atHome = false;
                spirit.home = null;
            }
        }
        if (InConstructionMode) GameManager.Instance.constructs.Remove(this);

        if (!(this is Shelter))
            for (int i = Spirits.Count - 1; i >= 0; i--)
            {
                //Spirits[i].workPlace = null;
                //spirit.Working = false;
                //Spirits[i].SpiritWork = SpiritWorkState.Idle;
                AIManager.Instance.RemoveSpiritFromWork(this);//test
            }
        // Dissolve shader?
        Destroy(gameObject, GameManager.Instance.BuildingDestroyTime);
    }


    protected virtual void UpdateActions()
    {
        if (this is Barracks)
        {
            MaxSpirits = 6;
        }
        // Before the building was placed.
        if (InConstrucionPlanningMode)
        {
            if (collisionCount == 0)
            {
                BuildingsManager.Instance.PlacementPossible = true;
                ChangeMaterial(1);
            }
            else
            {
                BuildingsManager.Instance.PlacementPossible = false;
                ChangeMaterial(2);
            }
        }

        // After the building was placed and before it finished constructing.
        if (InConstructionMode && SpiritsConstructingMe > 0)
        {
            float step = SpiritsConstructingMe * Time.deltaTime * 100.0f * ConstructionSpeedModifier / TimeNeededForConstruction;
            if (DegreeOfConstructionCompletion + step >= 100.0f)
            {
                DegreeOfConstructionCompletion = 100.0f;
            }
            else
            {
                DegreeOfConstructionCompletion += step;
            }

            // Here we should implement changing of material's shader commensurately to degree of construcion completion.

            if (Mathf.Approximately(DegreeOfConstructionCompletion, 100.0f))
            {
                foreach (Spirit spirit in Spirits)
                {
                    AIManager.Instance.workingSpirits.Remove(spirit);
                    AIManager.Instance.freeSpirits.Add(spirit);
                }
                if (this is Shelter || this is Barracks)
                {
                    Destroy(obstacle);
                }
                Spirits.Clear();
                CurrentSpirits = 0;
                SetWorkType();
                ChangeMaterial(0);
                OnFinishedConstruction();
                BuildingState = BuildingState.Built;
            }
        }

        if (IsRepairing && SpiritsConstructingMe > 0)
        {
            float step = SpiritsConstructingMe * ConstructionSpeedModifier * Time.deltaTime * 100f / TimeNeededForConstruction;
            CurrHitPoints += step;

            if (CurrHitPoints >= MaxHitPoints)
            {
                BuildingState = BuildingState.Built;
                foreach (Spirit spirit in Spirits)
                {
                    if (spirit.SpiritWork == SpiritWorkState.Constructing)
                    {
                        AIManager.Instance.workingSpirits.Remove(spirit);
                        AIManager.Instance.freeSpirits.Add(spirit);
                        CurrentSpirits--;
                    }
                }
                SetWorkType();
                SpiritsConstructingMe = 0;
                //change material for normal (Built)
            }
        }
    }

    protected virtual void SetWorkType()
    {
        WorkType = WorkTypeEnum.NonWorkable;
    }

    protected virtual void OnFinishedConstruction()
    {
        SpiritsConstructingMe = 0;//test
    }

    public void SetState(BuildingState state)
    {
        _buildingState = state;
        OnStateChanged?.Invoke(state);
    }

    private void Initialize()
    {
        OnCurrentSpiritsChanged += CountWorkers;
        selectionRing.SetActive(false);
        BuildingState = BuildingState.Planning; // TODO: Might need 'null' state for this
        DegreeOfConstructionCompletion = 0.0f;
        ConstructionSpeedModifier = 1.0f;
        SpiritsConstructingMe = 0;
        IsBeingDestroyed = false;
        WorkType = WorkTypeEnum.Constructing;
        CurrentSpirits = 0;
        obstacle = GetComponent<NavMeshObstacle>();
        var renderers = models.GetComponentsInChildren<MeshRenderer>(true);
        var materialsList = new List<Material>();
        foreach (var renderer in renderers)
        {
            materialsList.Add(renderer.material);
        }
        materials = materialsList.ToArray();
        onClickSound = gameObject.GetComponent<AudioSource>();
    }
    public void Repair()
    {
        if (!IsDamaged || AIManager.Instance.freeSpirits.Count == 0 || !AIManager.Instance.CanAssignSpiritToWork(this)) return;
        //Change material for repairing

        BuildingState = BuildingState.Repairing;
        WorkType = WorkTypeEnum.Repairing;

        foreach (Transform place in PlacesToStay)
        {
            AIManager.Instance.AddSpiritToWork(this);
        }

        SetWorkType();// to allow assigning spirits to normal work fe. collecting resources


    }

    private void OnDestroy()
    {
        OnCurrentSpiritsChanged -= CountWorkers;
    }
    public void CountWorkers()
    {
        int amount = 0;
        foreach (Spirit spirit in Spirits)
        {
            if (spirit.CollectingWork || (spirit.PrayingWork && spirit.IsPraying) || (spirit.ResearchingWork && !spirit.IsWalking))
                amount++;
        }
        WorkersAmount = amount;
    }

    public void DisableSelectionRing()
    {
        SelectionRing.SetActive(false);
    }
}




// You can move it to whatever file you want. Should work from anywhere. Just remember to put it BEHIND the class.
public enum WorkTypeEnum
{
    Constructing = 0,
    Collecting = 1,
    Praying = 2,
    Guarding = 3,
    Repairing = 4,
    Shooting = 5,
    Researching = 6,
    BeingInHome = 80,
    NonWorkable = 1000
}