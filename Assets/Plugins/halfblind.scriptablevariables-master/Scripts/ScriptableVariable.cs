namespace HalfBlind.ScriptableVariables {
    using System;
    using UnityEngine;

    public abstract class ScriptableVariable : ScriptableObject {
        public Action OnValueChanged;
        public abstract object GetValue();
        public abstract void FromString(string value);
    }

    public abstract class ScriptableVariable<T> : ScriptableVariable {
        public Action<T> OnTValueChanged;

        [SerializeField, Sirenix.OdinInspector.HideInPlayMode]
        internal T _initialValue;

        [NonSerialized] protected ScriptableVariable<T> _runtimeInstance;

        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.HideInEditorMode]
        public virtual T Value {
            get { return _runtimeInstance._initialValue; }
            set {
                _runtimeInstance._initialValue = value;
                OnTValueChanged?.Invoke(value);
                OnValueChanged?.Invoke();
            }
        }

        protected virtual void OnEnable() {
            _runtimeInstance = this;
#if UNITY_EDITOR
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(assetPath)) {
                _runtimeInstance = Instantiate(this);
            }
#endif
        }

        public override object GetValue() {
            return Value;
        }
    }
}
