using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notification : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _label;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] Image _image;
    Color color;

    public string Label { get { return _label.text; } set { _label.text = value; } }
    public string Description { get { return _description.text; } set { _description.text = value; } }
    public Color ColorRef {get {return color;} set
        {
            _label.color = value;
            _image.color = value;
        }
    }

    public void ButtonClicked()
    {
        NotificationManager.Instance.NotificationReadingFinished();
    }
}
