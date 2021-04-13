using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmountPanel : MonoBehaviour
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _amountText;
    public string AmountString { get { return _amountText.text; } set { _amountText.text = value; } }
    [SerializeField] private TextMeshProUGUI _incomeText;
    public string IncomeString { get { return _incomeText.text; } set { _incomeText.text = value; } }

    void Awake()
    {
        _image.sprite = _icon;
    }

}
