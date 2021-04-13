using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance {get; private set;}

    public string CurrentResearchName {get; private set;}
    public float TimeNeededForCurrentResearch {get; private set;}
    public float Value {get; private set;}
    public bool ResearchStarted {get; private set;}

    public float ResearchProgress { get; private set; } // % of current research completion.
    private int currResearchIndex;
    private AudioSource researchCompleteSound;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        Initialize();
    }


    public void StartNewResearch(int index) // For UI.
    {
        currResearchIndex = index;
        ResearchData set = new ResearchData();
        bool noError = true;
        switch(currResearchIndex)
        {
            case 1:
                set = BalancePanel.Instance.ResearchStats.research1;
                break;
            case 2:
                set = BalancePanel.Instance.ResearchStats.research2;
                break;
            case 3:
                set = BalancePanel.Instance.ResearchStats.research3;
                break;
            case 4:
                set = BalancePanel.Instance.ResearchStats.research4;
                break;
            case 5:
                set = BalancePanel.Instance.ResearchStats.research5;
                break;
            case 6:
                set = BalancePanel.Instance.ResearchStats.research6;
                break;    
            default:
                noError = false;
                break;
        }
        if(noError)
        {
            CurrentResearchName = set.name;
            Value = set.value;
            TimeNeededForCurrentResearch = set.timeNeededForCompletion;
            ResearchStarted = true;
        }
    }

    public void CancelResearch() // For UI.
    {
        ResearchStarted = false;
        CurrentResearchName = "-";
        TimeNeededForCurrentResearch = 0.0f;
        Value = 0.0f;
        ResearchProgress = 0.0f;
        currResearchIndex = 0;
    }

    public void AdvanceResearch(float value) // For Laboratory/ies.
    {
        if(ResearchProgress + value > 100)
        {
            ResearchProgress = 100.0f;
            ResearchComplete();
        }
        else 
        {
            ResearchProgress += value;
        }
    }


    private void Initialize()
    {
        ResearchProgress = 0.0f;
        ResearchStarted = false;
        currResearchIndex = 0;
        researchCompleteSound = gameObject.GetComponent<AudioSource>();
    }

    private void ResearchComplete()
    {
        BalancePanel.Instance.GainBenefitsFromResearch(currResearchIndex);
        // Here we should implement the updates of spirits, traps and incomePerSpirit stats.
        CancelResearch();
        if(researchCompleteSound.clip != null)
        {
            researchCompleteSound.Play();
        }
        else
        {
            Debug.Log("Research complete sound hasn't been assigned (ResearchManager.cs).");
        }
    }
}
