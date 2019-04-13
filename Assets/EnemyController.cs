﻿using System;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private Vector3 _offset = Vector2.one;
    [SerializeField] private float _radious = 2;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeedSeconds = 1;

    public event Action OnEnemydied;

    private HeroController _player;
    private HealthComponent _healthComp;
    private float _lastAttack;

    private void Awake() {
        _player = FindObjectOfType<HeroController>();
        _healthComp = GetComponent<HealthComponent>();
        _healthComp.OnEnemyDied += () => OnEnemydied?.Invoke();
    }

    private void Update() {
        var position = transform.position;
        var targetPosition = _player.transform.position + _offset;
        var direction = (targetPosition - position).normalized;
        position = Vector3.MoveTowards(position, targetPosition - direction*_radious, _speed);
        transform.position = position;

        if(Vector3.SqrMagnitude(transform.position - targetPosition) < _attackRange) {
            Attack();
        }
    }

    private void Attack() {
        if(Time.time - _lastAttack > _attackSpeedSeconds) {
            _lastAttack = Time.time;
            _player.TakeDamage(UnityEngine.Random.Range(1, 3));
            Debug.Log("Attacking player!");
        }
    }

    public void TakeDamage(int damage) {
        _healthComp.TakeDamage(damage);
    }
}
