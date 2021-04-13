using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Magda, I know that there is some serious copy-pasting here, but that was the fastest way to implement traps placing. Forgive me please :(
public class Trap : MonoBehaviour
{
    public TrapTypeEnum trapType;
    public Collider activationArea, effectArea; // Set in inspector.
    
    [SerializeField]
    private AudioSource sound;

    [HideInInspector]
    public new string name;
    [HideInInspector]
    public string description;
    [HideInInspector]
    public float timeToCraft;
    public float ValueOfEffect {private get; set;}
    [HideInInspector]
    public ResourcePack cost;
    public bool InConstrucionPlanningMode {get; set;}

    private int collisionCount = 0; // This is for trap PLACING purposes.
    private List<Enemy> enemiesInRange = new List<Enemy>();

    Trap()
    {

    }
    private void Awake()
    {
        InConstrucionPlanningMode = false;
        BalancePanel.Instance.BalanceUpdate(this);
    }

    private void Update()
    {
        if (InConstrucionPlanningMode)
        {
            if(collisionCount == 0)
            {
                BuildingsManager.Instance.PlacementPossible = true;
                ChangeShader(Shader.Find("Custom/HologramBlue"));
            }
            else
            {
                BuildingsManager.Instance.PlacementPossible = false;
                ChangeShader(Shader.Find("Custom/HologramRed"));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionCount++;
        var enemy = (other.gameObject.GetComponent<Enemy>());
        if(enemy != null)
        {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount--;
        var enemy = (other.gameObject.GetComponent<Enemy>());
        if(enemy != null)
        {
            enemiesInRange.Remove(enemy);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var enemy = (collision.gameObject.GetComponent<Enemy>());
        if(enemy != null)
        {
            foreach(var affected in enemiesInRange)
            {
                TrapEffect(affected);
            }
            // Some particles here or something?
            Destroy(gameObject);
        }
    }


    public void ChangeShader(Shader arg)
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach(var renderer in renderers)
        {
            renderer.material.shader = arg;
        }
    }


    private void TrapEffect(Enemy affected)
    {
        if(sound.clip != null)
        {
            sound.Play();
        }
        else
        {
            Debug.Log("One of the traps doesn't have a sound hooked.");
        }
        switch(trapType)
        {
            case TrapTypeEnum.explosing:
                affected.GetDamage(ValueOfEffect);
                break;
            case TrapTypeEnum.stunning:
                affected.BecomeStunned(ValueOfEffect);
                break;
            default:
                break;
        }
    }
}

public enum TrapTypeEnum
    {
        explosing = 0,
        stunning = 1
    }
