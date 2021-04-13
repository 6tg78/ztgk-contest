using System;
using UnityEngine;

public class WorkshopMenuButton : MonoBehaviour
{
    public event Action OnWorshopMenuOpen;

    public void OpenWorkshopMenu()
    {
        OnWorshopMenuOpen?.Invoke();
    }
}