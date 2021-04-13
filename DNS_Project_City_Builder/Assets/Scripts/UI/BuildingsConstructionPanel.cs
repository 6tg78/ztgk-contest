using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsConstructionPanel : MonoBehaviour
{

    /// <summary>
    /// Scroll list transform.
    /// </summary>
    [SerializeField] private RectTransform _scrollList;
    /// <summary>
    /// Scroll list content transform.
    /// </summary>
    [SerializeField] private RectTransform _content;

    /// <summary>
    /// Button prefab - contains icons and required resources.
    /// </summary>
    [SerializeField] private GameObject _buttonPrefab;

    /// <summary>
    /// List of buildings for Stage 2.
    /// </summary>
    [SerializeField] private GameObject[] _buildingsStage2;

    /// <summary>
    /// List of buildings for Stage 3.
    /// </summary>
    [SerializeField] private GameObject[] _buildingsStage3;
    void Start()
    {
        PopulateContent();
        Refresh();
        GameManager.Instance.OnStageChanged += Refresh;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStageChanged -= Refresh;
    }

    /// <summary>
    /// Removes all children from scroll list content.
    /// </summary>
    public void ClearContent()
    {
        // Can't do Destroy directly on transform's child.
        var contentCopy = new List<RectTransform>();
        foreach (RectTransform child in _content)
        {
            if(child == _content)
                continue;
            contentCopy.Add(child);
        }

        foreach(RectTransform child in contentCopy)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// Pupulates scroll list's content with buttons.
    /// </summary>
    public void PopulateContent()
    {
        int stage = GameManager.Instance.CurrentStage;
        
        if(stage >= 1)
            CreateButtonsForList(_buildingsStage2);
        
        if(stage >= 3)
            CreateButtonsForList(_buildingsStage3);
    }

    /// <summary>
    /// Clears and Repopulates scroll list's content.
    /// </summary>
    public void Refresh()
    {
        ClearContent();
        PopulateContent();
    }

    /// <summary>
    /// Instantiates and sets up buttons.
    /// </summary>
    /// <param name="list">List of buildings prefabs.</param>
    private void CreateButtonsForList(GameObject[] list)
    {
        foreach(GameObject prefab in list)
        {
            var buttonGO = Instantiate(_buttonPrefab, _content);
            var button = buttonGO.GetComponent<BuildingButtonNew>();
            if(!button)
            {
                Debug.Log("Button component == null");
                continue;
            }
            button.refBuilding = prefab.GetComponent<Building>();
            button.ParentTransform = transform;
            button.Setup();
        }
    }
}
