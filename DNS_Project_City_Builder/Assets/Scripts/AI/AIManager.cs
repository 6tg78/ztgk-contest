using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }
    public float LifeEnergyNeededToAddSpirit { get; set; }
    private int _maxPossibleSpirits = 0;
    public int MaxPossbileSpirits
    {
        get { return _maxPossibleSpirits; }
        set { _maxPossibleSpirits = value; OnMaxSpiritsCountChanged?.Invoke(); }
    }
    public int SpiritsCount { get { return spirits.Count; } }
    public int FreeSpiritsCount { get { return freeSpirits.Count; } }
    public event Action OnSpiritsCountChanged;
    public event Action OnFreeSpiritsCountChanged;
    public event Action OnMaxSpiritsCountChanged;
    public event Action OnWorkerAmountChange;//when we add or remove spirit from work

    [SerializeField]
    private GameObject spirit;
    [SerializeField]
    private Transform respawnPoint;

    public List<Spirit> spirits = new List<Spirit>();
    public List<Spirit> freeSpirits = new List<Spirit>();
    public List<Spirit> workingSpirits = new List<Spirit>();
    public List<Spirit> deadSpirits = new List<Spirit>();

    void Awake()
    {
        // Checking if its only one instance at the same time
        if (Instance == null)
        {
            Instance = this;
        }
        LifeEnergyNeededToAddSpirit = 15f;
        MaxPossbileSpirits = 0;
    }

    private void Start()
    {
        for (int i = BalancePanel.Instance.startSpiritCount; i > 0; i--)
            AddSpirit(new Vector3(i * 4, 0, UnityEngine.Random.Range(-5, 5)), true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) AddSpirit();

        if (Input.GetKeyDown(KeyCode.O)) EnemiesWaveIsOver();
    }
    public void AddSpirit(Vector3 offset, bool startGame = false)
    {
        GameObject newSpirit = null;
        if (startGame)
        {
            newSpirit = Instantiate(spirit, respawnPoint);
            newSpirit.transform.position += offset;
        }
        else if (ResourceManagement.Instance.EnoughResource<LifeEnergyResource>(LifeEnergyNeededToAddSpirit) && spirits.Count < MaxPossbileSpirits)
        {
            ResourceManagement.Instance.UseResource<LifeEnergyResource>(LifeEnergyNeededToAddSpirit);
            newSpirit = Instantiate(spirit, respawnPoint);
            newSpirit.gameObject.transform.position += offset;
            LifeEnergyNeededToAddSpirit += 10f;//TODO: less hardcode => maybe move it to BalancePanel
        }
        else
        {
            Debug.Log("Not enough life energy or shelters");//TODO: move to UI
            //NotificationManager.Instance.AddNotification("Not enough shelters.",
            //                                             "Build more shelters to spawn more spirits.");
            return;
        }

        freeSpirits.Add(newSpirit.GetComponent<Spirit>());
        spirits.Add(newSpirit.GetComponent<Spirit>());

        OnSpiritsCountChanged?.Invoke();
    }

    public void AddSpirit(bool startGame = false)
    {
        GameObject newSpirit = null;
        if (startGame)
        {
            newSpirit = Instantiate(spirit, respawnPoint);
        }
        else if (ResourceManagement.Instance.EnoughResource<LifeEnergyResource>(LifeEnergyNeededToAddSpirit) && spirits.Count < MaxPossbileSpirits)
        {
            ResourceManagement.Instance.UseResource<LifeEnergyResource>(LifeEnergyNeededToAddSpirit);
            newSpirit = Instantiate(spirit, respawnPoint);
            LifeEnergyNeededToAddSpirit += 10f;//TODO: less hardcode => maybe move it to BalancePanel
        }
        else if (spirits.Count >= MaxPossbileSpirits)
        {
            ShortNotification.Instance.TriggerNotification("Build more shelters to spawn more spirits.");
            return;
        }
        else
        {
            ShortNotification.Instance.TriggerNotification("You need more Life Energy to spawn more spirits.");
            return;
        }

        freeSpirits.Add(newSpirit.GetComponent<Spirit>());
        spirits.Add(newSpirit.GetComponent<Spirit>());


        OnSpiritsCountChanged?.Invoke();
    }

    public void RemoveSpirit(Spirit spiritToRemove)
    {
        AIManager.Instance.spirits.Remove(spiritToRemove);

        foreach (Spirit spirit in AIManager.Instance.workingSpirits)
        {
            if (spirit.spiritID == spiritToRemove.spiritID)
            {
                AIManager.Instance.workingSpirits.Remove(spirit);
                break;
            }
        }

        foreach (Spirit spirit in AIManager.Instance.freeSpirits)
        {
            if (spirit.spiritID == spiritToRemove.spiritID)
            {
                AIManager.Instance.freeSpirits.Remove(spirit);
                break;
            }
        }

        OnSpiritsCountChanged?.Invoke();
    }
    public bool CanAssignSpiritToWork(Building building)
    {
        // TODO: Check if we can change MasSpirits based on building state
        if (building.BuildingState != BuildingState.Construction && building.BuildingState != BuildingState.Repairing)
            return building.SpiritsConstructingMe < building.PlacesToStay.Count
                && building.Spirits.Count < building.MaxSpirits
                && AIManager.Instance.freeSpirits.Count > 0;
        else
            return building.SpiritsConstructingMe < building.PlacesToStay.Count
                    && AIManager.Instance.freeSpirits.Count > 0;
    }

    public bool CanRemoveSpiritFromWork(Building building)
    {
        return building.Spirits.Count > 0;
    }

    public void AddSpiritToWork(Building building)
    {
        if (!CanAssignSpiritToWork(building))
        {
            return;
        }
        int indexOfSpirit = UnityEngine.Random.Range(0, freeSpirits.Count - 1);

        building.Spirits.Add(freeSpirits[indexOfSpirit]);
        SpiritWorking(freeSpirits[indexOfSpirit], building);
        building.CurrentSpirits++;

        OnWorkerAmountChange?.Invoke();
        OnFreeSpiritsCountChanged?.Invoke();
    }

    public void RemoveSpiritFromWork(Building building)
    {
        if (!CanRemoveSpiritFromWork(building))
        {
            return;
        }

        int indexOfSpirit = UnityEngine.Random.Range(0, building.Spirits.Count - 1);

        foreach (Spirit spirit in workingSpirits)
        {
            if (building.Spirits[indexOfSpirit].spiritID == spirit.spiritID)
            {
                if (spirit.IsVisible == false)
                    spirit.IsVisible = true;
                building.Spirits.Remove(spirit);
                building.CurrentSpirits--;
                building.GiveBackPlace(spirit.placeToStay);
                SpiritChilling(spirit);
                break;
                //return;
            }
        }

        OnWorkerAmountChange?.Invoke();
        OnFreeSpiritsCountChanged?.Invoke();
    }

    public void RemoveSpiritFromWork(Building building, Spirit spirit)
    {
        if (!CanRemoveSpiritFromWork(building))
        {
            return;
        }

        if (spirit.IsVisible == false)
            spirit.IsVisible = true;
        building.Spirits.Remove(spirit);
        building.CurrentSpirits--;
        building.GiveBackPlace(spirit.placeToStay);
        SpiritChilling(spirit);

        OnWorkerAmountChange?.Invoke();
        OnFreeSpiritsCountChanged?.Invoke();
    }
    private void SpiritWorking(Spirit spirit, Building building)
    {
        freeSpirits.Remove(spirit);
        workingSpirits.Add(spirit);
        spirit.gameObject.SetActive(true);
        spirit.workPlace = building;

        switch(building.WorkType)
        {
            case WorkTypeEnum.Constructing: 
                spirit.SpiritWork = SpiritWorkState.Constructing;
                break;
            case WorkTypeEnum.Repairing:
                spirit.SpiritWork = SpiritWorkState.Constructing;
                break;
            case WorkTypeEnum.Collecting: 
                spirit.SpiritWork = SpiritWorkState.Collecting;
                break;
            case WorkTypeEnum.Praying: 
                spirit.SpiritWork = SpiritWorkState.Praying;
                break;
            case WorkTypeEnum.Guarding: 
                spirit.SpiritWork = SpiritWorkState.Guarding;
                break;
            case WorkTypeEnum.Shooting: 
                spirit.SpiritWork = SpiritWorkState.UsingTurret;
                break;
            case WorkTypeEnum.Researching: 
                spirit.SpiritWork = SpiritWorkState.Researching;
                break;
        }
    }

    private void SpiritChilling(Spirit spirit)
    {
        workingSpirits.Remove(spirit);
        freeSpirits.Add(spirit);
        OnWorkerAmountChange?.Invoke();
        OnFreeSpiritsCountChanged?.Invoke();
        spirit.SpiritWork = SpiritWorkState.Idle;
        spirit.workPlace = null;
    }

    public void EnemiesWaveIsOver()
    {
        foreach (Spirit spirit in spirits)
        {
            if (spirit.Working && spirit.atHome) spirit.gameObject.SetActive(true);
            spirit.waitUntilEnemiesLeave = false;
        }
    }
}
