using UnityEngine;
using TMPro;

public class BuildingDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    public string DescriptionString { get { return _text.text; } set { _text.text = value; } }
}
