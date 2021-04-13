using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a class, where we can modify the gameplay's balance to make the player's experience better.
public class BalancePanel : MonoBehaviour
{
    public static BalancePanel Instance { get; private set; }

    private int stage3variant;

    #region Structs

    [System.Serializable]
    public struct ResourcesData
    {
        [SerializeField]
        public float initialValue, initialMaxValue, incomePerSpirit, capacityGainPerStorage;
        // [HideInInspector] //TODO: Commented this for debug purpouses - fix the balance panel reset issues
        public float timerCooldown; //FIXME: This resets to 0 after stage transition.
    }

    [System.Serializable]
    public struct SpiritData
    {
        [SerializeField]
        public float hitPoints, attackDamage, movementSpeed;
    }

    [System.Serializable]
    public struct BuildingData
    {
        [SerializeField]
        public float hitPoints, constructionTime;
        [SerializeField]
        public int maxAssignableSpirits;
        public string buildingDescription;
        public Sprite buildingImage;
        [SerializeField]
        public ResourcePack costInResources;
    }

    [System.Serializable]
    public struct TurretData
    {
        [SerializeField]
        public float dmgPerHit, range;
    }

    [System.Serializable]
    public struct LRTurretData
    {
        [SerializeField]
        public TurretData turretParams;
        [SerializeField]
        public float areaOfEffect;
    }

    [System.Serializable]
    public struct TrapData
    {
        [SerializeField]
        public string name, description;
        [SerializeField]
        public float timeToCraft, valueOfEffect;
        [SerializeField]
        public ResourcePack cost;
    }

    [System.Serializable]
    public struct ResearchSet
    {
        [SerializeField]
        public ResearchData research1, research2, research3, research4, research5, research6;
    }

    [System.Serializable]
    public struct Stage3Data
    {
        [SerializeField]
        public string variantDescription;

        [SerializeField]
        public ResourcesData lifeEnergy;
        [SerializeField]
        public ResourcesData wood, thirdResource;

        [Header("Spirits")]
        [SerializeField]
        public SpiritData spirit;
        [SerializeField]
        public SpiritData warrior, enemyWarrior;

        [Header("Buildings")]
        [SerializeField]
        public BuildingData altar;
        [SerializeField]
        public BuildingData shelter, weaversHut, garden, storage, barracks, turret, longRangeTurret, workshop, laboratory, generator;

        [Header("Turrets")]
        [SerializeField]
        public TurretData turretStats;
        [SerializeField]
        public LRTurretData longRangeTurretStats;

        [Header("Traps")]
        [SerializeField]
        public TrapData explosingTrap;
        [SerializeField]
        public TrapData stunningTrap;

        [Header("Laboratory research")]
        [SerializeField]
        public ResearchSet researchStats; // We need to change this as soon as we have an idea for research.
    }

    #endregion

    #region Inspector

    [Header("Stage 2")]
    [SerializeField]
    private ResourcesData lifeEnergy;
    [SerializeField]
    private ResourcesData wood, thirdResource;
    [Header("Spirit")]
    public int startSpiritCount;
    [SerializeField]
    private SpiritData spirit;
    [HideInInspector]
    private SpiritData warrior, enemyWarrior;
    [Header("Buildings")]
    [SerializeField]
    private BuildingData altar;
    [SerializeField]
    private BuildingData shelter, weaversHut, garden, storage;
    [HideInInspector]
    private BuildingData barracks, turret, longRangeTurret, workshop, laboratory, generator;
    [Header("Turrets")]
    [HideInInspector]
    private TurretData turretStats;
    [HideInInspector]
    private LRTurretData longRangeTurretStats;
    [Header("Traps")]
    [HideInInspector]
    private TrapData explosingTrap;
    [HideInInspector]
    private TrapData stunningTrap;
    [Header("Laboratory research")]
    [HideInInspector]
    private ResearchSet researchStats; // We need to change this as soon as we have an idea for research.

    [Space(50)]
    [SerializeField]
    private Stage3Data stage3variant1;

    [SerializeField]
    private Stage3Data stage3variant2;

    #endregion

    #region Properties

    public ResourcesData LifeEnergy { get { return lifeEnergy; } private set { lifeEnergy = value; } }
    public ResourcesData Wood { get { return wood; } private set { wood = value; } }
    public ResourcesData ThirdResource { get { return thirdResource; } private set { thirdResource = value; } }
    public SpiritData Spirit { get { return spirit; } private set { spirit = value; } }
    public SpiritData Warrior { get { return warrior; } private set { warrior = value; } }
    public SpiritData EnemyWarrior { get { return enemyWarrior; } private set { enemyWarrior = value; } }
    public BuildingData Altar { get { return altar; } private set { altar = value; } }
    public BuildingData Shelter { get { return shelter; } private set { shelter = value; } }
    public BuildingData WeaversHut { get { return weaversHut; } private set { weaversHut = value; } }
    public BuildingData Garden { get { return garden; } private set { garden = value; } }
    public BuildingData Barracks { get { return barracks; } private set { barracks = value; } }
    public BuildingData Turret { get { return turret; } private set { turret = value; } }
    public BuildingData LongRangeTurret { get { return longRangeTurret; } private set { longRangeTurret = value; } }
    public BuildingData Workshop { get { return workshop; } private set { workshop = value; } }
    public BuildingData Storage { get { return storage; } private set { storage = value; } }
    public BuildingData Laboratory { get { return laboratory; } private set { laboratory = value; } }
    public BuildingData Generator { get { return generator; } private set { generator = value; } }
    public TurretData TurretStats { get { return turretStats; } private set { turretStats = value; } }
    public LRTurretData LongRangeTurretStats { get { return longRangeTurretStats; } private set { longRangeTurretStats = value; } }
    public TrapData ExplosingTrap { get { return explosingTrap; } private set { explosingTrap = value; } }
    public TrapData StunningTrap { get { return stunningTrap; } private set { stunningTrap = value; } }
    public ResearchSet ResearchStats { get { return researchStats; } private set { researchStats = value; } }
    #endregion


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void BalanceUpdate(Building arg)
    {
        BuildingData neededSet;
        // Assigning some values to get rid of compilation error.
        //neededSet.hitPoints = 1.0f;
        //neededSet.constructionTime = 0.1f;
        //neededSet.maxAssignableSpirits = 0;
        //neededSet.costInResources.lifeEnergy = 0.0f;
        //neededSet.costInResources.wood = 0.0f;
        //neededSet.costInResources.thirdResource = 0.0f;
        //neededSet.buildingDescription = "";

        if (arg is Altar) neededSet = altar;
        else if (arg is Shelter) neededSet = shelter;
        else if (arg is WeaversHut) neededSet = weaversHut;
        else if (arg is Garden) neededSet = garden;
        else if (arg is Barracks) neededSet = barracks;
        else if (arg is Turret) neededSet = turret;
        else if (arg is LongRangeTurret) neededSet = longRangeTurret;
        else if (arg is Workshop) neededSet = workshop;
        else if (arg is Storage) neededSet = storage;
        else if (arg is Laboratory) neededSet = laboratory;
        else /*if (arg is Generator)*/ neededSet = generator; //no more building types and will not need values assigning

        arg.MaxHitPoints = neededSet.hitPoints;
        arg.CurrHitPoints = neededSet.hitPoints; //to have full health after updating
        arg.TimeNeededForConstruction = neededSet.constructionTime;
        arg.MaxSpirits = neededSet.maxAssignableSpirits;
        arg.BuildingDescription = neededSet.buildingDescription;
        arg.BuildingCost.lifeEnergy = neededSet.costInResources.lifeEnergy;
        arg.BuildingCost.wood = neededSet.costInResources.wood;
        arg.BuildingCost.thirdResource = neededSet.costInResources.thirdResource;
        arg.BuildingImage = neededSet.buildingImage;
    }
    public void BalanceUpdate(out ResourcePack arg) // For StorageManager updates.
    {
        arg.lifeEnergy = lifeEnergy.capacityGainPerStorage;
        arg.wood = wood.capacityGainPerStorage;
        arg.thirdResource = thirdResource.capacityGainPerStorage;
    }

    public void BalanceUpdate(Trap arg)
    {
        Debug.Log("Updated traps statistics");
        TrapData set = new TrapData();
        bool noError = true;
        switch (arg.trapType)
        {
            case TrapTypeEnum.explosing:
                set = ExplosingTrap;
                break;
            case TrapTypeEnum.stunning:
                set = StunningTrap;
                break;
            default:
                noError = false;
                break;
        }
        if (noError)
        {
            arg.name = set.name;
            arg.description = set.description;
            arg.timeToCraft = set.timeToCraft;
            arg.ValueOfEffect = set.valueOfEffect;
            arg.cost = set.cost;
        }
    }

    public void GoToStage3(int variant)
    {
        stage3variant = variant;
        if (stage3variant == 1)
        {
            InternalUpdateOfDataToStage3(stage3variant1);
        }
        else
        {
            InternalUpdateOfDataToStage3(stage3variant2);
        }
    }

    public void GainBenefitsFromResearch(int index)
    {
        // It is divided to 2 switches, because we can possibly make different research available for different variants of stage 3.
        if (stage3variant == 1)
        {
            switch (index)
            {
                case 1:
                    lifeEnergy.incomePerSpirit += researchStats.research1.value;
                    break;
                case 2:
                    Building.ConstructionSpeedModifier += researchStats.research2.value / 100.0f;
                    break;
                case 3:
                    warrior.attackDamage += researchStats.research3.value / 100.0f;
                    break;
                case 4:
                    warrior.hitPoints += researchStats.research4.value / 100.0f;
                    break;
                case 5:
                    explosingTrap.valueOfEffect += researchStats.research5.value / 100.0f;
                    break;
                case 6:
                    stunningTrap.valueOfEffect += researchStats.research6.value / 100.0f;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (index)
            {
                case 1:
                    lifeEnergy.incomePerSpirit += researchStats.research1.value;
                    break;
                case 2:
                    Building.ConstructionSpeedModifier += researchStats.research2.value / 100.0f;
                    break;
                case 3:
                    warrior.attackDamage += researchStats.research3.value / 100.0f;
                    break;
                case 4:
                    warrior.hitPoints += researchStats.research4.value / 100.0f;
                    break;
                case 5:
                    explosingTrap.valueOfEffect += researchStats.research5.value / 100.0f;
                    break;
                case 6:
                    stunningTrap.valueOfEffect += researchStats.research6.value / 100.0f;
                    break;
                default:
                    break;
            }
        }
    }


    private void InternalUpdateOfDataToStage3(Stage3Data set)
    {
        lifeEnergy = set.lifeEnergy;
        wood = set.wood;
        thirdResource = set.thirdResource;
        spirit = set.spirit;
        warrior = set.warrior;
        enemyWarrior = set.enemyWarrior;
        altar = set.altar;
        shelter = set.shelter;
        weaversHut = set.weaversHut;
        garden = set.garden;
        barracks = set.barracks;
        turret = set.turret;
        longRangeTurret = set.longRangeTurret;
        workshop = set.workshop;
        storage = set.storage;
        laboratory = set.laboratory;
        generator = set.generator;
        turretStats = set.turretStats;
        longRangeTurretStats = set.longRangeTurretStats;
        explosingTrap = set.explosingTrap;
        stunningTrap = set.stunningTrap;
        researchStats = set.researchStats;
    }
}



[System.Serializable]
public struct ResourcePack
{
    public float lifeEnergy, wood, thirdResource;

    public ResourcePack(float lifeEnergy, float wood, float thirdResource)
    {
        this.lifeEnergy = lifeEnergy;
        this.wood = wood;
        this.thirdResource = thirdResource;
    }
}

[System.Serializable]
public struct ResearchData
{
    [SerializeField]
    public string name, description;
    [SerializeField]
    public float value, timeNeededForCompletion;
}