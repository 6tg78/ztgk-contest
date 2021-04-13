using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class manages building selection.
/// </summary>
public class SelectionController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    /// <summary>
    /// Callback - triggered when new building is selected or deselected.
    /// If building is deselected - null is passed.
    /// </summary>
    public static Action<Building> OnSelectedBuildingChanged;
    public static Building SelectedBuilding { get; private set; }
    
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            // If clicked UI - do nothing
            if(EventSystem.current.IsPointerOverGameObject())
                return;
            
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hitInfo))
            {
                var building = hitInfo.collider.GetComponent<Building>();
                SelectedBuilding = building;
                OnSelectedBuildingChanged?.Invoke(building);
            }
        }
    }
}
