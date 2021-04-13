using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region singleton
    public static EnemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion

    public GameObject enemy;
    public int enemiesMinAmount;
    public int enemiesMaxAmount;
    public List<Enemy> enemies = new List<Enemy>();
    public Transform[] enemyRespawns;

    // ~KS~ I've done some changes here to adjust enemies amount in waves.
    private int nextWaveIncrease = 5;

    private void Update()
    {

    }

    public void EnemyRespawn()
    {
        //int enemiesAmount = Random.Range(enemiesMinAmount, enemiesMaxAmount);         ~KS~ I commented it out for the time being.
        int enemiesAmount = nextWaveIncrease * GameManager.Instance.WavesSurvived + nextWaveIncrease;      // ~KS~ And added something like this.
        int currEnemyRespawn = Random.Range(0, enemyRespawns.Length - 1);

        for (int i = 0; i < enemiesAmount; i++)
        {
            enemies.Add(Instantiate(enemy, enemyRespawns[currEnemyRespawn], false).GetComponent<Enemy>());
        }
    }
}
