using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    public static TrapManager Instance {get; private set;}

    public int numberOfReadyExplosingTraps {get; private set;}
    public int numberOfReadyStunningTraps {get; private set;}

    private AudioSource trapCraftingCompletedSound;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        Initialize();
    }


    public void TrapCrafted(TrapTypeEnum trapType) // For Workshops.
    {
        if(trapCraftingCompletedSound.clip != null)
        {
            trapCraftingCompletedSound.Play();
        }
        else
        {
            Debug.Log("Trap crafting completed sound hasn't been assigned yet (TrapManager.cs).");
        }
        switch(trapType)
        {
            case TrapTypeEnum.explosing:
                numberOfReadyExplosingTraps++;
                break;
            case TrapTypeEnum.stunning:
                numberOfReadyStunningTraps++;
                break;
            default:
                break;
        }
    }

    public void PlaceTrap(TrapTypeEnum trapType) // For BuildingsManager.
    {
        switch(trapType)
        {
            case TrapTypeEnum.explosing:
                numberOfReadyExplosingTraps--;
                break;
            case TrapTypeEnum.stunning:
                numberOfReadyStunningTraps--;
                break;
            default:
                break;
        }
    }
    
    
    private void Initialize()
    {
        numberOfReadyExplosingTraps = 0;
        numberOfReadyStunningTraps = 0;
        trapCraftingCompletedSound = gameObject.GetComponent<AudioSource>();
    }
}
