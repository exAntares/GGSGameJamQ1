using UnityEngine;

public class ChestImageSetter : MonoBehaviour {
    public SpriteRenderer Item;
    public EquipmentComponent HeroEquipmentComp;

    private void Awake() {
        HeroEquipmentComp.OnCreatedNewItem += OnCreatedNewItem;
    }

    private void OnDestroy() {
        HeroEquipmentComp.OnCreatedNewItem -= OnCreatedNewItem;
    }

    private void OnCreatedNewItem(ScriptableEquipment newItem) {
        Item.sprite = newItem.value.Graphics;
        Item.color = newItem.value.Tint;
    }
}
