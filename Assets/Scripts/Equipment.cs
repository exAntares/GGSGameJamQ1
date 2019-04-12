using System;

public enum EquipmentType {
    Helmet,
    Weapon,
}

[Serializable]
public class Equipment {
    public EquipmentType Type;
    public int ExtraHealth;
    public int ExtraDamage;
    public int ExtraDamageSpeed;
}
