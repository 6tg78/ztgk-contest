using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    public float Progress { get { return _slider.value; } set { _slider.value = value; SetColor(value); } }
    [SerializeField] private TextMeshProUGUI _text;
    public string Label { get { return _text.text; } set { _text.text = value; } }
    [SerializeField] private Image _image;

    [SerializeField] private Color _beginColor;
    public Color BeginColor { get { return _beginColor; } set { _beginColor = value; } }
    [SerializeField] private Color _endColor;
    public Color EndColor { get { return _endColor; } set { _endColor = value; } }
    private void SetColor(float value)
    {
        _image.color = Color.Lerp(BeginColor, EndColor, value);
    }
}
