namespace HalfBlind.ScriptableVariables {
    using HalfBlind.Attributes;
    using HalfBlind.SaveUtils;
    using UnityEngine;

    public abstract class GlobalSavedBoolean : GlobalBoolean {
        [UniqueIdentifier]
        [SerializeField]
        private string _saveKey = null;

        public abstract ISave GetSaveHandler();

        public override bool Value {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return _initialValue;
                }
#endif
                var result = _initialValue;
                var savehandler = GetSaveHandler();
                if (savehandler != null && savehandler.Load<bool>(_saveKey, out result)) {
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
                        _saveSystem.Save<bool>(_saveKey, value);
                        OnTValueChanged?.Invoke(value);
                        OnValueChanged?.Invoke();
                    }
                }
            }
        }
    }
}
