using UnityEngine;
using System.Collections;

public class Idol : MonoBehaviour {
	public Sprite sprite1;
	public GameObject spawnPoint;
	public float dropForce = 100f;
	public float pickupDelay = 2f;

	bool isHeld = false;
	PlayerMovement lastPlayer = null;

	BoxCollider2D col = null;
	Rigidbody2D body = null;
	int damage = 0;
	float dropTime = 0f;

	// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider2D> ();
		body = GetComponent<Rigidbody2D> ();
		spawnPoint = GameObject.FindGameObjectWithTag ("IdolSpawn");
	}

	// Update is called once per frame
	void Update () {

	}

	public bool PickUp(PlayerMovement player){
		if (player != lastPlayer || (dropTime + pickupDelay < Time.time) ) {
			transform.parent = player.transform;
			float offset = player.GetComponent<CircleCollider2D> ().radius + col.size.y / 2;
			transform.localPosition = new Vector3 (0, offset, 0);
			col.enabled = false;
			body.isKinematic = true;
			isHeld = true;
			lastPlayer = player;
			return true;
		} else{
			return false;
		}
	}

	public void Drop(){
		col.enabled = true;
		body.isKinematic = false;
		transform.parent = lastPlayer.transform.parent;
		isHeld = false;
		dropTime = Time.time;
		Vector3 dir = spawnPoint.transform.position - transform.position;
		dir = dir.normalized;
		body.AddForce (dropForce * dir);
	}


}
