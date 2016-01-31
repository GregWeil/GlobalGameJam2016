using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;

	void Awake () {
		if (gm == null) { gm = this; }
	}

	[Header("Game Info")]
	[SerializeField]
	int[] playerScores = {0, 0, 0};
	[SerializeField]
	int idolsRemaining = 10;
	[SerializeField]
	int roundNumber = 1;

	[Header("Gameplay")]
	public int spawnDelay = 1;
	public int pointsPerIdol = 2;

	[Header("Setup")]
	public Sprite[] godSprites = new Sprite[3];
	public Sprite[] playerSprites = new Sprite[3];
	public GameObject playerPrefab;
	public GameObject idolPrefab;
	public GameObject playerSpawn;
	public GameObject idolSpawn;

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
		idolsRemaining--;
		StartCoroutine (RespawnIdol());
	}

	public static void BreakIdol(Idol idol){
		Destroy (idol.gameObject);
		gm.idolsRemaining--;
		gm.StartCoroutine (gm.RespawnIdol());
	}

}