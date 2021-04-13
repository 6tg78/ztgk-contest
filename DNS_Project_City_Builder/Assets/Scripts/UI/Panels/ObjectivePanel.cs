using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// TODO: Refactor once we have objective manager
public class ObjectivePanel : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI _descriptionLabel;
    [SerializeField]
    public TextMeshProUGUI _progressLabel;

    public static int targetGoal = 10;

    public Action OnGoalReached;

    public string DescriptionString { get { return _descriptionLabel.text; } set { _descriptionLabel.text = value; } }
    public string ProgressString { get { return _progressLabel.text; } set { _progressLabel.text = value; } }

    void Start()
    {
        StartCoroutine(SpiritsCountObjective());
    }

    IEnumerator SpiritsCountObjective()
    {
        DescriptionString = String.Format("Have {0} spirits.", targetGoal);
        var count = AIManager.Instance.spirits.Count;
        while(count < targetGoal)
        {
            count = AIManager.Instance.spirits.Count;
            ProgressString = String.Format("{0}/{1}", count, targetGoal);
            yield return new WaitForSeconds(1.0f);
        }
        OnGoalReached?.Invoke();
        StartCoroutine(WaveCountObjective());
    }

    IEnumerator WaveCountObjective()
    {
        targetGoal = 5;
        DescriptionString = String.Format("Survive {0} waves.", targetGoal);
        var count = GameManager.Instance.WavesSurvived;
        while(count < targetGoal)
        {
            count = GameManager.Instance.WavesSurvived;
            ProgressString = String.Format("{0}/{1}", count, targetGoal);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
