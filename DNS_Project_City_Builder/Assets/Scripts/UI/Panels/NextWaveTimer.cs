using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NextWaveTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private TextMeshProUGUI _time;
    public string Label { get { return _label.text; } set { _label.text = value; } }
    public string Time { get { return _time.text; } set { _time.text = value; } }
    
}
