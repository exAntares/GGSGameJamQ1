using System.Collections.Generic;
using UnityEngine;

public class EquipmentComponent : MonoBehaviour {
    public List<Equipment> Equipments;

    public int GetExtraHealth() {
        var accumulated = 0;
        for (int i = 0; i < Equipments.Count; i++) {
            accumulated += Equipments[i].ExtraHealth;
        }
        return accumulated;
    }

    public void AddEquipment(Equipment newEquip) {
        for (int i = 0; i < Equipments.Count; i++) {
            if(Equipments[i].Type == newEquip.Type) {
                Equipments[i] = newEquip;
                return;
            }
        }
        Equipments.Add(newEquip);
    }
}
