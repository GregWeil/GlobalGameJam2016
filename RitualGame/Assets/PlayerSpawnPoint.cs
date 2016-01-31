using UnityEngine;
using System.Collections;

public class PlayerSpawnPoint : MonoBehaviour {
	[Range(0,2)]
	public int playerNumber = 0;

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Idol") {
			Idol idol = coll.gameObject.GetComponent<Idol> ();
			if (idol.isHeld && idol.lastPlayer.playerNum == playerNumber) {
				idol.lastPlayer.DropIdol ();
				GameMaster.gm.ScorePoints (playerNumber, idol);
			}
		}
	}
}
