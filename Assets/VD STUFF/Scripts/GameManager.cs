﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private static GameManager _instance;

	enum GameMode 
	{
		STANDBY, 
		COUNTDOWN,
		TUTORIAL,
		GAME, 
		GAMEOVER, 
		CONFIG
	}

	enum GamePhase
	{
		ONE, //2 enemies at a time
		TWO, //6 enemies
		THREE, //unlimited enemies
		END //Game over/reset
	}

	private TutorialManager tm;
	private AudioManager am;
	private AimAssistManager aam;

	private GameObject scoreText;
	private GameObject timerText;
	private GameObject streakText;
	private GameObject countdownText;
	private GameObject finalScoreText;
	private int score;
	private int streak = 0;
	private int streakMultiplier;

	public GameObject enemy;
	public Transform enemySpawnPoint;
	public float spawnRate = 3f;

	public List<Material> gridMats;

	public bool randomSpawnTime = false;
	bool screenChanged;
	public float randomRange = 1f;

	GameMode mode = GameMode.STANDBY;
	GamePhase phase = GamePhase.ONE;
	float phaseTimer;
	bool moveEnemyIsRunning;
	bool gameStarted = false;
	public bool inTutorialMode;
	public int gamePhaseInt;

	public SpawnFloor spawnFloor;

	public float joinTimer = 5f;
	public float gameTimer = 10f;
	public float scoreboardTimer = 15f;

	public GameObject batHoldBox;
	public GameObject ballPrefab;

	[System.NonSerialized]
	public float timer = 0f;

	// Spider values
	private Spider gameSpider;
	public int numSpiderAppearances = 4;


//	GUIStyle guiStyle;
	public Font guiFont;
	public float guiLeft = 0.2f;
	public float guiTop = 0.8f;

	public bool enemiesKnockback = false;

	private GameObject redScoreBox, yellowScoreBox, blueScoreBox, greenScoreBox;
//	private GUIText redScoreTxt, redPlaceTxt, redAccTxt,
//						yellowScoreTxt, yellowPlaceTxt, yellowAccTxt,
//						blueScoreTxt, bluePlaceTxt, blueAccTxt,
//						greenScoreTxt, greenPlaceTxt, greenAccTxt;

	private TextMesh redScoreTxt, redAccTxt,
						yellowScoreTxt, yellowAccTxt,
						blueScoreTxt, blueAccTxt,
						greenScoreTxt, greenAccTxt;

	BallManager ballManager;
	PlayerManager playerManager;
	QueueManager queueManager;

	GameObject kinectErrorObj;

	StaticPool staticPool;

	public GameObject[] warpParticles;

	#region Singleton Initialization
	public static GameManager instance {
		get { 
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<GameManager>();
			
			return _instance;
		}
	}
	
	void Awake() {
		if(_instance == null) {
			//If I am the fist instance, make me the first Singleton
			_instance = this;
			DontDestroyOnLoad(gameObject);
			staticPool = new StaticPool(); // Fight me
		} else {
			//If a Singleton already exists and you find another reference in scene, destroy it
			if(_instance != this)
				Destroy(gameObject);
		}
	}
	#endregion

	void Start() {
		ballManager = GetComponent<BallManager> ();
		playerManager = GetComponent<PlayerManager> ();
		tm = GetComponent<TutorialManager> ();
		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		aam = GetComponent<AimAssistManager> ();

//		tutorialBallSpawnPos = GameObject.Find ("TutBallSpawn").transform.position;
//		cameraTransform = Camera.main.transform;

		scoreText = GameObject.Find ("ScoreText");
		timerText = GameObject.Find ("TimerText");
		streakText = GameObject.Find ("StreakText");
		countdownText = GameObject.Find ("CountdownText");
		finalScoreText = GameObject.Find ("FinalScoreText");
		queueManager = GameObject.Find( "QueueManager" ).GetComponent<QueueManager>();

//		for (int i = 0; i < tutorialEnemies.Length; i++) 
//		{
//			tutorialEnemies [i].SetActive (false);
//		}

		SwitchGameMode(GameMode.STANDBY);
	}

	void Update() {
		switch(mode)
		{
		case GameMode.STANDBY:

			if (Input.GetKeyDown (KeyCode.Space)) {
				SwitchGameMode (GameMode.COUNTDOWN);
				inTutorialMode = false;
			}

			if (Input.GetKeyDown (KeyCode.T)) {
				SwitchGameMode (GameMode.TUTORIAL);
				inTutorialMode = true;
			}

			if (Input.GetKeyDown (KeyCode.C)) {
				SwitchGameMode (GameMode.CONFIG);
			}

			break;

		case GameMode.COUNTDOWN:
			foreach( Material mat in gridMats )
				mat.SetFloat("_Opacity_Slider", timer );

			if (timer <= 0) 
			{
				if (!inTutorialMode) {
					SwitchGameMode (GameMode.GAME);
					return;
				} else {
					SwitchGameMode (GameMode.TUTORIAL);
					return;
				}
			} 
			else if (timer <= 1) 
			{
				countdownText.GetComponent<Text> ().text = "Go!";
			} 
			else if (timer >= 4)
			{
				if (!inTutorialMode)
					countdownText.GetComponent<Text> ().text = "Get Ready!";
				else
					countdownText.GetComponent<Text> ().text = "Tutorial";
			}
			else
			{
				countdownText.GetComponent<Text>().text = ((int)timer).ToString();
			}

			timer -= Time.deltaTime;
			break;

		case GameMode.TUTORIAL:


			break;

		case GameMode.GAME:
			switch (phase) 
			{
			case GamePhase.ONE:
				if (aam.onCourtEnemies.Count >= 3)// && moveEnemyIsRunning) 
				{
					if (moveEnemyIsRunning) 
					{
						StopCoroutine ("StartEnemyMove");
						moveEnemyIsRunning = false;
					}

//					StopCoroutine ("StartEnemyMove");
//					moveEnemyIsRunning = false;
				} 
				else
				{
					if(!moveEnemyIsRunning)
						StartCoroutine ("StartEnemyMove");
				}

				phaseTimer -= Time.deltaTime;
				if (phaseTimer <= 0) 
				{
					SwitchGamePhase (GamePhase.TWO);
				}

				if (score >= 3) 
				{
					SwitchGamePhase (GamePhase.TWO);
				}

				break;

			case GamePhase.TWO:
				if (aam.onCourtEnemies.Count >= 6) 
				{
					if (moveEnemyIsRunning) 
					{
						StopCoroutine ("StartEnemyMove");
						moveEnemyIsRunning = false;
					}

				} 
				else 
				{
					if(!moveEnemyIsRunning)
						StartCoroutine ("StartEnemyMove");
				}

				phaseTimer -= Time.deltaTime;
				if (phaseTimer <= 0) 
				{
					SwitchGamePhase (GamePhase.THREE);
				}

				if (score >= 10) 
				{
					SwitchGamePhase (GamePhase.THREE);
				}

				break;

			case GamePhase.THREE:
				if (!moveEnemyIsRunning)
					StartCoroutine ("StartEnemyMove");

				break;
			}

			//SCORE TEXT
			scoreText.GetComponent<Text> ().text = "Score: " + score;

			//TIMER TEXT
			int minutes = Mathf.FloorToInt (timer / 60F);
			int seconds = Mathf.FloorToInt (timer - minutes * 60);
			string stringTimer = string.Format ("{0:0}:{1:00}", minutes, seconds);
			timerText.GetComponent<Text> ().text = "Time: " + stringTimer;

			//STREAK TEXT
			streakText.GetComponent<Text> ().text = "Streak: " + streak + " (x" + streakMultiplier + ")";
			streakMultiplier = Mathf.Clamp (1 + Mathf.Clamp (streak, 0, 1) + (int)(streak / 3), 1, 3);

			if(timer <= 0) {
				ballManager.StopAllCoroutines();
				StopAllCoroutines();
				timer = scoreboardTimer;
				SwitchGameMode(GameMode.GAMEOVER);

				Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
				foreach( Enemy enemy in enemies ) {
					enemy.StopAllCoroutines();
					enemy.gameObject.SetActive( false );
				}
				// Fuck you eric
				spawnFloor.ResetTilesFilled();
				queueManager.Reset();

				return;
			}

			timer -= Time.deltaTime;
			break;
		case GameMode.GAMEOVER:
			foreach (Material mat in gridMats)
				mat.SetFloat ("_Opacity_Slider", 2.5f - timer);

			if( timer <= 0 )
			{
				SwitchGameMode( GameMode.STANDBY );
				return;
			}

			timer -= Time.deltaTime;
			break;
		case GameMode.CONFIG:
			if (Input.GetKeyDown (KeyCode.Space)) {
				OVRManager.display.RecenterPose ();
			}

			if (Input.GetKeyDown (KeyCode.Escape)) {
				SwitchGameMode (GameMode.STANDBY);
			}

			if(Input.GetKeyDown(KeyCode.L)) {
				//mode = GameMode.STANDBY;
				Application.LoadLevel("Intro");
			}
			break;
		}
	}

	void SwitchGameMode( GameMode gm )
	{
		switch( gm )
		{
		case GameMode.STANDBY:
			gamePhaseInt = 0;
			StaticPool.DestroyAllObjects ();
			countdownText.SetActive (false);
			scoreText.SetActive (false);
			timerText.SetActive (false);
			streakText.SetActive (false);
			finalScoreText.SetActive (false);
			batHoldBox.SetActive (true);
			am.PlayAmbientCubeAudio ();
			foreach( Material mat in gridMats )
				mat.SetFloat("_Opacity_Slider", 2.5f);
			break;
		case GameMode.COUNTDOWN:
			timer = 5f;
			countdownText.SetActive (true);
			batHoldBox.SetActive (false);

			score = 0;
			streak = 0;
			streakMultiplier = 1;
			break;
		case GameMode.TUTORIAL:
			countdownText.SetActive (false);
			batHoldBox.SetActive (false);

			inTutorialMode = true;
			tm.StartTutorial();

			break;
		case GameMode.GAME:

			aam.ClearOnCourtEnemies ();
			SwitchGamePhase (GamePhase.ONE);

			GameObject[] staticPoolBalls = GameObject.FindGameObjectsWithTag ("Ball");
			if (staticPoolBalls.Length >= 1) 
			{
				foreach (GameObject ball in staticPoolBalls) 
				{
					ball.SetActive (false);
				}
			}

			score = 0;
			streak = 0;
			streakMultiplier = 1;
			am.PlayBackgroundMusic ();
			timer = gameTimer;
			countdownText.SetActive(false);
			scoreText.SetActive(true);
			timerText.SetActive(true);
			streakText.SetActive (true);

			//StaticPool.DisableAllObjects();
			//StaticPool.DestroyAllObjects(); // Ghetto fix for now. Wasting an allocation somewhere also I think.
			queueManager.Reset();

			StartCoroutine ("SpawnEnemy");
			StartCoroutine( "StartEnemyMove" );

			break;
		case GameMode.GAMEOVER:
			timer = 3f;
			scoreText.SetActive (false);
			timerText.SetActive (false);
			streakText.SetActive (false);
			finalScoreText.GetComponent<Text> ().text = "Score: " + score;
			finalScoreText.SetActive (true);
			aam.ClearOnCourtEnemies ();
			break;
		case GameMode.CONFIG:
			break;
		}
		
		mode = gm;
	}

	void SwitchGamePhase(GamePhase gp)
	{
		switch (gp) 
		{
		case GamePhase.ONE:
			phaseTimer = 30f;
			gamePhaseInt = 1;

			break;

		case GamePhase.TWO:
			phaseTimer = 30f;
			gamePhaseInt++;

			break;

		case GamePhase.THREE:
			phaseTimer = gameTimer;
			gamePhaseInt++;

			break;

		case GamePhase.END:

			break;
		}

		phase = gp;
	}

	IEnumerator SpawnEnemy() {
		while(true) {
			queueManager.SpawnNewEnemy( enemy );
			Debug.Log ("Spawning Enemy from GM");
			yield return new WaitForSeconds( 0.25f );
		}
	}

	IEnumerator StartEnemyMove() {
		while( true ) {
			queueManager.StartNextInQueue();
			Debug.Log ("Start Enemy Move from GM");
			moveEnemyIsRunning = true;
			yield return new WaitForSeconds( 1.5f );
		}
	}

	IEnumerator ShowInstructions() {
		//introGUI.ShowInstructions ();
		yield return new WaitForSeconds (4f);
		ChangeScene ("Main");
	}

	void ChangeScene( string scene ) {
		switch( scene )
		{
		case "Intro":
			timer = 0f;
			gameStarted = false;
			playerManager.playerData.Clear();
			//mode = GameMode.STANDBY;			
			Application.LoadLevel("Intro");
			break;
		case "Main":
			timer = gameTimer;
			//mode = GameMode.GAME;
			Application.LoadLevel("Main");
			break;
		}
	}

	public void OSCMessageReceived(OSC.NET.OSCMessage message)
	{
		switch( message.Address )
		{
		case "/timeChange":
			break;
		case "/startGame":
			SwitchGameMode(GameMode.GAME);
			break;
		case "/skipTutorial":
			break;
		case "/endGame":
			break;
		case "/enterConfig":
			break;
		case "/centerConfig":
			OVRManager.display.RecenterPose();
			break;
		case "/exitConfig":
			break;
		}
//		if(message.Address == "/checkColor") {
//			if(mode != GameMode.STANDBY) {
//				print ("RENDERER IS TRUE");
//				Camera.main.GetComponent<screenChange>().screenChanged = true;
//				float xPos = (1 - (float)message.Values[0]) * Screen.width;
//				float yPos = (float)message.Values[1] * Screen.height;
//				Instantiate(warpParticles[DebugMode.CURWARPPARTICLE], Camera.main.ScreenToWorldPoint(new Vector3(xPos, yPos, 4f)), Quaternion.Euler(0f, 180f, 0f));
//			} else {
//				ArrayList args = new ArrayList();
//				args.Add(0f);
//				args.Add(0f);
//				args.Add(0);
//				//BallHit(args);
//			}
//		} else if(message.Address == "/shoot"){
//			if(mode != GameMode.CONFIG) {
//				//whiteScreen.SetActive(false);
//				//BallHit(message.Values); 
//			}
//		} else if(message.Address == "/config/done") {
////			print ("config done");
//			Application.LoadLevel("Intro");
//			//mode = GameMode.STANDBY;
//		} else if(message.Address == "/config/start") {
//			if(mode != GameMode.CONFIG) {
//				StopAllCoroutines();
//				Enemy[] enemies = FindObjectsOfType<Enemy>();
//				foreach(Enemy enemy in enemies) {
//					enemy.StopAllCoroutines();
//				}
//
//				Spider[] spiders = FindObjectsOfType<Spider>();
//				foreach(Spider spider in spiders) {
//					spider.StopAllCoroutines();
//				}
//
//				StaticPool.DestroyAllObjects();
//
//				//mode = GameMode.CONFIG;
//				Application.LoadLevel("Config");
//			}
//		} else if(message.Address == "/config/noKinect") {
//			if(mode == GameMode.CONFIG) {
//				kinectErrorObj.SetActive(true);
//			}
//		} else if(message.Address == "/config/kinectFound") {
//			if(mode == GameMode.CONFIG) {
//				kinectErrorObj.SetActive(false);
//			}
//		}
	}

	public void AddScore(int p_score) 
	{
		score += p_score * streakMultiplier;
		AddToStreak ();
	}

	public void StartTutorialOrCountdown(bool tutorialOn)
	{
		inTutorialMode = tutorialOn;
		if (inTutorialMode) {
			SwitchGameMode (GameMode.TUTORIAL);
		} else {
			SwitchGameMode (GameMode.COUNTDOWN);
		}
	}

	public void StartGame()
	{
		SwitchGameMode (GameMode.GAME);
	}

	public void AddToStreak()
	{
		streak += 1;
	}

	public void ResetStreak()
	{
		streak = 0;
	}
}
 