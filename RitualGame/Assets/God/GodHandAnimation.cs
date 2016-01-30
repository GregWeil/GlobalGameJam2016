using UnityEngine;
using System.Collections;

public class GodHandAnimation : MonoBehaviour {

	Vector2 position = Vector2.zero;
	Vector2 destination = Vector2.zero;
	Vector2 velocity = Vector2.zero;

	float cloudPosition = 0;
	float cloudVelocity = 0;

	public Transform hand, cloud;

	// Use this for initialization
	void Start () {
		position = transform.position;
		destination = position;
	}
	
	// Update is called once per frame
	void Update () {
		position = Vector2.SmoothDamp (position, destination, ref velocity, 0.3f);
		cloudPosition = Mathf.SmoothDamp (cloudPosition, destination.x, ref cloudVelocity, 0.5f);
		cloudPosition = position.x;

		hand.position = new Vector3 (position.x, position.y, hand.position.z);
		cloud.position = new Vector3 (cloudPosition, cloud.position.y, cloud.position.z);
		float handAngle = (-Mathf.Atan2 (cloud.position.x - hand.position.x, Mathf.Abs(cloud.position.y - hand.position.y)) * Mathf.Rad2Deg);
		hand.rotation = Quaternion.AngleAxis (handAngle, Vector3.forward);
	}

	public void SetGoal(Vector2 pos) {
		destination = pos;
	}

	public void SetExit() {
		destination = new Vector2 (position.x, cloud.position.y + 0.1f);
	}

	public Vector2 GetPosition() {
		return position;
	}

	public bool IsReady() {
		if ((position.y >= cloud.position.y) && (destination.y > position.y)) return true;
		return (Vector2.Distance (position, destination) <= 0.01f);
	}
}
