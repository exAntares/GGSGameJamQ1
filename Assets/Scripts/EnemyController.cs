using HalfBlind.ScriptableVariables;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public enum States {
        Alive,
        Death,
    }

    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private Vector3 _offset = Vector2.one;
    [SerializeField] private float _radious = 2;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeedSeconds = 1;
    [SerializeField] private GlobalBoolean _isGamePaused;

    public event Action OnEnemydied;
    public static event Action<EnemyController, int> OnEnemyDamaged;

    private HeroController _player;
    private HealthComponent _healthComp;
    private Animator _animator;
    private float _lastAttack;
    private States _currentState;

    private void Awake() {
        _player = FindObjectOfType<HeroController>();
        _healthComp = GetComponent<HealthComponent>();
        _healthComp.OnEnemyDied += OnEnemyDiedHandler;
        _animator = GetComponentInChildren<Animator>();
        _currentState = States.Alive;
    }

    private void OnEnemyDiedHandler() {
        _currentState = States.Death;
        _animator.Play("Dead");
        OnEnemydied?.Invoke();
        DisableGameObjectAsync();
    }

    private async void DisableGameObjectAsync() {
        await Task.Delay(300);
        gameObject.SetActive(false);
    }

    private void Update() {
        if (_isGamePaused.Value) {
            return;
        }

        switch (_currentState) {
            case States.Alive:
                UpdateAlive();
                break;
            case States.Death:
                UpdateDeath();
                break;
            default:
                break;
        }
    }

    private void UpdateDeath() {
        var position = transform.position;
        var targetPosition = transform.position;
        targetPosition.y = 0;
        position = Vector3.MoveTowards(position, targetPosition, _speed);
        transform.position = position;
    }

    private void UpdateAlive() {
        var position = transform.position;
        var targetPosition = _player.transform.position + _offset;
        var direction = (targetPosition - position).normalized;
        position = Vector3.MoveTowards(position, targetPosition - direction * _radious, _speed);
        transform.position = position;

        if (Vector3.SqrMagnitude(transform.position - targetPosition) < _attackRange) {
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
        if (_healthComp.TakeDamage(damage)) {
            OnEnemyDamaged?.Invoke(this, damage);
            if (_currentState != States.Death) {
                _animator.Play("OnHit");
            }
        }
    }
}
