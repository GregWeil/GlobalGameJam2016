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

	CircleCollider2D col = null;
	Rigidbody2D body = null;

	// Use this for initialization
	void Start () {
		col = GetComponent<CircleCollider2D> ();
		body = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		body.AddForce (gravity - Physics2D.gravity);
		float goalSpeed = (Input.GetAxis("Horizontal"+playerNum.ToString()) * moveSpeed);
		float force = (goalSpeed - body.velocity.x) * moveAccel;
		body.AddForce(new Vector2(force, 0));

        if (jump) {
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        } else if (Input.GetButton("Jump" + playerNum.ToString()) && !grounded) {
            body.AddForce(new Vector2(0, jumpCarry));
        }
        jump = false;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit2D hit = Physics2D.CircleCast (body.position, 0.9f*col.radius, Vector2.down, 0.1f, LayerMask.GetMask("Solid"));
        grounded = ((hit.collider != null) && !jump);
		if (Input.GetButtonDown ("Jump" + playerNum.ToString ()) && grounded) {
			jump = true;
		}
	}
}
