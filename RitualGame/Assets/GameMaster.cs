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
	int idolsRemaining;
	[SerializeField]
	int roundNumber = 0;
	[SerializeField]
	Vector3 playerRoles = new Vector3(0,1,2);

	[Header("Gameplay")]
	public int spawnDelay = 1;
	public int pointsPerIdol = 2;
	public int idolsPerRound = 10;

	[Header("Setup")]
	public Sprite[] godOpenSprites = new Sprite[3];
	public Sprite[] godClosedSprites = new Sprite[3];
	public Sprite[] playerSprites = new Sprite[3];
	public Sprite[] totemSprites = new Sprite[3];
	public GameObject playerPrefab;
	public GameObject idolPrefab;
	public GameObject idolSpawn;

	int[,] rounds = {
		//player #s, not control #s
		//{ left-tribesman, god, right-tribesman }
		{ 0, 1, 2 }, { 2, 0, 1 }, { 1, 2, 0 },
		{ 0, 1, 2 }, { 1, 2, 0 }, { 2, 0, 1 }
	};

	SpriteRenderer leftTotem = null;
	SpriteRenderer rightTotem = null;
	PlayerSpawnPoint leftSpawn = null;
	PlayerSpawnPoint rightSpawn = null;
	PlayerMovement leftPlayer = null;
	PlayerMovement rightPlayer = null;
	GodHandAnimation godPlayer = null;

	void Start(){
		GameObject[] totems = GameObject.FindGameObjectsWithTag ("Totem");
		Debug.Assert (totems.Length == 2);
		foreach (GameObject tt in totems) {
			if (tt.transform.position.x < 0) {
				leftTotem = tt.GetComponent<SpriteRenderer> ();
			} else {
				rightTotem = tt.GetComponent<SpriteRenderer> ();
			}
		}
		PlayerSpawnPoint[] spawns = GameObject.FindObjectsOfType<PlayerSpawnPoint> ();
		Debug.Assert (spawns.Length == 2);
		foreach (PlayerSpawnPoint sp in spawns) {
			if (sp.transform.position.x < 0) {
				leftSpawn = sp;
			} else {
				rightSpawn = sp;
			}
		}
		godPlayer = GameObject.FindObjectOfType<GodHandAnimation> ();
		PlayerMovement[] tribesmen = GameObject.FindObjectsOfType<PlayerMovement> ();
		Debug.Assert (tribesmen.Length == 2);
		foreach (PlayerMovement pl in tribesmen) {
			if (pl.transform.position.x < 0) {
				leftPlayer = pl;
			} else {
				rightPlayer = pl;
			}
		}
		InitializeRound ();
	}

	void Update(){
		if(idolsRemaining <= 0){
			InitializeRound ();
		}
	}

	void InitializeRound(){
		roundNumber++;
		idolsRemaining = idolsPerRound;
		playerRoles = new Vector3 (rounds[roundNumber,0], rounds[roundNumber,1], rounds[roundNumber,2]);
		//reset positions onscreen
		leftPlayer.transform.position = leftSpawn.transform.position;
		rightPlayer.transform.position = rightSpawn.transform.position;
		godPlayer.transform.position = new Vector3(0, godPlayer.transform.position.y);
		//set object player numbers
		leftSpawn.playerNumber = leftPlayer.playerNum = rounds[roundNumber,0];
		godPlayer.playerNum = rounds[roundNumber,1];
		rightSpawn.playerNumber = rightPlayer.playerNum = rounds[roundNumber,2];
		//set sprites
		leftPlayer.GetComponentInChildren<SpriteRenderer>().sprite = playerSprites[rounds[roundNumber,0]];
		leftTotem.sprite = totemSprites[rounds[roundNumber,0]];
		godPlayer.handOpen = godOpenSprites[rounds[roundNumber,1]];
		godPlayer.handClosed = godClosedSprites[rounds[roundNumber,1]];
		rightPlayer.GetComponentInChildren<SpriteRenderer>().sprite = playerSprites[rounds[roundNumber,2]];
		rightTotem.sprite = totemSprites[rounds[roundNumber,2]];
	}

	public IEnumerator RespawnIdol () {
		yield return new WaitForSeconds (spawnDelay);
		Instantiate (idolPrefab, idolSpawn.transform.position, idolSpawn.transform.rotation);
	}

	public void ScorePoints(int playerNum, Idol idol){
		int idolBonusPoints = idol.getDamage ();
		playerScores [rounds[roundNumber,1]] += pointsPerIdol + idolBonusPoints;
		playerScores [playerNum] += pointsPerIdol;
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