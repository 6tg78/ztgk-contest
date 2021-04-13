using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Building))]
public class BuildingUI : MonoBehaviour
{

    /// <summary>
    /// Prefab that is 'holding' UI for the building.
    /// Must implement BuildingUIComponent.
    /// </summary>
    [FormerlySerializedAs("_buildingUIPrefab")]
    [SerializeField] private GameObject _buildingUIPrefab;
    public GameObject BuildingUIPrefab { get { return _buildingUIPrefab; } }

    /// <summary>
    /// Prefab that is 'holding' hover UI for the building.
    /// Must implement HoverUIComponent.
    /// </summary>
    [SerializeField] private GameObject _hoverUIPrefab;
    private GameObject _hoverUIInstance;
    private HoverUIComponent _hoverUIComponent = null;

    public string Description;
    private Building _building;

    void Awake()
    {
        _building = GetComponent<Building>();
        CreateHoverUI();
    }

    void OnDestroy()
    {
        RemoveHoverUI();
    }

    /// <summary>
    /// Creates HoverUI Instance for the Building.
    /// </summary>
    private void CreateHoverUI()
    {
        if (_hoverUIPrefab != null)
        {
            _hoverUIInstance = Instantiate(_hoverUIPrefab, UIManager.Instance.UIParentTransform);
            _hoverUIComponent = _hoverUIInstance.GetComponent<HoverUIComponent>();
            _hoverUIComponent.Bind(_building);
        }
    }

    /// <summary>
    /// Removes HoverUI Instance.
    /// </summary>
    private void RemoveHoverUI()
    {
        if (_hoverUIInstance != null)
        {
            _hoverUIComponent.Unbind();
            _hoverUIComponent = null;
            Destroy(_hoverUIInstance);
        }
    }
}
