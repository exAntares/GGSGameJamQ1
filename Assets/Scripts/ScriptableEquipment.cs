using UnityEngine;

[CreateAssetMenu(fileName = nameof(ScriptableEquipment), menuName = "GameJam/"+ nameof(ScriptableEquipment))]
public class ScriptableEquipment : ScriptableObject {
    [SerializeField] private Equipment _value;
    private ScriptableEquipment _runtimeInstance;

    public Equipment value {
        get => _runtimeInstance._value;
    }

    private void OnEnable() {
        _runtimeInstance = this;
#if UNITY_EDITOR
        var assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        if (!string.IsNullOrEmpty(assetPath)) {
            _runtimeInstance = Instantiate<ScriptableEquipment>(this);
        }
#endif
    }
}
