using System.Collections.Generic;
using HalfBlind.ScriptableVariables;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private EnemyController[] _enemyPrefabs;
    [SerializeField] private DamageFeedback _damageFeedbackPrefab;
    [SerializeField] private GlobalFloat _currentLevel;
    [SerializeField] private ScriptableGameEvent _onAllEnemiesDied;
    [SerializeField] private int _healthFactor = 5;

    private int _enemiesDead;
    private int _spawnedEnemies;
    private List<EnemyController> _allEnemies;

    private void Awake() {
        EnemyController.OnEnemyDamaged += OnEnemyDamaged;
        _spawnedEnemies = 0;
        _enemiesDead = 0;

        _allEnemies = new List<EnemyController>();

        for (int i = 0; i < _currentLevel.Value + 1; i++) {
            _spawnedEnemies++;
            var prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            var instance = Instantiate(prefab);
            var targetPos = new Vector3(Random.Range(30, 60), Random.Range(0, 30), 0);
            instance.transform.position = targetPos;
            instance.OnEnemydied += OnEnemyDied;
            var healthComp = instance.GetComponent<HealthComponent>();
            healthComp.MaxHealth = ((int)_currentLevel.Value + 1) * _healthFactor;
            healthComp.Health = healthComp.MaxHealth;
            _allEnemies.Add(instance);
        }
    }

    private void OnDestroy() {
        EnemyController.OnEnemyDamaged -= OnEnemyDamaged;
    }

    private void OnEnemyDamaged(EnemyController obj, int damage) {
        var instance = Instantiate(_damageFeedbackPrefab);
        instance.transform.position = obj.transform.position;
        instance.SetText($"{damage}");
    }

    private void OnEnemyDied() {
        _enemiesDead++;
        Debug.Log($"EnemyDied, enemies left {_spawnedEnemies - _enemiesDead}");
        if (_enemiesDead == _spawnedEnemies) {
            _onAllEnemiesDied.SendEvent();
        }
    }

    public void TakeDamage(Vector3 pos, int damage, float areaOfEffect) {
        for (int i = 0; i < _allEnemies.Count; i++) {
            var currentEnemy = _allEnemies[i];
            var distance = pos - currentEnemy.transform.position;
            if(distance.magnitude <= areaOfEffect) {
                currentEnemy.TakeDamage(damage);
            }
        }
    }
}
