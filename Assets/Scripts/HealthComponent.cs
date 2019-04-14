using HalfBlind.ScriptableVariables;
using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour {
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private GlobalFloat _currentGlobalHealth;
    [SerializeField] private GlobalFloat _maxGlobalHealth;
    [SerializeField] private GlobalListFloat _damageTaken;
    [SerializeField] private ScriptableGameEvent _onDamageTaken;
    [SerializeField] private ScriptableGameEvent _onDied;

    public event Action OnEnemyDied;
    private int _currentHealth;

    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    public int Health {
        get => _currentHealth;
        set {
            _currentHealth = Mathf.Min(value, MaxHealth);
            if (_currentGlobalHealth) {
                _currentGlobalHealth.Value = _currentHealth;
            }
        }
    }

    private void Update() {
        if (_maxGlobalHealth && _maxGlobalHealth.Value != MaxHealth) {
            _maxGlobalHealth.Value = MaxHealth;
        }
    }

    public void Reset() {
        _currentHealth = MaxHealth;
    }

    public bool TakeDamage(int v) {
        if(Health <= 0) {
            return false;
        }
        Health -= v;
        Health = Health <= 0 ? 0 : Health;
        Debug.Log($"{name} Taking {v} damage, new Health {Health}");
        _damageTaken?.Value.Add(v);
        _onDamageTaken?.SendEvent();
        if (Health <= 0) {
            _onDied?.SendEvent();
            OnEnemyDied?.Invoke();
        }

        return true;
    }
}
