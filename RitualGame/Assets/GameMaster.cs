using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
	public int idolsPerRound = 3;
	[Range(0,1)]
	public float percentTotemOffscreen = 0.16f;
	public bool paused = true;
	bool gameOver = false;
	bool roundRunning = false;
	bool gameStarted = false;

	[Header("Setup")]
	public Material[] godReticles = new Material[3];
	public Sprite[] godOpenSprites = new Sprite[3];
	public Sprite[] godClosedSprites = new Sprite[3];
	public RuntimeAnimatorController[] playerAnimations = new RuntimeAnimatorController[3];
	public Sprite[] totemSprites = new Sprite[3];
	public GameObject playerPrefab;
	public GameObject idolPrefab;
	public GameObject idolSpawn;
	public AudioClip[] roundAudio = new AudioClip[6];
	public AudioClip whistle;


	Color[] plColors = { Color.blue, Color.green, Color.red };
	int[,] rounds = new int[,] {
		//player #s, not control #s
		//{ left-tribesman, god, right-tribesman }
		{-1, -1, -1}, //dummy array to align indices with round number
		{ 0, 1, 2 }, { 2, 0, 1 }, { 1, 2, 0 },
		{ 0, 1, 2 }, { 1, 2, 0 }, { 2, 0, 1 }
	};

	Camera cam;
	float camHorizView = 0f;

	//Gameplay objects
	SpriteRenderer leftTotem = null;
	SpriteRenderer rightTotem = null;
	PlayerSpawnPoint leftSpawn = null;
	PlayerSpawnPoint rightSpawn = null;
	PlayerMovement leftPlayer = null;
	PlayerMovement rightPlayer = null;
	GodHandAnimation godPlayer = null;

	// UI objects
	Text pauseText = null;
	Text titleText = null;
	Text roundText = null;
	Text startRoundText = null;
	Text endText = null;
	TextMesh idolCount = null;
	TextMesh leftTotemScore = null;
	TextMesh rightTotemScore = null;
	Text godScore = null;
	AudioSource menuMusic = null;
	AudioSource roundMusic = null;
	AudioSource roundBreakAudio = null;
	AudioSource endWhistle = null;
	AudioSource scoreAudio = null;

//====================================================================================

	void Start(){
		//audio
		menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource> ();
		roundMusic = GameObject.Find("RoundMusic").GetComponent<AudioSource> ();
		roundBreakAudio = GameObject.Find("RoundStart").GetComponent<AudioSource> ();
		endWhistle = GameObject.Find ("Whistle").GetComponent<AudioSource> ();
		scoreAudio = GameObject.Find ("ScoreAudio").GetComponent<AudioSource> ();
		menuMusic.Play ();
		roundMusic.Stop ();
		roundBreakAudio.Stop ();
		endWhistle.Stop ();
		scoreAudio.Stop ();

		//gameplay objects
		GameObject[] totems = GameObject.FindGameObjectsWithTag ("Totem");
		Debug.Assert (totems.Length == 2);
		foreach (GameObject tt in totems) {
			if (tt.transform.position.x < 0) {
				leftTotem = tt.GetComponentInChildren<SpriteRenderer> ();
				leftTotemScore = tt.GetComponentInChildren<TextMesh> ();
			} else {
				rightTotem = tt.GetComponentInChildren<SpriteRenderer> ();
				rightTotemScore = tt.GetComponentInChildren<TextMesh> ();
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

		//text
		godScore = GameObject.Find ("GodScore").GetComponent<Text> ();
		idolCount = GameObject.Find ("Pedestal").GetComponentInChildren<TextMesh> ();
		pauseText = GameObject.Find ("PauseText").GetComponent<Text> ();
		pauseText.enabled = false;
		titleText = GameObject.Find ("TitleText").GetComponent<Text> ();
		roundText = GameObject.Find ("RoundText").GetComponent<Text> ();
		startRoundText = GameObject.Find ("StartRoundButton").GetComponent<Text>();
		endText = GameObject.Find ("EndText").GetComponent<Text> ();
		endText.enabled = false;

		//screen positioning
		cam = Camera.main;
		camHorizView = Mathf.Abs(cam.transform.position.z) * Mathf.Tan (0.5f * Mathf.Deg2Rad * cam.fieldOfView) * Screen.width / Screen.height; //the camera's horizontal view extent
		Debug.Log ("Cam view = " + camHorizView);
        /*
		float totemOffsetFromEdge = leftTotem.sprite.bounds.size.x * percentTotemOffscreen;
//		Debug.Log ("Totem sprite width = " + leftTotem.sprite.bounds.size.x + ", offset from edge = " + totemOffsetFromEdge);
		leftTotem.transform.parent.position = new Vector3(totemOffsetFromEdge - camHorizView, leftTotem.transform.parent.position.y, leftTotem.transform.parent.position.z);
		rightTotem.transform.parent.position = new Vector3(camHorizView - totemOffsetFromEdge, rightTotem.transform.parent.position.y, rightTotem.transform.parent.position.z);
        */

        InitializeNextRound();
	}

//====================================================================================

	void Update(){
		if(idolsRemaining <= 0){
			endWhistle.Play ();
			if (roundNumber >= 6) { 
				EndGame ();
			} else{
				InitializeNextRound ();
			}
		}
		if (!gameOver && Input.GetButtonDown ("Pause")) { PauseGame (); }
		if (!roundRunning && Input.GetButtonDown ("Start")) { EndRoundBreak (); }
		if (paused && Input.GetButtonDown ("Restart")) { RestartGame (); }
		if (paused && Input.GetButtonDown ("Quit")) { QuitGame (); }
	}

//====================================================================================

	void UpdateDisplays(){
//		Debug.Log ("Round num: " + roundNumber + ", num scores: " + playerScores.Length);
		leftTotemScore.text = "" + playerScores[rounds[roundNumber,0]];
		godScore.text = "" + playerScores[rounds[roundNumber,1]];
		rightTotemScore.text = "" + playerScores[rounds[roundNumber,2]];
		idolCount.text = "" + idolsRemaining;
	}

//====================================================================================

	public void QuitGame(){
		if (paused) {
			Application.Quit ();
		}
	}

//====================================================================================

	public void RestartGame(){
		if (paused) {
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}
	}

//====================================================================================

	public void PauseGame(){
		if (!roundRunning) { return; }
		Debug.Log ("Pause");
		if (paused) {
			pauseText.enabled = false;
			paused = false;
			Time.timeScale = 1f;
		} else {
			paused = true;
			Time.timeScale = 0f;
			pauseText.enabled = true;
		}
	}

//====================================================================================

	public void RoundBreak(){
		roundRunning = false;
		paused = true;
		roundText.text = "Round " + roundNumber;
		roundText.enabled = true;
		startRoundText.enabled = true;
	}

//====================================================================================

	public void EndRoundBreak(){
		if (!gameStarted) {
			menuMusic.Stop ();
			roundMusic.Play ();
			gameStarted = true;
		}
		titleText.enabled = false;
		roundText.enabled = false;
		startRoundText.enabled = false;
		roundRunning = true;
		paused = false;
	}

//====================================================================================

	public void EndGame(){
		paused = true;
		roundMusic.Stop ();
		menuMusic.Play ();
		string p0 = "Player 1: " + playerScores[0] + "\n";
		string p1 = "Player 2: " + playerScores[1] + "\n";
		string p2 = "Player 3: " + playerScores[2] + "\n";
		endText.text = "Final Score:\n" + p0 + p1 + p2;
		endText.enabled = true;
		gameOver = true;
	}

//====================================================================================

	void InitializeNextRound(){
		roundNumber++;
		paused = true;
		idolsRemaining = idolsPerRound;
//		Debug.Log ("Round num: " + roundNumber);
//		Debug.Log ("Left: Player " + rounds [roundNumber, 0]);
//		Debug.Log("God: Player " + rounds[roundNumber,1]);
//		Debug.Log("Right: Player " + rounds[roundNumber,2]);
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
		leftPlayer.GetComponentInChildren<Animator>().runtimeAnimatorController = playerAnimations[rounds[roundNumber,0]];
		leftTotem.sprite = totemSprites[rounds[roundNumber,0]];
		leftTotemScore.color = plColors[rounds[roundNumber,0]];
		godPlayer.handOpen = godOpenSprites[rounds[roundNumber,1]];
		godPlayer.handClosed = godClosedSprites[rounds[roundNumber,1]];
		godScore.color = plColors[rounds[roundNumber,1]];
		godPlayer.GetComponentInChildren<MeshRenderer>().material = godReticles[rounds[roundNumber,1]];
		rightPlayer.GetComponentInChildren<Animator>().runtimeAnimatorController = playerAnimations[rounds[roundNumber,2]];
		rightTotem.sprite = totemSprites[rounds[roundNumber,2]];
		rightTotemScore.color = plColors[rounds[roundNumber,2]];
		UpdateDisplays ();

		roundBreakAudio.clip = roundAudio[roundNumber-1];
		roundBreakAudio.Play ();
		RoundBreak ();
		Debug.Log ("Left: Player " + playerRoles.x + ", God: Player " + playerRoles.y + ", Right: Player " + playerRoles.z);
	}

//====================================================================================

	public IEnumerator RespawnIdol () {
		yield return new WaitForSeconds (spawnDelay);
		Instantiate (idolPrefab, idolSpawn.transform.position, idolSpawn.transform.rotation);
	}

//====================================================================================

	public void ScorePoints(int playerNum, Idol idol){
		Debug.Log ("Round num: " + roundNumber);
		int idolBonusPoints = idol.getDamage ();
		playerScores [rounds[roundNumber,1]] += pointsPerIdol + idolBonusPoints;
		playerScores [playerNum] += pointsPerIdol;
		Destroy (idol.gameObject);
		idolsRemaining--;
		StartCoroutine (RespawnIdol());
		UpdateDisplays ();
		scoreAudio.Play ();
	}

//====================================================================================

	public static void BreakIdol(Idol idol){
		Destroy (idol.gameObject);
		gm.playerScores [gm.rounds [gm.roundNumber, 1]] -= gm.pointsPerIdol;
		gm.idolsRemaining--;
		gm.StartCoroutine (gm.RespawnIdol());
		gm.UpdateDisplays ();
	}

//====================================================================================

}
