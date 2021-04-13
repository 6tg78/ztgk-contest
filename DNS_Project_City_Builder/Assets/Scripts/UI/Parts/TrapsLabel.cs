using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrapsLabel : MonoBehaviour
{
    public event Action<Trap> OnTrapSelected;

    public List<TrapsPanel> traps;
    //public List<TextMeshProUGUI> textLabels;
    //public List<Image> images;
    public void SelectTrap(Trap trap)
    {
        OnTrapSelected?.Invoke(trap);
    }
}
