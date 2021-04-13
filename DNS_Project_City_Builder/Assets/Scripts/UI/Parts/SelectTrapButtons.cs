using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SelectTrapButtons : MonoBehaviour
{
    public Action OnOptionA;
    public Action OnOptionB;

    public string LabelString { get { return _textLabel.text; } set { _textLabel.text = value; } }

    [SerializeField]
    private TextMeshProUGUI _textLabel;

    public void PickOptionA()
    {
        OnOptionA?.Invoke();
    }

    public void PickOptionB()
    {
        OnOptionB?.Invoke();
    }
}
