using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	float attackRadius = 0.3f;
	float attackRange = 0.8f;

	float knockback = 20.0f;

	Collider2D col = null;
	Rigidbody2D body = null;
	int controlNum;

	// Use this for initialization
	void Start () {
		col = GetComponent<Collider2D> ();
		body = GetComponent<Rigidbody2D> ();
		controlNum = GetComponent<PlayerMovement> ().controlNum;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Attack" + controlNum.ToString ())) {
			Vector2 dir = GetComponentInChildren<SpriteRenderer> ().flipX ? Vector2.left : Vector2.right;
			var hits = Physics2D.CircleCastAll (body.position, attackRadius, dir, attackRange, LayerMask.GetMask ("Player"));
			foreach (var hit in hits) {
				if (hit.collider != col) {
					hit.collider.SendMessage ("Stun", 5.0f, SendMessageOptions.DontRequireReceiver);
					hit.collider.SendMessage ("DropIdol", SendMessageOptions.DontRequireReceiver);
					Vector2 push = (dir + new Vector2 (0, 0.3f)) * knockback;
					hit.collider.GetComponent<Rigidbody2D> ().AddForce (push, ForceMode2D.Impulse);
				}
			}
		}
	}
}
