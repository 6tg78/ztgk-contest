using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerShortInformation : MonoBehaviour
{
    #region Variables

    public GameObject PanelPrefab;
    public GameObject PanelInstance { get; private set; }

    private GameObject canvas;
    private Building refBuilding;

    #endregion

    #region Mono Behaviour
    void Start()
    {
        canvas = UIManager.Instance.ScreenSpaceCanvas.gameObject;
        refBuilding = gameObject.GetComponent<Building>();
    }

    // Called when mouse get over a button
    public void OnMouseEnter()
    {
        if (PanelInstance == null)
        {
            // Displaying panel only if building is not in build mode
            if (!GetComponent<Building>().InConstructionMode && !GetComponent<Building>().InConstrucionPlanningMode && !BuildingsManager.Instance.InConstrucionPlanningMode)
            {
                PanelInstance = Instantiate(PanelPrefab, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
                PanelInstance.GetComponent<BuildingShortInformation>().SetPanel(refBuilding);
                // Chcecking if panel is outside the UI Canvas
                Vector3 panelPos = Camera.main.WorldToScreenPoint(transform.position);
                panelPos.x += 30 * canvas.transform.localScale.x;
                panelPos.y += 30 * canvas.transform.localScale.y;

                // Checking if panel will go over the screen
                if ((panelPos.x + PanelInstance.GetComponent<RectTransform>().rect.width * canvas.transform.localScale.x) > canvas.GetComponent<RectTransform>().rect.width)
                {
                    panelPos.x -= PanelInstance.GetComponent<RectTransform>().rect.width * canvas.transform.localScale.x;
                }
                if ((panelPos.y + PanelInstance.GetComponent<RectTransform>().rect.height * canvas.transform.localScale.y) > canvas.GetComponent<RectTransform>().rect.height)
                {
                    panelPos.y -= PanelInstance.GetComponent<RectTransform>().rect.height * canvas.transform.localScale.y;
                }

                PanelInstance.transform.position = panelPos;
            }
        }
    }

    public void OnMouseExit()
    {
        Destroy(PanelInstance);
        PanelInstance = null;
    }
    #endregion
}
