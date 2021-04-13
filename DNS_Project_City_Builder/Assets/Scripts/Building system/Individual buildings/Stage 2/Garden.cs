using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garden : Building
{
    public GameObject Layer1, Layer2, Layer3;
    
    private GameObject currentLayer;
    private bool gardenIsGrowing = false, gardenFullyGrown = false;
    private int timeCounter = 0;
    private const int t1Time = 120, t2Time = 300, t3Time = 500, timeToGrowLayer = 20;


    Garden()
    {
        BuildingName = "Flower meadow";
    }

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine("WaitForConstruction");
    }

    protected override void Update()
    {
        base.Update();
        if(gardenIsGrowing)
        {
            Vector3 pos = currentLayer.transform.position;
            float step = Time.deltaTime * (0.2f / timeToGrowLayer); // 0.5f is the total distance that flowers have to move from the undergroud to the surface.
            if(pos.y + step < 0.0f)
            {
                pos.y += step;
            }
            else
            {
                pos.y = 0.0f;
                gardenIsGrowing = false;
                if(currentLayer == Layer3)
                {
                    gardenFullyGrown = true;
                }
            }
            currentLayer.transform.position = pos;
        }
    }

    public IEnumerator MeasureTime()
    {
        while(true)
        {
            if(gardenFullyGrown == false)
            {
                switch(timeCounter)
                {
                    case t1Time:
                        gardenIsGrowing = true;
                        currentLayer = Layer1;
                        break;
                    case t2Time:
                        gardenIsGrowing = true;
                        currentLayer = Layer2;
                        break;
                    case t3Time:
                        gardenIsGrowing = true;
                        currentLayer = Layer3;
                        break;
                    default:
                        break;
                }
                timeCounter++;
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield break;
            }
        }
    }

    private IEnumerator WaitForConstruction()
    {
        while(true)
        {
            if(IsOperating)
            {
                StartCoroutine("MeasureTime");
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
