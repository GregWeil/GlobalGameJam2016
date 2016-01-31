using UnityEngine;
using System.Collections;

public class SpriteShadow : MonoBehaviour {

    public Material material;
    SpriteRenderer sprite, shadow;

	// Use this for initialization
	void Start () {
        sprite = GetComponent<SpriteRenderer>();
        var obj = new GameObject("Shadow");
        obj.transform.parent = sprite.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        shadow = obj.AddComponent<SpriteRenderer>();
        shadow.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        shadow.material = material;
    }

    void Update () {
        shadow.sprite = sprite.sprite;
        shadow.flipX = sprite.flipX;
        shadow.flipY = sprite.flipY;
    }

}
