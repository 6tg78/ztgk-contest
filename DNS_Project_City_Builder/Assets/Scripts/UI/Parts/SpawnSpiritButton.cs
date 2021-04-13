using UnityEngine;
using System;

public class SpawnSpiritButton : MonoBehaviour
{
    public Action OnSpawnButton;

    public void Spawn()
    {
        OnSpawnButton?.Invoke();
    }
}
