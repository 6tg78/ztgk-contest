using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : Building
{
    private static bool notificationTriggeredOnFirstShelterBuilt = false;
    private int amountAdded;
    private bool constructed = false;
    [SerializeField]
    private GameObject spiritEnteringShelterAudioPlayer;


    Shelter()
    {
        BuildingName = "Shelter";
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnFinishedConstruction()
    {
        base.OnFinishedConstruction();
        constructed = true;
        AIManager.Instance.MaxPossbileSpirits += MaxSpirits;
        amountAdded = MaxSpirits;
        if (notificationTriggeredOnFirstShelterBuilt == false)
        {
            notificationTriggeredOnFirstShelterBuilt = true;
            NotificationManager.Instance.AddNotification("Growing up", "Some of your spirits already have a roof above their heads, but your grove is still very small. Try increasing your ranks. The Bloom has to persist...", false);
        }
    }
    protected override void SetWorkType()
    {
        WorkType = WorkTypeEnum.BeingInHome;
    }

    public void UpdateSpiritsMaxAmount()
    {
        AIManager.Instance.MaxPossbileSpirits += (MaxSpirits - amountAdded);
    }

    public override void Destroyed()
    {
        if (constructed)
            AIManager.Instance.MaxPossbileSpirits -= MaxSpirits;
        base.Destroyed();
    }

    public void PlayEnteringShelterSound()
    {
        var player = Instantiate(spiritEnteringShelterAudioPlayer);
        if (player.GetComponent<AudioSource>().clip != null)
        {
            player.GetComponent<AudioSource>().Play();
        }
        else
        {
            Debug.Log("Spirit entering shelter sound isn't assigned (in Shelter.cs).");
        }
        Destroy(player, 10.0f);
    }
}
