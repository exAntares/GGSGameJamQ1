using System.Collections.Generic;
using UnityEngine;

namespace HalfBlind.ScriptableVariables {
    [CreateAssetMenu(fileName = "GlobalListSprite", menuName = "Variables/List/Sprite")]
    public class GlobalListSprite : ScriptableVariable<List<Sprite>> {
        public override void FromString(string value) {
            throw new System.NotImplementedException();
        }
    }
}
