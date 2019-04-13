using System.Collections.Generic;
using UnityEngine;

namespace HalfBlind.ScriptableVariables {
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GlobalListFloat))]
    public class GlobalListFloatEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var list = (target as GlobalListFloat).Value;
            for (int i = 0; i < list.Count; i++) {
                UnityEditor.EditorGUILayout.LabelField($"Item{i:00}: {list[i]}");
            }
        }
    }
#endif

    [CreateAssetMenu(fileName = "GlobalListFloat", menuName = "Variables/List/Float")]
    public class GlobalListFloat : ScriptableVariable<List<float>> {
        [SerializeField] private string _separator = ";";

        public override void FromString(string value) {
            var split = value.Split(new string[] { _separator }, System.StringSplitOptions.None);
            Value = new List<float>(split.Length);
            for (int i = 0; i < split.Length; i++) {
                float result;
                if(float.TryParse(split[i], out result)) {
                    Value.Add(result);
                }
            }
        }
    }
}
