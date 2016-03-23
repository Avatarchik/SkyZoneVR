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
		ROTATE,
		HITBALL,
		HITENEMY,
		ENEMYJUMP,
		ENEMYTHROW
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
	TutorialState tutMode = TutorialState.HITENEMY;

	public SpawnFloor spawnFloor;

	public float joinTimer = 5f;
	public float gameTimer = 10f;
	public float scoreboardTimer = 15f;

	public GameObject batHoldBox;
	public GameObject ballPrefab;

	//Tutorial Variables
	public GameObject[] tutorialEnemies;
	public int tutorialEnemiesActive;
	GameObject tutorialBall;
	Vector3 tutorialBallSpawnPos;
	bool inTutorialMode;
	float tutBallTimer;
	float tutTransitionTimer = 0f;
	//ROTATE Variables
	float initialYRot;
	float minRot;
	float maxRot;
	float rotationProgress;
	public GameObject rotationBarBG;
	public Image rotationBar;
	Transform cameraTransform;
	//HITBALL Variables
	float tutBallLerpTimer = 0f;

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
		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();

		tutorialBallSpawnPos = GameObject.Find ("TutBallSpawn").transform.position;
		cameraTransform = Camera.main.transform;

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
			switch (tutMode) 
			{
			case TutorialState.ROTATE:

				float rot = cameraTransform.rotation.eulerAngles.y - initialYRot;

				if( rot < 0 )
					rot += 360;

				if ( rot > minRot && Mathf.Abs( rot - minRot ) < 10  )
					minRot = rot;
				else if ( rot < maxRot && Mathf.Abs( rot - maxRot ) < 10 )
					maxRot = rot;

				rotationProgress = 1 - (maxRot - minRot) / 360;// (maxRot - minRot) / 360;

				rotationBar.fillAmount = rotationProgress;

				if ( rotationProgress >= 0.98f )
				{
					rotationBar.fillAmount = 1;
					SwitchTutorialState(TutorialState.HITBALL);
				}

				break;

			case TutorialState.HITBALL:
				if (tutorialBall.GetComponent<EnemyBall> ().fromEnemy) 
				{
					tutBallLerpTimer += Time.deltaTime;

					if (tutBallLerpTimer >= 2)
						tutBallLerpTimer = 2f;

					tutorialBall.transform.position = Vector3.Lerp(transform.position, tutorialBallSpawnPos, tutBallLerpTimer/2f);
					print (tutBallLerpTimer);
				} 
				else 
				{
					tutTransitionTimer += Time.deltaTime;

					foreach( Material mat in gridMats )
						mat.SetFloat("_Opacity_Slider", 2.5f - tutTransitionTimer );

					if (tutTransitionTimer > 2.5f) {
						
						tutTransitionTimer = 0;
						SwitchTutorialState (TutorialState.HITENEMY);
					}
				}

				break;

			case TutorialState.HITENEMY:

				if (tutorialBall.GetComponent<EnemyBall> ().tutorialBall == false) {
					tutBallTimer -= Time.deltaTime;
				}

				if (tutBallTimer < 0) {
					tutorialBall = null;
					TutorialBallSpawn (tutorialBallSpawnPos);
					tutBallTimer = 2.5f;
				}

				if (tutorialEnemiesActive < 4) 
				{
					tutTransitionTimer += Time.deltaTime;
					tutorialBall.SetActive (false);

					if (tutTransitionTimer > 3) 
					{
						SwitchTutorialState (TutorialState.ENEMYJUMP);
						tutTransitionTimer = 0f;
					}						
				}

				break;

			case TutorialState.ENEMYJUMP:
				if (tutorialBall.GetComponent<EnemyBall> ().tutorialBall == false) {
					tutBallTimer -= Time.deltaTime;
				}

				if (tutBallTimer < 0) {
					tutorialBall = null;
					TutorialBallSpawn (tutorialBallSpawnPos);
					tutBallTimer = 2.5f;
				}

				if (tutorialEnemiesActive < 4) 
				{
					tutTransitionTimer += Time.deltaTime;
					tutorialBall.SetActive (false);

					if (tutTransitionTimer > 3) 
					{
						SwitchTutorialState (TutorialState.ENEMYTHROW);
						tutTransitionTimer = 0f;
					}	
				}

				break;

			case TutorialState.ENEMYTHROW:

				if (tutorialEnemiesActive < 4) 
				{
					SwitchGameMode (GameMode.GAME);
				}

				break;
			}

			break;

		case GameMode.GAME:
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

	void SwitchGameMode( GameMode gm )
	{
		switch( gm )
		{
		case GameMode.STANDBY:
			StaticPool.DestroyAllObjects ();
			countdownText.SetActive (false);
			scoreText.SetActive (false);
			timerText.SetActive (false);
			streakText.SetActive (false);
			finalScoreText.SetActive (false);
			batHoldBox.SetActive (true);
			rotationBarBG.SetActive (false);
			tutorialEnemiesActive = 0;
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
			tutorialEnemiesActive = 0;
			break;
		case GameMode.TUTORIAL:
			countdownText.SetActive (false);
			batHoldBox.SetActive (false);
			SwitchTutorialState (TutorialState.ROTATE);

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
		case TutorialState.ROTATE:
			
			initialYRot = cameraTransform.rotation.eulerAngles.y;
			minRot = 0;
			maxRot = 360;
			rotationBarBG.SetActive (true);
			rotationBar.gameObject.SetActive (true);
			break;

		case TutorialState.HITBALL:
			rotationBarBG.SetActive (false);
			tutBallLerpTimer = 0f;
			TutorialBallSpawn (tutorialBallSpawnPos + new Vector3(0, -3f, 0));
			break;

		case TutorialState.HITENEMY:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;
				tutorialEnemies[i].transform.position = new Vector3(tutorialEnemies[i].transform.position.x, 3f, tutorialEnemies[i].transform.position.z);
			}
			tutorialEnemiesActive = tutorialEnemies.Length;

			tutBallTimer = 2.5f;
			TutorialBallSpawn (tutorialBallSpawnPos);

			break;

		case TutorialState.ENEMYJUMP:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().StartCoroutine ("Hop", tutorialEnemies [i].GetComponent<Enemy> ().tutorialHop);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;
			}
			tutorialEnemiesActive = tutorialEnemies.Length;

			if(tutorialBall.activeSelf == false)
				tutorialBall.SetActive (true);

			tutBallTimer = 1f;
			if(tutorialBall == null)
				TutorialBallSpawn (tutorialBallSpawnPos);

			break;
		
		case TutorialState.ENEMYTHROW:
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

			EnemyBall[] staticPoolBalls = GameObject.Find ("StaticPool").transform.FindChild ("Throwing_Ball").GetComponentsInChildren<EnemyBall> ();
			foreach (EnemyBall ball in staticPoolBalls) 
			{
				ball.tutorialBall = false;
			}

			break;
		}

		tutMode = ts;
	}

	void TutorialBallSpawn(Vector3 spawnPos)
	{
		tutorialBall = StaticPool.GetObj (ballPrefab);
		tutorialBall.GetComponent<EnemyBall> ().Reset ();
		tutorialBall.GetComponent<EnemyBall> ().tutorialBall = true;
		tutorialBall.transform.position = spawnPos;
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

	public void AddScore(int p_score) 
	{
		score += p_score * streakMultiplier;
		AddToStreak ();
	}

	public void StartGame(bool tutorialOn)
	{
		inTutorialMode = tutorialOn;
		if (inTutorialMode) {
			SwitchGameMode (GameMode.TUTORIAL);
		} else {
			SwitchGameMode (GameMode.COUNTDOWN);
		}
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
 