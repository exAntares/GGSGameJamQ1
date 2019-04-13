using HalfBlind.ScriptableVariables;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private EnemyController[] _enemyPrefabs;
    [SerializeField] private GlobalFloat _currentLevel;

    private void Awake() {
        for (int i = 0; i < _currentLevel.Value + 1; i++) {
            for (int j = 0; j < _enemyPrefabs.Length; j++) {
                Instantiate(_enemyPrefabs[j]);
            }
        }
    }
}
