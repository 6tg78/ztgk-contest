using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongRangeTurret : Building
{
    private LayerMask terrainLayerMask;

    public GameObject attackPlace;
    public float fireRate;
    [SerializeField]
    private float offset;
    [SerializeField]
    private bool isOperative = false;

    [SerializeField]
    private Transform missileSpawnPointStage2;
    [SerializeField]
    private Transform missileSpawnPointStage3_1;
    [SerializeField]
    private Transform missileSpawnPointStage3_2;
    [SerializeField]
    private AudioSource shootSound;


    private Transform MissileSpawnPoint
    {
        get
        {
            if (GameManager.Instance.CurrentStage == 2) return missileSpawnPointStage2;
            else
            {
                if (GameManager.Instance.SelectedStage3Variant == 1)
                    return missileSpawnPointStage3_1;
                else
                    return missileSpawnPointStage3_2;
            }
        }
    }

    public bool IsOperative
    {
        get { return isOperative; }
        set
        {
            if (value && isOperative != true)
            {
                isOperative = value;
                StartCoroutine(Shoot(fireRate));
                Debug.Log("shooting");
            }
            isOperative = value;

        }
    }


    private BalancePanel.LRTurretData stats;

    LongRangeTurret()
    {
        BuildingName = "Mortar";
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void UpdateActions()
    {
        base.UpdateActions();

        if (EnemyManager.Instance.enemies.Count > 0)
            IsOperative = true;
        else
            IsOperative = false;
    }
    protected override void OnFinishedConstruction()
    {
        base.OnFinishedConstruction();
        IsOperative = false;
        SelectionController.OnSelectedBuildingChanged += ShowAttackPlace;
        UpdateStatistics();
    }
    private IEnumerator ISetAttackPlace() // for setting where our turret deals damage --needs button in UI
    {
        attackPlace.SetActive(true);
        bool settingAttackPlace = true;

        while (settingAttackPlace)
        {
            terrainLayerMask = LayerMask.GetMask("Terrain");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f, terrainLayerMask))
            {
                attackPlace.transform.position = hitInfo.point;
            }
            if (Input.GetButtonDown("Fire1"))
            {
                settingAttackPlace = false;
                attackPlace.SetActive(false);
            }
            yield return null;//new WaitForEndOfFrame();
        }
    }

    public void SetAttackPlace()
    {
        StartCoroutine(ISetAttackPlace());
    }
    private IEnumerator Shoot(float time)
    {
        while (IsOperative)
        {
            //DealDamage();
            Vector3 dir = attackPlace.transform.position - transform.position;
            dir.y = Vector3.Distance(transform.position, attackPlace.transform.position) * Mathf.Sqrt(3);//degree of 60

            float speed = (Mathf.Sqrt((Vector3.Distance(transform.position, attackPlace.transform.position) + offset) * -Physics.gravity.y / Mathf.Sin(2.0943951f)));
            FindObjectOfType<ShootingSystem>().Fire(MissileSpawnPoint, GameManager.Instance.SelectedStage3Variant, dir, speed, true);
            if(shootSound.clip != null)
            {
                shootSound.Play();
            }
            else                {
                Debug.Log("Long range turret's shootSound isn't assigned.");
            }
            yield return new WaitForSeconds(time);
        }
    }

    private void DealDamage()//deprecated xd
    {
        List<Enemy> enemies = new List<Enemy>(FindObjectsOfType<Enemy>());

        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, attackPlace.transform.position) <= stats.areaOfEffect) this.Delay(2f/*animation time*/, () => enemy.GetDamage(stats.turretParams.dmgPerHit));
        }
    }
    public void UpdateStatistics()
    {
        stats = BalancePanel.Instance.LongRangeTurretStats;
    }

    private void OnDisable()
    {
        SelectionController.OnSelectedBuildingChanged -= ShowAttackPlace;
    }
    private void ShowAttackPlace(Building building)
    {
        attackPlace.SetActive(building == this);
    }
}