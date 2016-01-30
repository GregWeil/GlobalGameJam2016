using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public int playerNum = 0;

	public float moveSpeed = 1.0f;
	public float moveAccel = 1.0f;

	public float jumpForce = 1.0f;
    public float jumpCarry = 1.0f;
	public Vector2 gravity = Vector2.zero;

    bool grounded = false;
	bool jump = false;
	bool hasIdol = false;

	GameObject idol = null;

	float stun = 1.0f;

	CircleCollider2D col = null;
	Rigidbody2D body = null;
	//Sprite sprite = null;

	// Use this for initialization
	void Start () {
		col = GetComponent<CircleCollider2D> ();
		body = GetComponent<Rigidbody2D> ();
		//sprite = GetComponentInChildren<SpriteRenderer> ().sprite;
	}

	void FixedUpdate () {
		body.AddForce (gravity - Physics2D.gravity);
		float goalSpeed = (Input.GetAxis("Horizontal"+playerNum.ToString()) * moveSpeed / stun);
		float force = ((goalSpeed - body.velocity.x) * moveAccel);
		if (!grounded) force /= stun;
		body.AddForce(new Vector2(force, 0));

		if (jump) {
			body.AddForce(new Vector2(0, jumpForce / stun), ForceMode2D.Impulse);
        } else if (Input.GetButton("Jump" + playerNum.ToString()) && !grounded) {
            body.AddForce(new Vector2(0, jumpCarry));
        }
        jump = false;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit2D hit = Physics2D.CircleCast (body.position, 0.9f*col.radius, Vector2.down, 0.1f, LayerMask.GetMask("Solid"));
		grounded = ((hit.collider != null) && !jump);

		//Turning
		if (grounded && (Mathf.Abs(Input.GetAxis("Horizontal"+playerNum.ToString())) > 0.1f)) {
			GetComponentInChildren<SpriteRenderer> ().flipX = (Input.GetAxis("Horizontal"+playerNum.ToString()) < 0.0f);
		}

		//Jumping
		if (Input.GetButtonDown ("Jump" + playerNum.ToString ()) && grounded) {
			jump = true;
		}

		//Recovery
		stun = Mathf.MoveTowards(stun, 1.0f, 5.0f * Time.deltaTime);
	}

	void Stun (float amount) {
		stun = Mathf.Max (amount, stun);
	}

	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Idol" && !hasIdol) {
			PickUpIdol(coll.gameObject);
		}
	}

	void PickUpIdol(GameObject id){
		idol = id;
		BoxCollider2D idol_box = idol.GetComponent<BoxCollider2D>();
		Rigidbody2D idol_body = idol.GetComponent<Rigidbody2D> ();
		idol.transform.parent = transform;
		float offset = col.size.y/2 + idol_box.size.y/2;
		idol.transform.localPosition = new Vector3 (0, offset, 0);
		idol_box.enabled = false;
		idol_body.isKinematic = true;
		hasIdol = true;
	}

	void DropIdol(){
		idol.GetComponent<BoxCollider2D> ().enabled = true;
		idol.GetComponent<Rigidbody2D> ().isKinematic = false;
		idol.transform.parent = transform.parent;
		hasIdol = false;
		idol = null;
	}

}








