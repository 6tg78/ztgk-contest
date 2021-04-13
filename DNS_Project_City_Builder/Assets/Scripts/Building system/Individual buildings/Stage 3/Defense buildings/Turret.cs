using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    public float rotationSpeed;
    private float rotationDirection;
    private bool targetLocked;
    private GameObject lockedTarget;
    [SerializeField]
    private GameObject range;

    public float fireRate;
    public float whenStartShooting;
    public bool aimedTarget;

    [SerializeField] float _missileSpeed;
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

    public float angle;

    private bool isOperative = false;
    public bool IsOperative
    {
        get { return isOperative; }
        set { isOperative = value; if (value) StartUpdatingTarget(); }
    }

    float nearestEnemy = Mathf.Infinity;
    public float repeatingTime = 0.5f;

    private BalancePanel.TurretData stats;

    Turret()
    {
        BuildingName = "Tower";
    }

    protected override void Awake()
    {
        base.Awake();
        targetLocked = false;
        aimedTarget = false;
        stats = BalancePanel.Instance.TurretStats;
        range.transform.localScale = new Vector3(stats.range, stats.range, stats.range);
        SelectionController.OnSelectedBuildingChanged += ShowRange;

    }

    protected override void OnFinishedConstruction()
    {
        base.OnFinishedConstruction();
        IsOperative = false;
        stats = BalancePanel.Instance.TurretStats;

    }

    protected override void SetWorkType()
    {
        WorkType = WorkTypeEnum.Shooting;
    }
    protected override void UpdateActions()
    {
        base.UpdateActions();

        /*if (targetLocked && TargetInRange(lockedTarget)) AimTarget(lockedTarget);
        else
        {
            targetLocked = false;
            aimedTarget = false;
        }
        if (Mathf.Abs(angle) <= whenStartShooting && targetLocked) aimedTarget = true;
        else aimedTarget = false;*/
    }
    private void UpdateTarget()
    {
        nearestEnemy = Mathf.Infinity;

        if (EnemyManager.Instance.enemies.Count == 0)
        {
            aimedTarget = false;
            return;
        }

        foreach (Enemy enemy in EnemyManager.Instance.enemies)
        {
            if (TargetInRange(enemy.gameObject))
            {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance < nearestEnemy)
                {
                    nearestEnemy = distance;
                    targetLocked = true;
                    lockedTarget = enemy.gameObject;
                    aimedTarget = true;
                }
            }
        }
    }

    private void AimTarget(GameObject target) //rotating turret 
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        angle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);
        if (Mathf.Abs(angle) > 1f)
        {
            if (angle > 0f) rotationDirection = -1;
            else rotationDirection = 1;
            transform.Rotate(new Vector3(0, 1, 0) * rotationSpeed * rotationDirection * Time.deltaTime);
        }
    }

    private bool TargetInRange(GameObject target)
    {
        if (target == null || target.gameObject.activeInHierarchy == false) return false;
        else if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= stats.range) return true;
        else return false;
    }

    private void StartUpdatingTarget()
    {
        this.Timer(0.5f, UpdateTarget, () => { return IsOperative; });
        StartCoroutine(ShootTarget(fireRate));
    }

    private IEnumerator ShootTarget(float time)
    {
        while (IsOperative)
        {
            while (aimedTarget)
            {
                if (lockedTarget == null)
                {
                    aimedTarget = false;
                    continue;
                }
                Vector3 dir = lockedTarget.transform.position - MissileSpawnPoint.position;
                FindObjectOfType<ShootingSystem>().Fire(MissileSpawnPoint, GameManager.Instance.SelectedStage3Variant, dir.normalized, _missileSpeed);
                if(shootSound != null)
                {
                    shootSound.Play();
                }
                else
                {
                    Debug.Log("Turret's shootSound isn't assigned.");
                }
                yield return new WaitForSeconds(time);
            }
            yield return null;
        }
    }

    private void OnDisable()
    {
        SelectionController.OnSelectedBuildingChanged -= ShowRange;
    }

    private void ShowRange(bool value)
    {
        range.SetActive(value);
    }
    private void ShowRange(Building building)
    {
        range.SetActive(building == this);
    }
}
