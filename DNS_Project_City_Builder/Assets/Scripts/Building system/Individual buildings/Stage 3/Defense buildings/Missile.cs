using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Rigidbody Rb { get; private set; }

    [SerializeField]
    private AudioSource soundOfHittingTheGround, soundOfHittingEnemy;
    [SerializeField]
    private GameObject physicalPart;
    public bool AoE { get; set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!AoE && other.gameObject.transform.TryGetComponent(out Enemy component))
        {
            component.GetDamage(BalancePanel.Instance.TurretStats.dmgPerHit);
            MissileHit(soundOfHittingEnemy);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(AoE)
            DealAOEDamage();
        if(other.collider.gameObject.layer == 8)
        {
            MissileHit(soundOfHittingTheGround);
        }
    }

    private void MissileHit(AudioSource soundSource)
    {
        if(soundSource.clip != null)
        {
            soundSource.Play();
        }
        else
        {
            Debug.Log("Some sound in Missile.cs isn't assigned.");
        }
        GetComponent<Collider>().enabled = false;
        physicalPart.SetActive(false);
        Destroy(gameObject, 3.0f);
    }

    private void DealAOEDamage()
    {
        foreach(Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if(Vector3.Distance(enemy.gameObject.transform.position, gameObject.transform.position) <= BalancePanel.Instance.LongRangeTurretStats.areaOfEffect)
            {
                enemy.GetDamage(BalancePanel.Instance.LongRangeTurretStats.turretParams.dmgPerHit);
            }
        }
    }
}