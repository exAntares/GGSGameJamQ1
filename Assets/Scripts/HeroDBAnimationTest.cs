using DragonBones;
using UnityEngine;

public class HeroDBAnimationTest : MonoBehaviour {
    private UnityArmatureComponent _animation;

    // Start is called before the first frame update
    void Start() {
        _animation = GetComponent<UnityArmatureComponent>();
        _animation.armature.animation.FadeIn("Idle", 0.05f, -1, 0); // a shooting animation that plays in loop (-1)
        _animation.armature.animation.FadeIn("Attack", 0.025f, 0, 1); // while shooting, he also kicks in one round. Not 
    }
}
