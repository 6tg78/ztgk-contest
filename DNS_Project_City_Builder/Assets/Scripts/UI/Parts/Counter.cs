using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Counter : MonoBehaviour
{
    public Action OnAddCallback;
    public Action OnSubtractCallback;

    public string LabelString { get { return _textLabel.text; } set { _textLabel.text = value; } }

    [SerializeField]
    private TextMeshProUGUI _textLabel;

    public void Add()
    {
        OnAddCallback?.Invoke();
    }

    public void Remove()
    {
        OnSubtractCallback?.Invoke();
    }

}
