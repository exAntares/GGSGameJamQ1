using UnityEngine;

public class EnemyController : MonoBehaviour {
    private HeroController _player;

    private void Awake() {
        _player = FindObjectOfType<HeroController>();
    }

    private void FixedUpdate() {
        
    }
}
