using System;
using UnityEngine;

public enum EquipmentType {
    Helmet,
    Weapon,
}

[Serializable]
public class Equipment {
    public EquipmentType Type;
    public Sprite Graphics;
    public Color Tint = Color.white;
    public int ExtraHealth;
    public int ExtraDamage;
    public int ExtraDamageSpeed;
}
