namespace HalfBlind.ScriptableVariables {
    using HalfBlind.Attributes;
    using HalfBlind.SaveUtils;
    using UnityEngine;

    public abstract class GlobalSavedFloat : GlobalFloat {
        [UniqueIdentifier]
        [SerializeField]
        private string _saveKey = null;

        public abstract ISave GetSaveHandler();

        public override float Value {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return _initialValue;
                }
#endif
                var result = _initialValue;
                var saveHandler = GetSaveHandler();
                if (saveHandler != null && saveHandler.Load<float>(_saveKey, out result)) {
                    return result;
                }

                return _initialValue;
            }
            set {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return;
                }
#endif
                var _saveSystem = GetSaveHandler();
                if (_saveSystem != null) {
                    if(Value != value) {
                        _saveSystem.Save<float>(_saveKey, value);
                        OnTValueChanged?.Invoke(value);
                        OnValueChanged?.Invoke();
                    }
                }
            }
        }
    }
}
