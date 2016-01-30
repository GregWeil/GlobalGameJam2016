using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;

	void Awake () {
		if (gm == null) { gm = this; }
	}

//	[System.Serializable]
//	public class Scores{
//		public int[] playerScores = {0, 0, 0};
//	}
//
//	public Scores score;
	public GameObject playerPrefab;
	public GameObject idolPrefab;
	public GameObject playerSpawn;
	public GameObject idolSpawn;
	public int spawnDelay = 1;
	public int pointsPerIdol = 2;

	[SerializeField]
	int[] playerScores = {0, 0, 0};

//	GameObject leftPlayerSpawn = null;
//	GameObject rightPlayerSpawn = null;

	void Start(){
		
	}

	public IEnumerator RespawnIdol () {
		yield return new WaitForSeconds (spawnDelay);
		Instantiate (idolPrefab, idolSpawn.transform.position, idolSpawn.transform.rotation);
	}

	public void ScorePoints(int playerNum, Idol idol){
		int idolBonusPoints = idol.getDamage ();
		playerScores [playerNum] += pointsPerIdol + idolBonusPoints;
		Destroy (idol.gameObject);
		StartCoroutine (RespawnIdol());
	}

	public static void BreakIdol(Idol idol){
		Destroy (idol.gameObject);
		gm.StartCoroutine (gm.RespawnIdol());
	}

}