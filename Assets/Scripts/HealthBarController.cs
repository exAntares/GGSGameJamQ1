using UnityEngine;

public class HealthBarController : MonoBehaviour {
    public SpriteRenderer Fill;
    private HealthComponent _healthComp;

    // Start is called before the first frame update
    void Start() {
        _healthComp = GetComponent<HealthComponent>();
    }

    // Update is called once per frame
    void Update() {
        var fillScale = Fill.transform.localScale;
        fillScale.x = _healthComp.Health / (float)_healthComp.MaxHealth;
        Fill.transform.localScale = fillScale;
    }
}
