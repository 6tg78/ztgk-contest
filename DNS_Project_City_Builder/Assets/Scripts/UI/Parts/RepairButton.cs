using System;
using UnityEngine;

public class RepairButton : MonoBehaviour
{
    public event Action OnRepair;

    public void Repair()
    {
        OnRepair?.Invoke();
    }

}
