using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverUIComponent : MonoBehaviour
{
    [SerializeField] private GameObject _constructionBarPrefab;
    [SerializeField] private GameObject _healthBarPrefab;

    private GameObject _constructionBarInstance = null;
    private GameObject _healthBarInstance = null;

    private ProgressBar _constructionBar = null;
    private ProgressBar _healthBar = null;

    private Building _boundBuilding;
    public Building BoundBuilding { get { return _boundBuilding; } }


    public void Bind(Building building)
    {
        _boundBuilding = building;
        BoundBuilding.OnConstructionProgress += HandleConstructionBar;
        BoundBuilding.OnHealthChanged += HandleHealthBar;

        BoundBuilding.OnStateChanged += HandleHealthBarBuildingState;
        BoundBuilding.OnStateChanged += HandleConstructionBarBuildingState;
    }

    public void Unbind()
    {
        BoundBuilding.OnConstructionProgress -= HandleConstructionBar;
        BoundBuilding.OnHealthChanged -= HandleHealthBar;

        BoundBuilding.OnStateChanged -= HandleHealthBarBuildingState;
        BoundBuilding.OnStateChanged -= HandleConstructionBarBuildingState;
    }

    void Update()
    {
        if (_healthBarInstance != null)
        {
            TranslateProgressBar(_healthBarInstance, 70);
        }

        if (_constructionBarInstance != null)
        {
            TranslateProgressBar(_constructionBarInstance, 100);
        }
    }

    private void OnDestroy()
    {
        HandleHealthBarBuildingState(BuildingState.Destroyed);
    }

    /// <summary>
    /// Callback for handling BuildingState changes.
    /// Creates and removes Health Bar based on BoundBuilding state.
    /// </summary>
    /// <param name="state">Current building state.</param>
    void HandleHealthBarBuildingState(BuildingState state)
    {
        switch (state)
        {
            case BuildingState.Planning:
            case BuildingState.Destroyed:
                {
                    RemoveProgressBar(_healthBarInstance, out _healthBar);
                    return;
                }
        }

        if (_healthBarInstance == null)
            CreateProgressBarInstance(_healthBarPrefab,
                                      out _healthBarInstance,
                                      out _healthBar);

        _healthBarInstance.SetActive(false);
        var health = BoundBuilding.CurrHitPoints / BoundBuilding.MaxHitPoints;
        _healthBar.Progress = health;
        _healthBar.Label = string.Format("Health: {0}", BoundBuilding.CurrHitPoints);
    }

    /// <summary>
    /// Callback for handling BuildingState changes.
    /// Creates and removes Construction Bar based on BoundBuilding state.
    /// </summary>
    /// <param name="state">Current building state.</param>
    void HandleConstructionBarBuildingState(BuildingState state)
    {
        switch (state)
        {
            case BuildingState.Construction:
                {
                    CreateProgressBarInstance(_constructionBarPrefab,
                                              out _constructionBarInstance,
                                              out _constructionBar);

                    var completed = BoundBuilding.DegreeOfConstructionCompletion;
                    _constructionBar.Progress = completed / 100.0f;
                    _constructionBar.Label = string.Format("Constructing: {0}%", (int)(completed));
                    return;
                }
        }
        RemoveProgressBar(_constructionBarInstance, out _constructionBar);
    }

    /// <summary>
    /// Callback for handling Building construction progress.
    /// </summary>
    private void HandleConstructionBar()
    {
        if (_constructionBar == null)
            return;

        var completed = BoundBuilding.DegreeOfConstructionCompletion;
        _constructionBar.Progress = completed / 100.0f;
        _constructionBar.Label = string.Format("Constructing: {0}%", (int)(completed));
    }

    /// <summary>
    /// Callback for handling Building health changes.
    /// </summary>
    private void HandleHealthBar()
    {
        if (_healthBar == null)
            return;

        if (Mathf.Approximately(BoundBuilding.CurrHitPoints, BoundBuilding.MaxHitPoints))
            _healthBarInstance.SetActive(false);
        else
            _healthBarInstance.SetActive(true);

        var health = BoundBuilding.CurrHitPoints / BoundBuilding.MaxHitPoints;
        _healthBar.Progress = health;
        _healthBar.Label = string.Format("Health: {0}", BoundBuilding.CurrHitPoints);
    }


    // Helper functions

    /// <summary>
    /// Instantiates and assigns ProgressBar to variables.
    /// </summary>
    /// <param name="prefab">ProgressBar prefab.</param>
    /// <param name="instance">ProgressBar instance.</param>
    /// <param name="bar">ProgressBar reference.</param>
    private void CreateProgressBarInstance(GameObject prefab, out GameObject instance, out ProgressBar bar)
    {
        instance = Instantiate(prefab, UIManager.Instance.ScreenSpaceCanvas.transform);
        instance.transform.SetSiblingIndex(0);
        bar = instance.GetComponent<ProgressBar>();
    }

    /// <summary>
    /// Translates instance by given offset in Y axis.
    /// </summary>
    /// <param name="instance">Object instance.</param>
    /// <param name="offset">Offset.</param>
    private void TranslateProgressBar(GameObject instance, int offset)
    {
        var position = Camera.main.WorldToScreenPoint(BoundBuilding.transform.position);
        position.y += offset;
        instance.transform.position = position;
    }

    /// <summary>
    /// Removes instance object if possible and sets ProgressBar reference to null.
    /// </summary>
    /// <param name="instance">Object instance.</param>
    /// <param name="bar">ProgressBar reference.</param>
    private void RemoveProgressBar(GameObject instance, out ProgressBar bar)
    {
        bar = null;
        if (instance == null)
            return;
        Destroy(instance);
    }

}
