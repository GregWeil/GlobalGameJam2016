using UnityEngine;
using System.Collections;

public class PlayerSpawnPoint : MonoBehaviour {

	public int playerNumber = 0;

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Idol") {
			GameMaster.gm.ScorePoints (playerNumber, coll.gameObject.GetComponent<Idol>());
		}
	}
}
