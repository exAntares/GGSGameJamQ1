using UnityEngine;

public class HeroController : MonoBehaviour {
    [SerializeField] private float _gravity = 8.9f;
    [SerializeField] private Vector2 _minMaxX;
    [SerializeField] private Vector2 _minMaxY;

    private Vector2 Impulse = Vector2.zero;
    private float lastSign = 1;

    private void FixedUpdate() {
        if (Input.GetButton("Jump")) {
            Impulse = Vector2.up;
        }

        var horizontal = Input.GetAxis("Horizontal");
        if(horizontal != 0) {
            var localScale = transform.localScale;
            localScale.x = Mathf.Sign(horizontal);
            transform.localScale = localScale;
        }
        Impulse.x = horizontal;

        var currentPos = transform.position;
        currentPos += new Vector3(Impulse.x, Impulse.y);
        Impulse *= 0.5f;

        currentPos.y -= _gravity * 0.01f;
        currentPos.y = currentPos.y <= _minMaxY.x ? _minMaxY.x : currentPos.y >= _minMaxY.y ? _minMaxY.y : currentPos.y;
        currentPos.x = currentPos.x <= _minMaxX.x ? _minMaxX.x : currentPos.x >= _minMaxX.y ? _minMaxX.y : currentPos.x;
        transform.position = currentPos;
    }
}
