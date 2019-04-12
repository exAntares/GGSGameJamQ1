using System.Collections.Generic;
using UnityEngine;

namespace HalfBlind.ScriptableVariables {
    [CreateAssetMenu(fileName = nameof(GlobalListGlobalBoolean), menuName = "Variables/List/"+nameof(GlobalListGlobalBoolean))]
    public class GlobalListGlobalBoolean : ScriptableVariable<List<GlobalBoolean>> {
        public override void FromString(string value) {
            throw new System.NotImplementedException();
        }
    }
}
