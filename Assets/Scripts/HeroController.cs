using UnityEngine;

public class HeroController : MonoBehaviour {
    private float Gravity = 8.9f;
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

        currentPos.y -= Gravity * 0.01f;
        currentPos.y = currentPos.y <= 0 ? 0 : currentPos.y;
        transform.position = currentPos;
    }
}
