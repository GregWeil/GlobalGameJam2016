using UnityEngine;
using System.Collections;

public class GodHandAnimation : MonoBehaviour {

	public int playerNum = 0;

	Vector2 position = Vector2.zero;
	Vector2 destination = Vector2.zero;
	Vector2 velocity = Vector2.zero;

	float cloudPosition = 0;
	float cloudVelocity = 0;

	float armLength = 15f;
	
	public Sprite handOpen, handClosed;

	public Transform hand, cloud, cutoff;
	SpriteRenderer handRen = null;
	Material handMat = null;

	// Use this for initialization
	void Start () {
		position = transform.position;
		destination = position;
		handRen = hand.GetComponentInChildren<SpriteRenderer> ();
		handMat = handRen.material;
	}
	
	// Update is called once per frame
	void Update () {
		position = Vector2.SmoothDamp (position, destination, ref velocity, 0.1f);
		cloudPosition = Mathf.SmoothDamp (cloudPosition, destination.x, ref cloudVelocity, 0.3f);
		cloudPosition = position.x;

		hand.position = new Vector3 (position.x, position.y, hand.position.z);
		cloud.position = new Vector3 (cloudPosition, cloud.position.y, cloud.position.z);
		cloud.localPosition = new Vector3 (cloud.localPosition.x, 0f, cloud.localPosition.z);
		Vector3 cloudOffset = (cloud.position - hand.position);
		if (cloudOffset.magnitude > armLength) {
			cloud.position = (hand.position + (cloudOffset.normalized * armLength));
		}

		if (cloud.position.y > hand.position.y) {
			float handAngle = (-Mathf.Atan2 (cloud.position.x - hand.position.x, cloud.position.y - hand.position.y) * Mathf.Rad2Deg);
			hand.rotation = Quaternion.AngleAxis (handAngle, Vector3.forward);
		}

		handMat.SetFloat ("_Cutoff", cutoff.position.y);
	}

	public void SetGoal(Vector2 pos) {
		destination = pos;
	}

	public void SetExit() {
		destination = new Vector2 (position.x, transform.position.y + 1f);
	}

	public Vector2 GetPosition() {
		return position;
	}

	public bool IsReady() {
		if ((position.y >= cloud.position.y) && (destination.y > position.y)) return true;
		return (Vector2.Distance (position, destination) <= 0.1f);
	}

	public void Open() {
		handRen.sprite = handOpen;
	}

	public void Close() {
		handRen.sprite = handClosed;
	}
}
