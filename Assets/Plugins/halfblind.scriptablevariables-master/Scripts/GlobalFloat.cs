namespace HalfBlind.ScriptableVariables {
    using UnityEngine;

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GlobalFloat))]
    public class GlobalFloatEditor: UnityEditor.Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            UnityEditor.EditorGUILayout.LabelField($"CurrentValue : {(target as GlobalFloat).Value}");
        }
    }
#endif

    [CreateAssetMenu(fileName = "MyFloat", menuName = "Variables/Float")]
    public class GlobalFloat : ScriptableVariable<float> {
        public override void FromString(string value) {
            float result;
            if (float.TryParse(value, out result)) {
                Value = result;
            }
        }
    }
}
