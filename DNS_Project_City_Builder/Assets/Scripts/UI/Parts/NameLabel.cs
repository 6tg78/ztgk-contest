using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NameLabel : MonoBehaviour
{
    public Action OnCloseCallback;

    [SerializeField]
    private TextMeshProUGUI _textLabel;

    public string LabelString { get { return _textLabel.text; } set { _textLabel.text = value; } }
    
    public void Close()
    {
        OnCloseCallback?.Invoke();
    }

    public void DisableSelectionRings()
    {
        GameManager.Instance.DisableSelectionRings();
    }
}
