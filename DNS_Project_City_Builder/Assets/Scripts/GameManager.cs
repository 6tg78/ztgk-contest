using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Main manager of the whole game (except for gameplay balance).
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region Properties
    public GameObject SunLight { get { return sunLight; } private set { sunLight = value; } }
    public float DayLength { get { return dayLength; } private set { dayLength = value; } }
    public float LifeEnergySubtractionPeriod { get { return lifeEnergySubtractionPeriod; } private set { lifeEnergySubtractionPeriod = value; } }
    public float BuildingDestroyTime { get { return buildingDestroyTime; } private set { buildingDestroyTime = value; } }
    public float TimeToNextWave { get { return timeToNextWave; } private set { timeToNextWave = value; } }
    public float TimeTheNextWaveBegins { get; private set; }
    public int WavesSurvived { get; private set; }
    public int SelectedStage3Variant { get; set; }
    public int SpiritNeededToStage3 { get; private set; }
    public bool VictoryAchieved { get { return victoryAchieved; } private set { victoryAchieved = value; } }

    #endregion

    #region Inspector

    [SerializeField]
    private GameObject sunLight;
    [SerializeField]
    private float dayLength;
    [SerializeField]
    private float lifeEnergySubtractionPeriod;
    [SerializeField]
    private float buildingDestroyTime;
    [SerializeField]
    private float timeToNextWave;

    [SerializeField]
    private int startStage;

    [SerializeField]
    private int spiritsNeededToStage3;

    #endregion

    #region BuildingsLists
    [HideInInspector]
    public List<Building> buildings = new List<Building>();
    [HideInInspector]
    public List<Building> constructs = new List<Building>();
    [HideInInspector]
    public List<Shelter> houses = new List<Shelter>();
    [HideInInspector]
    public List<Altar> altars = new List<Altar>();
    [HideInInspector]
    public List<WeaversHut> weaversHuts = new List<WeaversHut>();
    [HideInInspector]
    public List<Storage> storages = new List<Storage>();
    #endregion
    [SerializeField]
    private int _currentStage;
    public int CurrentStage
    {
        get { return _currentStage; }
        private set { _currentStage = value; OnStageChanged?.Invoke(); }
    }
    private bool isDuringWave;

    private bool isPlaying;

    private bool victoryAchieved = false;

    public event Action OnStageChanged;

    public AudioSource shortNotificationSound;

    [SerializeField]
    private AudioSource enemiesSpawnedSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Initialize();
        DontDestroyOnLoad(gameObject); //maybe naking GameManager statis is better - static classes are not destroyed
    }

    void Start()
    {
        isPlaying = true;
        StartCoroutine(Timer(LifeEnergySubtractionPeriod)); // Timer - subtracting life energy used by spirits
        //this.Timer(0.5f, delegate { if (AIManager.Instance.spirits.Count >= SpiritNeededToStage3) StageProgress(); }, () => { return currStage == 2; });
        AddStartTutorial();
        StartCoroutine(VictoryOrDefeatChecker());
    }

    void Update()
    {
        UpdateBuildings();
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            SelectedStage3Variant = 1;
            StageProgress();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ResourceManagement.Instance.AddResources<LifeEnergyResource>(99995);
            ResourceManagement.Instance.AddResources<WoodResource>(99995);
            ResourceManagement.Instance.AddResources<ThirdResource>(99995);
        }
        
        if (!isDuringWave && CurrentStage == 3) ToNextWave();
    }


    private void Initialize()
    {
        CurrentStage = startStage;
        isDuringWave = false;
        TimeTheNextWaveBegins = TimeToNextWave;
        SpiritNeededToStage3 = spiritsNeededToStage3;
        WavesSurvived = 0;
    }


    private IEnumerator Timer(float time)
    {

        while (isPlaying)//to keep this timer alive
        {
            float amount = 0.0f;

            //counting how much energy our spirits use 
            foreach (Spirit spirit in AIManager.Instance.spirits)
            {
                amount += spirit.GetComponent<Spirit>().lifeEnergyConsumingPerDay;
            }

            amount /= (DayLength / time); //to made it constantly counted how much energu spirits use in period of time when they need a value per day, 
                                          //expected that some spirits can use more energu to live

            yield return new WaitForSeconds(time);
            if (!UIManager.Instance.GamePaused) ResourceManagement.Instance.UseResource<LifeEnergyResource>(amount);
        }

    }

    private void ToNextWave()
    {
        if (TimeTheNextWaveBegins <= 0)
        {
            EnemyManager.Instance.EnemyRespawn();
            isDuringWave = true;
            TimeTheNextWaveBegins = TimeToNextWave;
            this.Timer(0.5f, () =>
                             {
                                 isDuringWave = EnemyManager.Instance.enemies.Count != 0;
                                 if (!isDuringWave)
                                 {
                                     AIManager.Instance.EnemiesWaveIsOver();
                                     WavesSurvived++;
                                     if (WavesSurvived == 4)
                                     {
                                         NotificationManager.Instance.AddNotification("Almost...", "You're nearly there! Defend The Forest this one last time and let The Bloom unleash itself!", false);
                                     }
                                 }
                             },
                             () => { return isDuringWave; });
            UIManager.Instance.NextWaveTimer.gameObject.SetActive(false);
            if(enemiesSpawnedSound != null)
            {
                enemiesSpawnedSound.Play();
            }
            else
            {
                Debug.Log("EnemiesSpawnedSound in GameManager isn't assigned.");
            }
        }
        else
        {
            UIManager.Instance.NextWaveTimer.gameObject.SetActive(true);
            TimeTheNextWaveBegins -= Time.deltaTime;
            UIManager.Instance.NextWaveTimer.Time = new TimeSpan(0, 0, (int)TimeTheNextWaveBegins).ToString("mm':'ss");
        }
    }

    private void UpdateBuildings()
    {
        buildings = FindObjectsOfType<Building>().ToList();
        houses = FindObjectsOfType<Shelter>().ToList();
        altars = FindObjectsOfType<Altar>().ToList();
        weaversHuts = FindObjectsOfType<WeaversHut>().ToList();
        storages = FindObjectsOfType<Storage>().ToList();


        foreach (Building building in buildings)
        {
            bool outOfLoop = false;
            if (building.InConstructionMode)
            {
                foreach (Building construct in constructs)
                {
                    if (construct.BuildingID == building.BuildingID) outOfLoop = true;
                }
                if (outOfLoop) continue;
                else constructs.Add(building);
            }
            else
            {
                foreach (Building construct in constructs)
                {
                    if (construct.BuildingID == building.BuildingID) outOfLoop = true;
                }
                if (!outOfLoop) continue;
                else constructs.Remove(building);
            }
        }
    }
    public void StageProgress() // This is a method called when we make it to the next stage and after we select its variant.
    {
        BalancePanel.Instance.GoToStage3(SelectedStage3Variant); // Choosing new balance data set (from Stage 3).
        StorageManager.Instance.StageProgress(); // Updating resources limits and capacityGainPerStorage. 
        ResourceManagement.Instance.StageProgress(); // Updating incomePerSpirit.
        foreach (var building in buildings)
        {
            UIManager.Instance.PrepareBuildingForStage3(building);
        }
        CurrentStage++;
        // FindObjectOfType<GenerateBuildingPanel>().StageProgress();

        foreach (Shelter shelter in houses)
        {
            shelter.UpdateSpiritsMaxAmount();
        }

        //****************************************************************
        //TODO: We still have to implement updates of spirits, turrets, traps and research stats.
        //* ****************************************************************

        //spirits
        foreach(Spirit spirit in AIManager.Instance.spirits)
        {
            spirit.UpdateStatistics();
        }
    }

    private IEnumerator VictoryOrDefeatChecker()
    {
        yield return new WaitForSeconds(5.0f); // Making sure that our first altar will spawn before we check the defeat condition.
        while (WavesSurvived < ObjectivePanel.targetGoal && altars.Count() >= 1)
        {
            yield return new WaitForSeconds(1.0f);
        }
        if (WavesSurvived == ObjectivePanel.targetGoal)
        {
            victoryAchieved = true;
        }
        SceneChanger.Instance.AllowToChangeScene();
    }

    private void AddStartTutorial()
    {
        // STORY
        NotificationManager.Instance.AddNotification("First steps", "Welcome to The Forest! These little creatures standing in the meadow are its Spirits. You should provide them with some housing. No one likes to sleep on the ground...", false);

        // BASIC CONTROLS
        NotificationManager.Instance.AddNotification("Camera", "To move the camera you can use your mouse or \"WSAD\" keys. You can rotate it using \"Q\" and \"E\" and you can use your mouse's scroll to zoom it.");

        // UI
        NotificationManager.Instance.AddNotification("Objective", "Your current objective is always displayed in the top right corner of the screen, just below the time control buttons. You have to complete it to continue the game.");
        NotificationManager.Instance.AddNotification("Time control", "Just above the objective panel are the time control buttons and the menu button. They let you pause your game, speed it up and open the menu, which lets you surrender or exit the game. You can also use \"Space\" button to pause or unpause the game.");
        NotificationManager.Instance.AddNotification("Population", "Right to the left of those buttons you can find information about your population. It indicates its current size and maximum size. The value in parentheses indicates how many of your Spirits don't have any work to do. You should try to keep this number as low as possible.");

        // BUILDINGS UI
        NotificationManager.Instance.AddNotification("Buildings", "The building you require at the moment is called \"Shelter\". To build it, you can open construction panel by clicking the button on the left side of the screen.");
        NotificationManager.Instance.AddNotification("Cost", "Remember that you always need to have enough resources to build anything. All of your resources are displayed in the top part of the screen.");
        NotificationManager.Instance.AddNotification("Construction", "If you have enough resources, you can click one of the buttons in the construction panel to enter the construction mode and you can press \"Esc\" key or RMB to exit this mode. You can also manipulate your building by moving the mouse or use MLB to rotate it. When you are ready, just press LMB to place it (if you hold Left Shift you can place multiple buldings).");
        NotificationManager.Instance.AddNotification("Progress", "After you place your building you have to click it and add some Spirits to the construction (more Spirits equals less time needed to finish the construction). The building won't build itself.");

        // GAMEPLAY
        NotificationManager.Instance.AddNotification("Workers", "Some buildings require Spirits to be assigned to them and some don't. You should always check if you have your Spirits assigned to the building, if it seems like not doing anything at all.");
        NotificationManager.Instance.AddNotification("Purpose", "So, to put it all together, Spirits can construct your buildings and operate them. If all of your Spirits are busy and you need them to work somewhere else, try to unassign some of them from what they're doing.");
        NotificationManager.Instance.AddNotification("Altar", "The building which you already have built from the beggining is called \"Altar\". It generates valuable life energy, which is you main resource. You should keep generating it. Don't remember to assign your Spirits to it!");

        // CONCLUSION
        NotificationManager.Instance.AddNotification("Conclusion", "Now you know everything you need to start your adventure. If you're lost, just follow the objectives. Good luck!");
    }

    public void DisableSelectionRings()
    {
        foreach(var building in buildings)
        {
            building.DisableSelectionRing();
        }
    }
}
