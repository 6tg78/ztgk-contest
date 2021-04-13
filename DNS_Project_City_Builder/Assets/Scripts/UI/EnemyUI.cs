using System;
using UnityEngine;
[RequireComponent(typeof(Enemy))]
public class EnemyUI : MonoBehaviour
{
    [SerializeField] GameObject _healthBarPrefab;
    private ProgressBar _healthBar;
    private GameObject _healthBarInstance;
    private Enemy _enemy;
    public void Bind(Enemy enemy)
    {
        _enemy = enemy;
        CreateProgressBarInstance(_healthBarPrefab, out _healthBarInstance, out _healthBar);
        _enemy.OnHitPointsChanged += HandleHealthBar;
    }

    public void Unbind()
    {
        _enemy.OnHitPointsChanged -= HandleHealthBar;
        _healthBar = null;
        Destroy(_healthBarInstance);
    }

    private void Update()
    {
        if (_healthBar != null)
        {
            TranslateProgressBar(_healthBarInstance, 5f);
        }
    }

    private void TranslateProgressBar(GameObject healthBar, float offset)
    {
        var enemyPosition = _enemy.gameObject.transform.position;
        enemyPosition.y += offset;
        var position = Camera.main.WorldToScreenPoint(enemyPosition);
        healthBar.transform.position = position;
    }
    private void HandleHealthBar()
    {
        if (_healthBar == null)
            return;

        if (Mathf.Approximately(_enemy.HitPoints, _enemy.MaxHitPoints))
            _healthBarInstance.SetActive(false);
        else
            _healthBarInstance.SetActive(true);

        var health = _enemy.HitPoints / _enemy.MaxHitPoints;
        _healthBar.Progress = health;
        _healthBar.Label = string.Format("Health: {0}", _enemy.HitPoints);
    }

    private void CreateProgressBarInstance(GameObject prefab, out GameObject instance, out ProgressBar bar)
    {
        instance = Instantiate(prefab, UIManager.Instance.ScreenSpaceCanvas.transform);
        instance.transform.SetSiblingIndex(0);
        bar = instance.GetComponent<ProgressBar>();
    }
}