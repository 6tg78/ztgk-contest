using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : Building
{
    public Animator Animator { get; set; }
    Storage()
    {
        BuildingName = "Shed";
    }

    protected override void Awake()
    {
        base.Awake();
        Animator = gameObject.GetComponentInChildren<Animator>();
    }

    protected override void OnFinishedConstruction()
    {
        base.OnFinishedConstruction();
        StorageManager.Instance.IncreaseLimits();
        Animator.SetBool("storage", true);
    }

    public override void ChangeModel(int arg)
    {
        base.ChangeModel(arg);

        switch (arg)
        {
            case 0:
                Animator = stage2model.GetComponent<Animator>();
                break;
            case 1:
                Animator = stage3variant1model.GetComponent<Animator>();
                break;
            case 2:
                Animator = stage3variant2model.GetComponent<Animator>();
                break;
        }

        Animator.SetBool("storage", true);

    }

}