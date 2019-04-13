using System.Collections.Generic;
using HalfBlind.ScriptableVariables;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private EnemyController[] _enemyPrefabs;
    [SerializeField] private GlobalFloat _currentLevel;
    [SerializeField] private ScriptableGameEvent _onAllEnemiesDied;
    [SerializeField] private int _healthFactor = 5;

    private int _enemiesDead;
    private int _spawnedEnemies;
    private List<EnemyController> _allEnemies;

    private void Awake() {
        _spawnedEnemies = 0;
        _enemiesDead = 0;

        _allEnemies = new List<EnemyController>();

        for (int i = 0; i < _currentLevel.Value + 1; i++) {
            for (int j = 0; j < _enemyPrefabs.Length; j++) {
                _spawnedEnemies++;
                var targetPos = new Vector3(UnityEngine.Random.Range(30, 60), UnityEngine.Random.Range(0, 30), 0);
                var instance = Instantiate(_enemyPrefabs[j]);
                instance.transform.position = targetPos;
                instance.OnEnemydied += OnEnemyDied;
                var healthComp = instance.GetComponent<HealthComponent>();
                healthComp.MaxHealth = ((int)_currentLevel.Value + 1) * _healthFactor;
                healthComp.Health = healthComp.MaxHealth;
                _allEnemies.Add(instance);
            }
        }
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
