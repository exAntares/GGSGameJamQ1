using UnityEngine;

public class HealthComponent : MonoBehaviour {
    [SerializeField] private int _maxHealth = 0;
    private int _currentHealth;
    private EquipmentComponent _equipmentComponent;

    public int MaxHealth => _maxHealth + _equipmentComponent.GetExtraHealth();

    public int Health {
        get => _currentHealth;
        set => _currentHealth = Mathf.Max(value, MaxHealth);
    }

    private void Awake() {
        _equipmentComponent = GetComponent<EquipmentComponent>();
        _currentHealth = MaxHealth;
    }

    public void Reset() {
        _currentHealth = MaxHealth;
    }
}
