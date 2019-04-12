using System.Collections.Generic;
using UnityEngine;

namespace HalfBlind.ScriptableVariables {
    [CreateAssetMenu(fileName = "GlobalListString", menuName = "Variables/List/String")]
    public class GlobalListString : ScriptableVariable<List<string>> {
        [SerializeField] private string _separator = ";";

        public override void FromString(string value) {
            var split = value.Split(new string[] { _separator }, System.StringSplitOptions.None);
            Value = new List<string>(split);
        }
    }
}
