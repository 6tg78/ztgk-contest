using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingsManager : MonoBehaviour
{
    public static BuildingsManager Instance { get; private set; }

    public bool InConstrucionPlanningMode { get; private set; }
    public bool PlacementPossible { get; set; }

    private float mouseWheelRotation;
    private GameObject currentBuilding;
    private GameObject copy = null;
    private LayerMask terrainLayerMask;

    [SerializeField]
    private Altar altar;
    [SerializeField]
    private Vector3 altarStartPosition;

    private AudioSource placingSound;

    public event Action<GameObject, bool, bool, bool> OnBuildingNotBuild;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
    }

    private void Start()
    {

        if (GameManager.Instance.altars.Count == 0) SetStartAltar();
    }

    private void SetStartAltar()
    {
        Building startAltar = Instantiate(altar);
        startAltar.transform.position = altarStartPosition;
        startAltar.BuildingState = BuildingState.Construction;

        var colliders = startAltar.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.isTrigger = true;
        }
        startAltar.GetComponent<Building>().Obstacles.SetActive(true);
        startAltar.GetComponent<Building>().ChangeModel(0);

        startAltar.SpiritsConstructingMe = int.MaxValue;//to build it as fast as possible
        
    }

    void Update()
    {
        ConstrucionPlanningModeManagement();
    }


    // Method for UI building creation.
    public void Build(GameObject enteredPrefab)
    {
        enteredPrefab.SetActive(true);

        //should repair shader bug
        copy = enteredPrefab;
        //copy.SetActive(false);

        if (InConstrucionPlanningMode == false)
        {
            if (enteredPrefab.GetComponent<Building>() != null)
            {
                if (!ResourceManagement.Instance.EnoughResource<LifeEnergyResource>(enteredPrefab.GetComponent<Building>().BuildingCost.lifeEnergy)
                    || !ResourceManagement.Instance.EnoughResource<WoodResource>(enteredPrefab.GetComponent<Building>().BuildingCost.wood)
                    || !ResourceManagement.Instance.EnoughResource<ThirdResource>(enteredPrefab.GetComponent<Building>().BuildingCost.thirdResource)
                    )
                {
                    OnBuildingNotBuild?.Invoke(enteredPrefab, !ResourceManagement.Instance.EnoughResource<LifeEnergyResource>(enteredPrefab.GetComponent<Building>().BuildingCost.lifeEnergy),
                        !ResourceManagement.Instance.EnoughResource<WoodResource>(enteredPrefab.GetComponent<Building>().BuildingCost.wood),
                        !ResourceManagement.Instance.EnoughResource<ThirdResource>(enteredPrefab.GetComponent<Building>().BuildingCost.thirdResource));
                    ShortNotification.Instance.TriggerNotification("Not enough resources!");
                    return;
                }
            }
            InConstrucionPlanningMode = true;
            PlacementPossible = true;
            currentBuilding = Instantiate(enteredPrefab);
            if (currentBuilding.GetComponent<Building>() != null)
            {
                // currentBuilding.GetComponent<Building>().InConstrucionPlanningMode = true;
                currentBuilding.GetComponent<Building>().BuildingState = BuildingState.Planning;
                currentBuilding.GetComponent<Building>().ChangeModel(GameManager.Instance.SelectedStage3Variant);
            }
            else if (currentBuilding.GetComponent<Trap>() != null) // We will use building system functionality for traps.
            {
                currentBuilding.GetComponent<Trap>().InConstrucionPlanningMode = true;
            }
            currentBuilding.transform.position += new Vector3(-10000, 0, -10000); // It prevents the object from spawning in the (0,0,0) position. It looked bad.
        }
        else // It executes when we click to build some building, but haven't yet exited build mode.
        {
            QuitConstrucionPlanningMode();
            Build(enteredPrefab);
        }
    }

    public void QuitConstrucionPlanningMode()
    {
        PlacementPossible = false;
        InConstrucionPlanningMode = false;
        Destroy(currentBuilding);
    }


    private void ConstrucionPlanningModeManagement()
    {
        if (InConstrucionPlanningMode)
        {
            MoveCurrentBuildingToMouse();
            RotateBuildingUsingMouseWheel();
            if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false) // third condition is checking whether we clicked UI element or not
            {
                if(PlacementPossible)
                {
                    //cannot create copy here because it creates copy of building from gameObject with changed material - here blue shader 
                    //var copy = Instantiate(currentBuilding);
                    //copy.SetActive(false);
                    Place();
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        Build(copy);
                    }
                    else
                        //Destroy(copy);
                        copy = null;
                }
                else
                {
                    ShortNotification.Instance.TriggerNotification("You cannot place your structure here!");
                }
            }
            if(Input.GetMouseButtonUp(1))
            {
                QuitConstrucionPlanningMode();
            }
        }
    }

    private void Initialize()
    {
        PlacementPossible = false;
        InConstrucionPlanningMode = false;
        terrainLayerMask = LayerMask.GetMask("Terrain");
        placingSound = gameObject.GetComponent<AudioSource>();
    }

    private void MoveCurrentBuildingToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1000f, terrainLayerMask))
        {
            currentBuilding.transform.position = hitInfo.point;
            /*
            if(currentBuilding.GetComponent<Building>().MaterialChangingFunctionVersion == 1)
            {
                currentBuilding.transform.position -= new Vector3(0, prefabMesh.mesh.bounds.min.y*currentBuilding.transform.localScale.y, 0); //set bottom of building on terrain
            }
            */
            currentBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
    }

    private void RotateBuildingUsingMouseWheel()
    {
        mouseWheelRotation += Input.mouseScrollDelta.y;
        currentBuilding.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
    }

    private void Place()
    {
        var colliders = currentBuilding.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.isTrigger = true;
        }
        if (currentBuilding.GetComponent<Building>() != null)
        {
            ResourceManagement.Instance.UseResource<LifeEnergyResource>(currentBuilding.GetComponent<Building>().BuildingCost.lifeEnergy);
            ResourceManagement.Instance.UseResource<WoodResource>(currentBuilding.GetComponent<Building>().BuildingCost.wood);
            ResourceManagement.Instance.UseResource<ThirdResource>(currentBuilding.GetComponent<Building>().BuildingCost.thirdResource);
            // currentBuilding.GetComponent<Building>().InConstrucionPlanningMode = false;
            // currentBuilding.GetComponent<Building>().InConstructionMode = true;
            currentBuilding.GetComponent<Building>().BuildingState = BuildingState.Construction;
            currentBuilding.GetComponent<Building>().ChangeMaterial(3);
            currentBuilding.tag = "Construction";
            currentBuilding.GetComponent<Building>().Obstacles.SetActive(true);
        }
        else if (currentBuilding.GetComponent<Trap>() != null)
        {
            currentBuilding.GetComponent<Trap>().InConstrucionPlanningMode = false;
            currentBuilding.GetComponent<Trap>().ChangeShader(Shader.Find("Standard"));
            currentBuilding.tag = "Trap"; //remember to create tag before using it in scripts
            currentBuilding.GetComponent<Trap>().activationArea.isTrigger = false;
            TrapManager.Instance.PlaceTrap(currentBuilding.GetComponent<Trap>().trapType);
        }
        PlacementPossible = false;
        InConstrucionPlanningMode = false;
        currentBuilding = null;
        if(placingSound.clip != null)
        {
            placingSound.Play();
        }
        else
        {
            Debug.Log("Buildings manager doesn't have placing sound assigned.");
        }
    }
}