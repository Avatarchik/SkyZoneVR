using UnityEngine;
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

	enum TutorialState
	{
		ONE,
		TWO,
		THREE
	}

	private AudioManager am;

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
	bool gameStarted = false;
	TutorialState tutMode = TutorialState.ONE;

	public SpawnFloor spawnFloor;

	public float joinTimer = 5f;
	public float gameTimer = 10f;
	public float scoreboardTimer = 15f;

	public GameObject batHoldBox;
	public GameObject ballPrefab;

	public GameObject[] tutorialEnemies;
	public int tutorialEnemiesActive;
	bool inTutorialMode;

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
	
//	public TextMesh scoreText;
//	GUIText redGameScore, yellowGameScore, greenGameScore, purpleGameScore; //In-game score gui

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
        // disable cursor
        //Screen.showCursor = false;

		//introGUI = GameObject.Find("IntroGUI").GetComponent<IntroGUI>();
		ballManager = GetComponent<BallManager> ();
		playerManager = GetComponent<PlayerManager> ();
		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();

		scoreText = GameObject.Find ("ScoreText");
		timerText = GameObject.Find ("TimerText");
		streakText = GameObject.Find ("StreakText");
		countdownText = GameObject.Find ("CountdownText");
		finalScoreText = GameObject.Find ("FinalScoreText");
		queueManager = GameObject.Find( "QueueManager" ).GetComponent<QueueManager>();

		for (int i = 0; i < tutorialEnemies.Length; i++) 
		{
			tutorialEnemies [i].SetActive (false);
		}

		SwitchGameMode(GameMode.STANDBY);

		//Application.LoadLevel ("Config");
	}

//	void OnLevelWasLoaded(int level) {
//		if(level == 0) {
//			gameStarted = false;
//		} else if(level == 1) {
//			StaticPool.DestroyAllObjects(); // Ghetto fix for now. Wasting an allocation somewhere also I think.
//			queueManager = GameObject.Find( "QueueManager" ).GetComponent<QueueManager>();
//
////			redGameScore = GameObject.Find("RedScore").GetComponent<GUIText>();
////			yellowGameScore = GameObject.Find("YellowScore").GetComponent<GUIText>();
////			greenGameScore = GameObject.Find("GreenScore").GetComponent<GUIText>();
////			purpleGameScore = GameObject.Find("PurpleScore").GetComponent<GUIText>();
//
////			redScoreBox = GameObject.Find( "InGameScoreRed" );
////			yellowScoreBox = GameObject.Find( "InGameScoreYellow" );
////			blueScoreBox = GameObject.Find( "InGameScoreBlue" );
////			greenScoreBox = GameObject.Find( "InGameScoreGreen" );
//
//			StartCoroutine ("SpawnEnemy");
//			StartCoroutine( "StartEnemyMove" );
////			GameObject.Find("GameCamera").GetComponent<Camera>().enabled = true;
////			GameObject.Find("Timer").SetActive(true);
//				
//		} else if(level == 2) { //Config level
//			OSCSender.SendEmptyMessage("/config/start");
//			mode = GameMode.CONFIG;
//			kinectErrorObj = GameObject.Find("KinectError");
//			kinectErrorObj.SetActive(false);
//		}
//	}

	void Update() {

//		//SCORE TEXT
//		scoreText.GetComponent<Text> ().text = "Score: " + score;
//
//		//TIMER TEXT
//		int minutes = Mathf.FloorToInt(timer / 60F);
//		int seconds = Mathf.FloorToInt(timer - minutes * 60);
//		string stringTimer = string.Format ("{0:0}:{1:00}", minutes, seconds);
//		timerText.GetComponent<Text>().text = "Time: " + stringTimer;

		switch(mode)
		{
		case GameMode.STANDBY:
//			if(gameStarted) {
//				if(timer > 0f) {
//					timer -= Time.deltaTime;
//					if(timer <= 0f) {
//						timer = 0f;
//						StartCoroutine("ShowInstructions");
////						ChangeScene( "Main" );
//						return;
//					}
//				}
//			}
//			if(Input.GetKeyDown(KeyCode.K)) {
//				Application.LoadLevel("Config");
//
//				ballManager.StopAllCoroutines();
//				StopAllCoroutines();
//				
//				Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
//				foreach( Enemy enemy in enemies ) {
//					enemy.StopAllCoroutines();
////					enemy.gameObject.SetActive( false );
//				}
//				
//				Spider[] spiders = GameObject.FindObjectsOfType<Spider>();
//				foreach(Spider spider in spiders ) {
//					spider.StopAllCoroutines();
////					spider.gameObject.SetActive( false );
//				}
//				
//				StaticPool.DestroyAllObjects();
//
//			}
			//countdownText.transform.localScale = Vector3.zero;

			if (Input.GetKeyDown (KeyCode.Space)) {
				SwitchGameMode (GameMode.COUNTDOWN);
				inTutorialMode = false;
			}

			if (Input.GetKeyDown (KeyCode.T)) {
				SwitchGameMode (GameMode.COUNTDOWN);
				inTutorialMode = true;
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
			if (tutorialEnemiesActive <= 2) 
			{
				SwitchGameMode (GameMode.GAME);
			}

			switch (tutMode) 
			{
			case TutorialState.ONE:

				break;

			case TutorialState.TWO:

				break;

			case TutorialState.THREE:

				break;
			}

			break;

		case GameMode.GAME:

//			if(Input.GetKeyDown(KeyCode.K)) {
//				Application.LoadLevel("Config");
//
//				ballManager.StopAllCoroutines();
//				StopAllCoroutines();
//
//				Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
//				foreach( Enemy enemy in enemies ) {
//					enemy.StopAllCoroutines();
////					enemy.gameObject.SetActive( false );
//				}
//
//				Spider[] spiders = GameObject.FindObjectsOfType<Spider>();
//				foreach(Spider spider in spiders ) {
//					spider.StopAllCoroutines();
////					spider.gameObject.SetActive( false );
//				}
//
//				StaticPool.DestroyAllObjects();
//			}
			// Once timer goes down to zero

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
//			if (streak <= 0) 
//			{
//				if (streak >= 1) 
//				{
//					if (streak >= 3) 
//					{
//						streakMultiplier = 3;
//					}
//					else
//					{
//						streakMultiplier = 2;
//					}
//				}
//				else
//				{
//					streakMultiplier = 1;
//				}
//			}

//			if(streak <= 0)
//				streakMultiplier = 1;
//			if (streak >= 1)
//				streakMultiplier = 2;
//			if (streak >= 3)
//				streakMultiplier = 3;

			if(timer <= 0) {
				ballManager.StopAllCoroutines();
				StopAllCoroutines();
				timer = scoreboardTimer;
				SwitchGameMode(GameMode.GAMEOVER);
				//mode = GameMode.GAMEOVER;

				//GameObject.Find("ScoreGUI").GetComponent<ScoreGUI>().Activate();
				//GameObject.Find("Timer").SetActive(false);

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
			foreach( Material mat in gridMats )
				mat.SetFloat("_Opacity_Slider", 2.5f - timer );

			if( timer <= 0 )
			{
				SwitchGameMode( GameMode.STANDBY );
				return;
			}

			timer -= Time.deltaTime;
			break;
		case GameMode.CONFIG:
			if(Input.GetKeyDown(KeyCode.L)) {
				//mode = GameMode.STANDBY;
				Application.LoadLevel("Intro");
			}
			break;
		}
	}

//	public void BallHit(ArrayList args) {
//		float x = (float)(args[0]);
//		x = Mathf.Abs(x - 1);
//		float y = (float)(args[1]);
//		y = Mathf.Abs(y - 1);
//		Vector2 pos = new Vector2(x,y);
//
//		int colorID =  (int)args[2];
//
//		PlayerColor color = PlayerColor.Red;
//
//		switch(colorID) {
//		case 0:
//			color = PlayerColor.Red;
//			break;
//		case 3:
//			color = PlayerColor.Green;
//			break;
////		case 1:
////			color = PlayerColor.Yellow;
////			break;
////		case 2:
////			color = PlayerColor.Blue;
////			break;
//		default:
//			print("Bad Color");
//			break;
//		}
//
//		if(!playerManager.Added(color)) {
//			playerManager.AddPlayer(color);
//			if(mode == GameMode.STANDBY) {
//				//introGUI.TurnOnColor(color);
//				timer = joinTimer;
//			}
//		}
//		if(!gameStarted) {
//			gameStarted = true;
//			timer = joinTimer;
//		}
//
//		if(mode == GameMode.GAME) {
//			pos.x *= Screen.width;
//			pos.y = 1 - pos.y;
//			pos.y *= Screen.height;
//			ballManager.Shoot(pos, color);
//		}
//	}

	void SwitchGameMode( GameMode gm )
	{
		switch( gm )
		{
		case GameMode.STANDBY:
			//playerManager.playerData.Clear ();
			StaticPool.DestroyAllObjects ();
			countdownText.SetActive (false);
			scoreText.SetActive (false);
			timerText.SetActive (false);
			streakText.SetActive (false);
			finalScoreText.SetActive (false);
			batHoldBox.SetActive(true);
			tutorialEnemiesActive = 0;
			am.PlayAmbientCubeAudio ();
			foreach( Material mat in gridMats )
				mat.SetFloat("_Opacity_Slider", 2.5f);
			break;
		case GameMode.COUNTDOWN:
			timer = 5f;
			countdownText.SetActive (true);
			batHoldBox.SetActive (false);
//			if(GameObject.Find("LoadingBarBackground").activeInHierarchy == true)
//				GameObject.Find("LoadingBarBackground").SetActive(false);
			score = 0;
			streak = 0;
			streakMultiplier = 1;
			tutorialEnemiesActive = 0;
			break;
		case GameMode.TUTORIAL:
			countdownText.SetActive (false);
			SwitchTutorialState (TutorialState.ONE);

			break;
		case GameMode.GAME:
			foreach (GameObject enemy in tutorialEnemies)
				enemy.SetActive (false);
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
			break;
		case GameMode.CONFIG:
			break;
		}
		
		mode = gm;
	}

	void SwitchTutorialState(TutorialState ts)
	{
		switch (ts) 
		{
		case TutorialState.ONE:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().StartCoroutine ("Hop", tutorialEnemies [i].GetComponent<Enemy> ().tutorialHop);
				//tutorialEnemies[i].GetComponent<Enemy>().InvokeRepeating("Hop" 2f, 2f);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;
				tutorialEnemies [i].GetComponent<Enemy> ().waitToThrow = true;
				tutorialEnemies [i].GetComponent<Enemy> ().throwWaitTime = i + i;
				tutorialEnemies [i].GetComponent<Enemy> ().StartCoroutine ("ThrowRoutine");
			}
			tutorialEnemiesActive = tutorialEnemies.Length;


			GameObject stationaryBall = StaticPool.GetObj (ballPrefab);
			stationaryBall.transform.position = new Vector3 (0, 5.25f, 7);

			break;

		case TutorialState.TWO:

			break;
		
		case TutorialState.THREE:

			break;
		}

		tutMode = ts;
	}

	IEnumerator SpawnEnemy() {
		while(true) {
			queueManager.SpawnNewEnemy( enemy );
			yield return new WaitForSeconds( 0.25f );
		}
	}

	IEnumerator StartEnemyMove() {
		while( true ) {
			queueManager.StartNextInQueue();
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
	
//	public void AdjustGameSetting(string setting, float value) {
//		switch(setting) 
//		{
//		case "Game Time":
//			gameTimer = value;
//			break;
//		default:
//			break;
//		}
//	}
//
//	public void AdjustGameSetting(string setting, bool value) {
//		switch(setting)
//		{
//		case "Quit Game":
//			ChangeScene( "Intro" );
//			break;
//		default:
//			break;
//		}
//	}

	public void AddScore(int p_score) 
	{
		score += p_score * streakMultiplier;
		AddToStreak ();
	}

	public void StartGame(bool tutorialOn)
	{
		inTutorialMode = tutorialOn;
		SwitchGameMode (GameMode.COUNTDOWN);
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
 