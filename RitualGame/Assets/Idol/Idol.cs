using UnityEngine;
using System.Collections;

public class Idol : MonoBehaviour {
	public Sprite[] damageSprites = new Sprite[4];
	public float baseDropForce = 120f;
	public float pickupDelay = 2f;

	public bool isHeld = false;
	public PlayerMovement lastPlayer = null;

	GameObject spawnPoint;
	BoxCollider2D box = null;
	Rigidbody2D body = null;
	CircleCollider2D groundCheck = null;
	int damage = 0;
	float dropTime = 0f;

	[SerializeField]
	Vector2 dropForce;
	[SerializeField]
	float distToSpawn;

	// Use this for initialization
	void Start () {
		box = GetComponent<BoxCollider2D> ();
		body = GetComponent<Rigidbody2D> ();
		groundCheck = GetComponentInChildren<CircleCollider2D> ();
		spawnPoint = GameMaster.gm.idolSpawn;
		dropForce = new Vector2 (0f, baseDropForce);

	}

	// Update is called once per frame
	void Update () {
		distToSpawn = spawnPoint.transform.position.x - transform.position.x;
		float camHorizExtend = Camera.main.orthographicSize * Screen.width / Screen.height;
		float horizPercent = distToSpawn / camHorizExtend;
		dropForce.x = baseDropForce * 2 * horizPercent;
	}

	public bool PickUp(PlayerMovement player){
		if (player != lastPlayer || (dropTime + pickupDelay < Time.time) ) {
			transform.parent = player.transform;
            var col = player.GetComponent<BoxCollider2D>();
            float offset = col.offset.y + col.size.y/2 + box.size.y/2;
			transform.localPosition = new Vector3 (0, offset, 0);
			box.isTrigger = true;
			body.isKinematic = true;
			groundCheck.enabled = false;
			isHeld = true;
			lastPlayer = player;
			return true;
		} else{
			return false;
		}
	}

	public void Drop(){
		box.isTrigger = false;
		body.isKinematic = false;
		groundCheck.enabled = true;
		transform.parent = lastPlayer.transform.parent;
		isHeld = false;
		dropTime = Time.time;
		body.AddForce(dropForce);
	}

	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "PlayerSpawn") {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Ground" && !isHeld) {
			TakeDamage (1);
		}
	}

	void TakeDamage(int dam){
		damage += dam;
		Debug.Log ("Damage = " + damage);
		if (damage > 3) {
			GameMaster.BreakIdol (this);
		} else {
			GetComponentInChildren<SpriteRenderer> ().sprite = damageSprites [damage];
		}
	}

	public int getDamage(){ return damage; }

}
