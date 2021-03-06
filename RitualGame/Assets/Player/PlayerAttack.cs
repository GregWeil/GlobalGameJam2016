﻿using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	float attackRadius = 0.3f;
	float attackRange = 0.8f;

	float knockback = 20.0f;
    float cooldown = 0.0f;
    
	Rigidbody2D body = null;
	PlayerMovement move = null;
	Animator anim = null;
	int controlNum;

	public AudioSource hurtSound;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		move = GetComponent<PlayerMovement> ();
		anim = GetComponentInChildren<Animator> ();
		controlNum = move.controlNum;
	}

	IEnumerator Attack () {
		anim.SetTrigger ("Attack");
        cooldown = 0.2f;
		yield return new WaitForSeconds (0.15f);
        Vector2 pos = body.position + (0.8f * Vector2.up);
		Vector2 dir = GetComponentInChildren<SpriteRenderer> ().flipX ? Vector2.left : Vector2.right;
		var hits = Physics2D.CircleCastAll (pos, attackRadius, dir, attackRange, LayerMask.GetMask ("Player"));
		foreach (var hit in hits) {
			if (hit.collider.gameObject != gameObject) {
				hit.collider.SendMessage ("Stun", 5.0f, SendMessageOptions.DontRequireReceiver);
				hit.collider.SendMessage ("DropIdol", SendMessageOptions.DontRequireReceiver);
				Vector2 push = (dir + new Vector2 (0, 0.3f)) * knockback;
				hit.collider.GetComponent<Rigidbody2D> ().AddForce (push, ForceMode2D.Impulse);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameMaster.gm.paused) { return; }
        cooldown -= Time.deltaTime;
		if (Input.GetButtonDown ("Attack" + controlNum.ToString ()) && !move.HasIdol() && (cooldown < 0f)) {
			StartCoroutine (Attack ());
		}
	}

	void Stun () {
		anim.SetTrigger ("Hurt");
		hurtSound.Play ();
	}
}
