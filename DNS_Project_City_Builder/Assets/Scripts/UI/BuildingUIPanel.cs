using UnityEngine;

/// <summary>
/// Base class for Building UI Panels.
/// Derived classes should implement logic/behaviour between parts of UIPanel.
/// </summary>
public class BuildingUIPanel : MonoBehaviour
{
    private Building _boundBuilding;
    public Building BoundBuilding{ get { return _boundBuilding; } protected set { _boundBuilding = value; } }
    
    public virtual void Bind(Building building)
    {
        Debug.Log("Bind not implemented!");
    }
}
