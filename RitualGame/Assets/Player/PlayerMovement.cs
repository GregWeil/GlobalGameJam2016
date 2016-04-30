using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	[Range(0,2)]
	public int playerNum = 0;
	[Range(0,1)]
	public int controlNum = 0;

	public float moveSpeed = 1.0f;
	public float moveAccel = 1.0f;

	public float idolSlow = 0.8f;

	public float jumpForce = 1.0f;
    public float jumpCarry = 1.0f;
	public Vector2 gravity = Vector2.zero;

    bool grounded = false;
	bool jump = false;
	[SerializeField]
	bool hasIdol = false;

	Idol idol = null;

	float stun = 1.0f;

	BoxCollider2D col = null;
	Rigidbody2D body = null;

	Animator anim = null;

	public AudioSource moveSound, jumpSound, pickSound;
	public AudioClip runClip, walkClip;

	// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider2D> ();
		body = GetComponent<Rigidbody2D> ();
		anim = GetComponentInChildren<Animator> ();
	}

	void FixedUpdate () {
		if (GameMaster.gm.paused) { return; }
		body.AddForce (gravity - Physics2D.gravity);
		float goalSpeed = (Input.GetAxis("Horizontal"+controlNum.ToString()) * moveSpeed / stun);
		if (hasIdol) goalSpeed *= idolSlow;
		float force = ((goalSpeed - body.velocity.x) * moveAccel);
		if (!grounded) force /= stun;
		body.AddForce(new Vector2(force, 0));

		if (jump) {
			body.AddForce(new Vector2(0, jumpForce / stun), ForceMode2D.Impulse);
        } else if (Input.GetButton("Jump" + controlNum.ToString()) && !grounded) {
            body.AddForce(new Vector2(0, jumpCarry));
        }
        jump = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Update grounded
		RaycastHit2D hit = Physics2D.BoxCast (body.position, 0.9f*col.size, 0f, Vector2.down, 0.5f, LayerMask.GetMask("Solid"));
		grounded = ((hit.collider != null) && !jump);

		//Animations
		anim.SetBool ("Grounded", grounded);
		anim.SetFloat ("SpeedX", Mathf.Abs (body.velocity.x));
		anim.SetFloat ("SpeedY", body.velocity.y);
		anim.SetBool ("Carrying", hasIdol);

		if (GameMaster.gm.paused) { return; }

		//Turning
		if (grounded && (Mathf.Abs(Input.GetAxis("Horizontal"+controlNum.ToString())) > 0.4f)) {
			GetComponentInChildren<SpriteRenderer> ().flipX = (Input.GetAxis("Horizontal"+controlNum.ToString()) < 0.0f);
			if (!moveSound.isPlaying) moveSound.Play ();
		}

		//Jumping
		if (Input.GetButtonDown ("Jump" + controlNum.ToString ()) && grounded) {
			jump = true;
			jumpSound.Play ();
		}

		//Recovery
		stun = Mathf.MoveTowards(stun, 1.0f, 5.0f * Time.deltaTime);

		//Animations
		anim.SetBool ("Grounded", grounded);
		anim.SetFloat ("SpeedX", Mathf.Abs (body.velocity.x));
		anim.SetFloat ("SpeedY", body.velocity.y);
		anim.SetBool ("Carrying", hasIdol);
	}

	void Stun (float amount) {
		stun = Mathf.Max (amount, stun);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Idol" && !hasIdol) {
			PickUpIdol(coll.gameObject.GetComponent<Idol>());
		}
	}

	public void PickUpIdol(Idol id) {
		idol = id;
		hasIdol = idol.PickUp (this);
		if(hasIdol) { 
			moveSound.clip = walkClip;
			pickSound.Play (); 
		}
	}

	public void DropIdol() {
		if (!hasIdol) { return; }
		idol.Drop();
		hasIdol = false;
		idol = null;
		moveSound.clip = runClip;
	}

	public bool HasIdol() {
		return hasIdol;
	}

}
