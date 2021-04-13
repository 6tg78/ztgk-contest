using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script for clicking on button to hide / show more buttons for buildings
/// </summary>
public class HideShowBuildingsPanel : MonoBehaviour
{
    public GameObject Panel;
    public Sprite leftArrowSprite;
    public Sprite rightArrowSprite;
    public float animationShift;

    private bool flag;
    private bool flagChanged;
    private new bool animation;

    private float panelWidth;

    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = leftArrowSprite;
        flag = false;
        flagChanged = false;
        animation = false;
        panelWidth = Panel.GetComponent<RectTransform>().rect.width * UIManager.Instance.ScreenSpaceCanvas.transform.localScale.x;
        Panel.transform.Translate(-panelWidth, 0, 0);
    }

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            HideShowOnClick();
        }

        if(flagChanged)
        {
            // True means show
            if (flag)
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().sprite = rightArrowSprite;
            }
            // False means hide
            else
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().sprite = leftArrowSprite;
            }
            flagChanged = false;
            animation = true;
            UIManager.Instance.BuildingsPanelAnimation = true;
        }

        if(animation)
        {
            if (flag)
            {
                if( Mathf.Approximately(Panel.transform.position.x, 0.0f) || Panel.transform.position.x > 0)
                {
                    animation = false;
                    UIManager.Instance.BuildingsPanelAnimation = false;
                    Panel.transform.position = new Vector3(0, Panel.transform.position.y, Panel.transform.position.z);
                    gameObject.transform.position = new Vector3(panelWidth, gameObject.transform.position.y, gameObject.transform.position.z);
                }
                else
                {
                    Panel.transform.position += new Vector3(animationShift, 0, 0) * Time.unscaledDeltaTime * 100f;
                    gameObject.transform.position += new Vector3(animationShift, 0, 0) * Time.unscaledDeltaTime * 100f;
                    ShortNotification.Instance.ShiftWithPanel(animationShift * 1.6f);
                }
            }
            else
            {
                if( Mathf.Approximately(Panel.transform.position.x, -panelWidth) || Panel.transform.position.x < -panelWidth)
                {
                    animation = false;
                    UIManager.Instance.BuildingsPanelAnimation = false;
                    Panel.transform.position = new Vector3(-panelWidth, Panel.transform.position.y, Panel.transform.position.z);
                    gameObject.transform.position = new Vector3(0, gameObject.transform.position.y, gameObject.transform.position.z);
                }
                else
                {
                    Panel.transform.position -= new Vector3(animationShift, 0, 0) * Time.unscaledDeltaTime * 100f;
                    gameObject.transform.position -= new Vector3(animationShift, 0, 0) * Time.unscaledDeltaTime * 100f;
                    ShortNotification.Instance.ShiftWithPanel(-animationShift * 1.6f);
                }
            }
        }
    }

    //Listener for the button
    public void HideShowOnClick()
    {
        flag = !flag;
        flagChanged = true;
    }
}
