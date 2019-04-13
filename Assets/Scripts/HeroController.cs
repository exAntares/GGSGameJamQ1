﻿using UnityEngine;

public class HeroController : MonoBehaviour {
    [SerializeField] private float _gravity = 8.9f;
    [SerializeField] private Vector2 _minMaxX;
    [SerializeField] private Vector2 _minMaxY;
    [SerializeField] private float _attackSpeedSeconds = 1;
    [SerializeField] private float _areaOfAttack = 1;
    [SerializeField] private Vector3 _areaOfAttackOffset;

    private Vector2 Impulse = Vector2.zero;
    private float lastSign = 1;
    private EnemySpawner _enemies;
    private HealthComponent _healthComp;
    private float _lastAttack;
    

    private void Awake() {
        _enemies = FindObjectOfType<EnemySpawner>();
        _healthComp = GetComponent<HealthComponent>();
        var equipments = GetComponent<EquipmentComponent>();
        _healthComp.MaxHealth += equipments.GetExtraHealth();
        _healthComp.Health = _healthComp.MaxHealth;
    }

    private void FixedUpdate() {
        if (Input.GetButton("Fire")) {
            Attack();
        }

        if (Input.GetButton("Jump")) {
            Impulse = Vector2.up;
        }

        var horizontal = Input.GetAxis("Horizontal");
        if(horizontal != 0) {
            var localScale = transform.localScale;
            localScale.x = Mathf.Sign(horizontal);
            transform.localScale = localScale;
        }
        Impulse.x = horizontal;

        var currentPos = transform.position;
        currentPos += new Vector3(Impulse.x, Impulse.y);
        Impulse *= 0.5f;

        currentPos.y -= _gravity * 0.01f;
        currentPos.y = currentPos.y <= _minMaxY.x ? _minMaxY.x : currentPos.y >= _minMaxY.y ? _minMaxY.y : currentPos.y;
        currentPos.x = currentPos.x <= _minMaxX.x ? _minMaxX.x : currentPos.x >= _minMaxX.y ? _minMaxX.y : currentPos.x;
        transform.position = currentPos;
    }

    private void Attack() {
        if (Time.time - _lastAttack > _attackSpeedSeconds) {
            _lastAttack = Time.time;
            Debug.Log("Player Attacked!");
            var actualOffset = _areaOfAttackOffset;
            actualOffset.x *= transform.localScale.x;
            _enemies.TakeDamage(transform.position + actualOffset, 5, _areaOfAttack);
        }
    }

    public void TakeDamage(int damage) {
        _healthComp.TakeDamage(damage);
    }

    private void OnDrawGizmos() {
        var actualOffset = _areaOfAttackOffset;
        actualOffset.x *= transform.localScale.x;
        UnityEngine.Gizmos.DrawWireSphere(transform.position + actualOffset, _areaOfAttack);
    }
}
