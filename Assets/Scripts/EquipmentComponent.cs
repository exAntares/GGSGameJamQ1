using HalfBlind.ScriptableVariables;
using System;
using UnityEngine;

public class EquipmentComponent : MonoBehaviour {
    public Vector2 RandomRangeNewEquipment = new Vector2(1, 6);
    public ScriptableGameEvent _onKilledAllEnemies;
    public ScriptableEquipment _hat;
    public ScriptableEquipment _weapon;

    public Sprite[] _possibleHats;
    public Sprite[] _possibleWeapons;
    public SpriteRenderer _hatRenderer;
    public SpriteRenderer _weaponRenderer;
    public SpriteRenderer _trail;

    public event Action<ScriptableEquipment> OnCreatedNewItem;

    private void Awake() {
        _onKilledAllEnemies.AddListener(OnKilledAllEnemies);
        _hatRenderer.sprite = _hat.value.Graphics;
        _hatRenderer.color = _hat.value.Tint;
        _weaponRenderer.sprite = _weapon.value.Graphics;
        _weaponRenderer.color = _weapon.value.Tint;
        _trail.color = _weapon.value.Tint;
    }

    private void OnDestroy() {
        _onKilledAllEnemies.RemoveListener(OnKilledAllEnemies);
    }

    private void OnKilledAllEnemies() {
        var equipmentType = (EquipmentType)UnityEngine.Random.Range(0, 2);
        Sprite sprite;
        ScriptableEquipment newItem = null;
        switch (equipmentType) {
            case EquipmentType.Helmet:
                sprite = _possibleHats[UnityEngine.Random.Range(0, _possibleHats.Length)];
                _hat.value.Graphics = sprite;
                _hat.value.ExtraHealth += Mathf.FloorToInt(UnityEngine.Random.Range(RandomRangeNewEquipment.x, RandomRangeNewEquipment.y));
                newItem = _hat;
                break;
            case EquipmentType.Weapon:
                sprite = _possibleWeapons[UnityEngine.Random.Range(0, _possibleWeapons.Length)];
                _weapon.value.Graphics = sprite;
                _weapon.value.ExtraDamage += Mathf.FloorToInt(UnityEngine.Random.Range(RandomRangeNewEquipment.x, RandomRangeNewEquipment.y)); ;
                newItem = _weapon;
                break;
            default:
                break;
        }

        newItem.value.Tint = UnityEngine.Random.ColorHSV();
        OnCreatedNewItem?.Invoke(newItem);
    }

    public int GetExtraHealth() {
        var accumulated = 0;
        accumulated += _hat.value.ExtraHealth;
        accumulated += _weapon.value.ExtraHealth;
        return accumulated;
    }

    public int GetExtraDamage() {
        var accumulated = 0;
        accumulated += _hat.value.ExtraDamage;
        accumulated += _weapon.value.ExtraDamage;
        return accumulated;
    }
}
