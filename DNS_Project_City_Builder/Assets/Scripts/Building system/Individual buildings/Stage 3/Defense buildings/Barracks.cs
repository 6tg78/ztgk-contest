using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Barracks : Building
{
    private List<Enemy> enemies = new List<Enemy>();
    public List<Spirit> Warriors { get; private set; }
    [SerializeField]
    private float ray;
    public float Ray
    {
        get { return ray; }
    }
    [SerializeField]
    private int howManySpiritsToOneEnemy;
    [SerializeField]
    private GameObject spiritTurningIntoWarriorAudioPlayer;

    Barracks()
    {
        BuildingName = "Forest Guard House";
    }

    protected override void Awake()
    {
        Warriors = new List<Spirit>();
        base.Awake();
    }

    protected override void UpdateActions()
    {
        base.UpdateActions();

        if (!InConstructionMode)
        {
            enemies = FindObjectsOfType<Enemy>().ToList();
        }

        if (enemies.Count > 0)
        {
            foreach (Enemy enemy in enemies)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < Ray)
                {
                    SendWarriors();
                }
            }
        }
    }

    protected override void SetWorkType()
    {
        WorkType = WorkTypeEnum.Guarding;
    }

    private void SendWarriors()
    {
        foreach(Spirit warrior in Warriors)
        {
            if(warrior.WarriorIdle)
            {
                warrior.IsVisible = true;
                warrior.WarriorIdle = false;
            }
        }
    }

    public void PlayTurningIntoWarriorSound()
    {
        var player = Instantiate(spiritTurningIntoWarriorAudioPlayer);
        if(player.GetComponent<AudioSource>().clip != null)
        {
            player.GetComponent<AudioSource>().Play();
        }
        else
        {
            Debug.Log("Spirit turning into warrior sound isn't assigned (in Barracks.cs).");
        }
        Destroy(player, 10.0f);
    }
}
