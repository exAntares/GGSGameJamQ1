using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DamageFeedback : MonoBehaviour {
    public float OffsetY = 5;
    public float Duration = 1.0f;
    public TextMesh Text;

    IEnumerator Start() {
        transform.DOMoveY(transform.position.y + OffsetY, Duration);
        GetComponentInChildren<MeshRenderer>().material.DOFade(0.0f, Duration);
        yield return new WaitForSeconds(Duration);
        Destroy(gameObject);
    }

    public void SetText(string text) {
        Text.text = text;
    }
}
