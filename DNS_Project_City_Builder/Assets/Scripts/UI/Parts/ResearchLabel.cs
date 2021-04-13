using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ResearchLabel : MonoBehaviour
{
    //public List<TextMeshProUGUI> researchLabels;
    //public List<Image> researchImages;
    public List<ResearchPanel> researchs;

    public event Action<int> OnResearchSeleccted;

    public void SelectResearch(int research)
    {
        OnResearchSeleccted?.Invoke(research);
    }
}
