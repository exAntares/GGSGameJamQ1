using UnityEngine;
using XNode;

namespace HalfBlind.Nodes {
    [CreateNodeMenu("GameJam/" + nameof(GetEquipmentData))]
    public class GetEquipmentData : MonoNode {
        [Input] public ScriptableEquipment Equipment;
        [Output] public float Health;
        [Output] public float Damage;
        [Output] public Sprite Image;

        public override object GetValue(NodePort port) {
            if (port.fieldName == nameof(Health)) {
                var input = GetInputValue<ScriptableEquipment>(nameof(Equipment), Equipment);
                return input?.value.ExtraHealth;
            }

            if (port.fieldName == nameof(Damage)) {
                var input = GetInputValue<ScriptableEquipment>(nameof(Equipment), Equipment);
                return input?.value.ExtraDamage;
            }

            if (port.fieldName == nameof(Image)) {
                var input = GetInputValue<ScriptableEquipment>(nameof(Equipment), Equipment);
                return input?.value.Graphics;
            }

            return null;
        }
    }
}
