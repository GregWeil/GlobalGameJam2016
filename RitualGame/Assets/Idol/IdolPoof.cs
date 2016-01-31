using UnityEngine;
using System.Collections;

public class IdolPoof : MonoBehaviour {

	public Sprite[] frames;
	public float animTime = 1.0f;

	float time = 0.0f;
	SpriteRenderer render;

	// Use this for initialization
	void Start () {
		render = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (time >= animTime) {
			Destroy (gameObject);
		}
		render.sprite = frames [Mathf.FloorToInt (frames.Length * time / animTime)];
		time += Time.deltaTime;
	}
}
