using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This class handles UI panels for Building instance.
/// It selects which UI to display based on Building State.
/// Derive this class to implement custom behaviour.
/// </summary>
public class BuildingUIComponent : MonoBehaviour
{
    /// <summary>
    /// Building for which the UI is displayed.
    /// </summary>
    private Building _boundBuilding;
    public Building BoundBuilding { get { return _boundBuilding; } protected set { _boundBuilding = value; } }

    private GameObject _uiInstance;
    /// <summary>
    /// Instantiated UI Panel prefab.
    /// </summary>
    public GameObject UIInstance { get { return _uiInstance; } set { _uiInstance = value; } }
    
    private BuildingUIPanel _uiPanel;
    /// <summary>
    /// Reference to BuildingUIPanel on UIInstance.
    /// </summary>
    public BuildingUIPanel UIPanel { get { return _uiPanel; } set { _uiPanel = value; } }

    [FormerlySerializedAs("_constructionPrefab")]
    [SerializeField] protected GameObject _constructionPrefab;

    [FormerlySerializedAs("_builtPrefab")]
    [SerializeField] protected GameObject _builtPrefab;

    [FormerlySerializedAs("_damagedPrefab")]
    [SerializeField] protected GameObject _damagedPrefab;

    [FormerlySerializedAs("_repairingPrefab")]
    [SerializeField] protected GameObject _repairingPrefab;

    /// <summary>
    /// Creates selected UI.
    /// </summary>
    /// <param name="building"></param>
    public virtual void CreateUI(Building building)
    {
        BoundBuilding = building;
        BoundBuilding.OnStateChanged += Refresh;
        var prefab = SelectPanel();
        SetUp(prefab);
    }

    /// <summary>
    /// Removes current UI.
    /// </summary>
    public virtual void RemoveUI()
    {
        if (_uiInstance != null)
        {
            Destroy(_uiInstance);
            _uiInstance = null;
        }
        BoundBuilding.OnStateChanged -= Refresh;
        BoundBuilding = null;
    }

    /// <summary>
    /// Removes and recreates the UI.
    /// UI Panel is reselected.
    /// </summary>
    /// <param name="state">Unused.</param>
    public virtual void Refresh(BuildingState state)
    {
        if (_uiInstance != null)
        {
            Destroy(_uiInstance);
            _uiInstance = null;
        }

        var prefab = SelectPanel();
        SetUp(prefab);
    }

    /// <summary>
    /// Instantiates and prepares UI Panel.
    /// </summary>
    /// <param name="prefab">Selected UI Prefab.</param>
    public virtual void SetUp(GameObject prefab)
    {
        if (prefab == null)
        {
            //TODO: Remove this message.
            //Debug.LogWarning("BuildingUIComponent: No prefab selected");
            return;
        }
        _uiInstance = Instantiate(prefab, UIManager.Instance.ScreenSpaceCanvas.transform);
        _uiPanel = UIInstance.GetComponent<BuildingUIPanel>();
        UIPanel.Bind(BoundBuilding);
    }

    /// <summary>
    /// Selects which UI Prefab should be instantiated based on Building State.
    /// </summary>
    /// <returns>Selected prefab.</returns>
    public virtual GameObject SelectPanel()
    {
        if (UIInstance != null)
            Destroy(UIInstance);

        switch (BoundBuilding.BuildingState)
        {
            case BuildingState.Construction:
                {
                    return _constructionPrefab;
                }
            case BuildingState.Built:
                {
                    return _builtPrefab;
                }
            case BuildingState.Damaged:
                {
                    return _damagedPrefab;
                }
            case BuildingState.Repairing:
                {
                    return _repairingPrefab;
                }
        }

        return null;

    }
}
