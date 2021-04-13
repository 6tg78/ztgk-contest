using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables

    public Canvas ScreenSpaceCanvas;
    public bool StageThree;
    public GameObject thirdResourceGroup;

    public Image[] resourcesIcons; //array with all resources icons to choose
    public bool GamePaused { get; set; }

    public bool BuildingsPanelAnimation { get; set; }

    public static UIManager Instance { get; private set; }
    public GameObject AdvancedPanelInstance { get; set; }

    private bool stageThreeEntered;

    private BuildingUIComponent _buildingUIComponent;
    private Building _selectedBuilding;

    [SerializeField] private Transform _uiParentTransform;
    public Transform UIParentTransform { get { return _uiParentTransform; } }

    [SerializeField] private NextWaveTimer _nextWaveTimer;
    public NextWaveTimer NextWaveTimer { get { return _nextWaveTimer; } }


    public GameObject trapsPanel;


    #endregion

    #region Mono Behaviour

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SelectionController.OnSelectedBuildingChanged += HandleSelection;
    }

    void OnDestroy()
    {
        SelectionController.OnSelectedBuildingChanged -= HandleSelection;
    }

    private void Start()
    {
        AdvancedPanelInstance = null;
        stageThreeEntered = false;
        GamePaused = false;
        _selectedBuilding = null;
        NextWaveTimer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (StageThree && !stageThreeEntered)
        {
            stageThreeEntered = true;
            trapsPanel.SetActive(true);
            StageProgress();
        }
    }
    #endregion

    #region Component

    public void SetGameSpeed(float speedValue)
    {
        Time.timeScale = speedValue;
    }

    public void SelectStage3Variant(int choice)
    {
        GameManager.Instance.SelectedStage3Variant = choice;
    }

    public void PrepareBuildingForStage3(Building building) // You have to use it on prefabs in UI, if u want to use them for placing buildings.
    {
        BalancePanel.Instance.BalanceUpdate(building); // Updating basic stats of the building.
        building.ChangeModel(GameManager.Instance.SelectedStage3Variant);
    }

    public void StageProgress()
    {
        thirdResourceGroup.GetComponent<CanvasGroup>().alpha = 1.0f;
        StageThree = true;
    }
    #endregion

    /// <summary>
    /// Callback for handling building selection.
    /// Creates UI Panel for selected building.
    /// </summary>
    /// <param name="building">Selected building.</param>
    public void HandleSelection(Building building)
    {
        // If selection didn't change, do nothing
        if (_selectedBuilding == building)
            return;

        // It changed so clear current UI panel
        _buildingUIComponent?.RemoveUI();
        _buildingUIComponent = null;

        // If it was deselection, we are done
        _selectedBuilding = building;
        if (building == null)
            return;

        // Else create UI for new selection
        var buildingUI = building.GetComponent<BuildingUI>();
        _buildingUIComponent = buildingUI.BuildingUIPrefab.GetComponent<BuildingUIComponent>();
        _buildingUIComponent.CreateUI(building);
    }

    /// <summary>
    /// Closes currently open UI Panel.
    /// </summary>
    public void CloseBuildingUI()
    {
        HandleSelection(null);
    }
}
