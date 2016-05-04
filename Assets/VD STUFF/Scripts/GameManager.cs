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
		SCORECARD,
		CONFIG
	}

	enum GamePhase
	{
		WARMUP,
		ONE, //2 enemies at a time
		TWO, //6 enemies
		THREE, //unlimited enemies
		END //Game over/reset
	}

	private TextManager textManager;
	//private TutorialManager tm;
	private AudioManager am;
	private AimAssistManager aam;
	private AutoCenter ac;

	private bool firstCalibrationDone;
	private float firstCalibrationTimer = 0f;

	public GameObject player;

	private int score;
	private int streak = 0;
	public int streakMultiplier;
	bool newHighScore = false;
	int bestStreak;

	public GameObject enemy;
	public Transform throwDestination;

    public List<Material> gridMats;

	GameMode mode = GameMode.STANDBY;
	GamePhase phase = GamePhase.ONE;
	float phaseTimer;
	bool moveEnemyIsRunning;
	bool gameStarted = false;
	//public bool inTutorialMode;
	public int gamePhaseInt;

	public bool easyMode;

	public SpawnFloor spawnFloor;

	public float gameTimer = 10f;

	public Material skybox;
	float skyboxRot;

	public GameObject batHoldBox;
	public GameObject ballPrefab;

	public List<EnemyBall> activeBalls;

	public int warmUpBallsThrown;
	public int warmUpBallsDone;

	[System.NonSerialized]
	public float timer = 0f;

	BallManager ballManager;
	QueueManager queueManager;

	StaticPool staticPool;

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

		ac = GameObject.FindGameObjectWithTag ("Player").GetComponent<AutoCenter> ();
	}
	#endregion

	void Start() {
		ballManager = GetComponent<BallManager> ();
		//tm = GetComponent<TutorialManager> ();
		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		aam = GetComponent<AimAssistManager> ();
		textManager = GetComponent<TextManager> ();

		queueManager = GameObject.Find( "QueueManager" ).GetComponent<QueueManager>();

		ac.Calibrate ();

		CheckDebugInfoLog ();

		gameTimer += 5;

		SwitchGameMode(GameMode.STANDBY);
	}

	void Update() {

		if (firstCalibrationTimer < 0.1 && !firstCalibrationDone)
			firstCalibrationTimer += Time.deltaTime;

		if (firstCalibrationTimer > 0.1 && !firstCalibrationDone) 
		{
			ac.Calibrate ();
			firstCalibrationDone = true;
		}

		skyboxRot -= Time.deltaTime;
		skybox.SetFloat("_Rotation", skyboxRot);

		switch(mode)
		{
		case GameMode.STANDBY:

			if (Input.GetKeyDown (KeyCode.Space)) {
				//SwitchGameMode (GameMode.COUNTDOWN);
				StartCountdown(true);
			}

//			if (Input.GetKeyDown (KeyCode.T)) {
//				SwitchGameMode (GameMode.TUTORIAL);
//				//inTutorialMode = true;
//			}

			if (Input.GetKeyDown (KeyCode.C)) {
				SwitchGameMode (GameMode.CONFIG);
			}

			break;

		case GameMode.COUNTDOWN:
			foreach( Material mat in gridMats )
				mat.SetFloat("_Opacity_Slider", timer * 6f );
			//gridMats [1].SetFloat ("_Opacity_Slider", timer * .714f);

			if (timer <= 0) 
			{
				SwitchGameMode (GameMode.GAME);
				return;
			} 
			else if (timer <= 1) 
			{
				textManager.countdownText.text = "Go!";
			} 
			else if (timer >= 4)
			{
				textManager.countdownText.text = "Get Ready!";
			}
			else
			{
				textManager.countdownText.text = ((int)timer).ToString();
			}

			timer -= Time.deltaTime;
			break;

		case GameMode.TUTORIAL:


			break;

		case GameMode.GAME:
			switch (phase) {
			case GamePhase.WARMUP:
				if (warmUpBallsThrown >= 3 && warmUpBallsDone >= 3) {
					for (int i = 0; i < aam.onCourtEnemies.Count; i++) 
					{
						if (aam.onCourtEnemies [i].GetComponent<Enemy> ().inWarmUp == true)
							aam.onCourtEnemies [i].GetComponent<Enemy> ().CallFade ();
					}

					SwitchGamePhase (GamePhase.ONE);
					return;
				}

				if (aam.onCourtEnemies.Count >= 1)
				{
					if (moveEnemyIsRunning)
					{
						StopCoroutine ("StartEnemyMove");
						moveEnemyIsRunning = false;
					}
				}
				else
				{
					if (!moveEnemyIsRunning)
						StartCoroutine ("StartEnemyMove");
				}

				textManager.hitXBallsNumberText.text = (3 - warmUpBallsDone).ToString();

				break;

			case GamePhase.ONE:
				if (aam.onCourtEnemies.Count >= 3)
                { 
					if (moveEnemyIsRunning)
                    {
						StopCoroutine ("StartEnemyMove");
						moveEnemyIsRunning = false;
					}
				}
                else
                {
					if (!moveEnemyIsRunning)
						StartCoroutine ("StartEnemyMove");
				}

				phaseTimer -= Time.deltaTime;
				if (phaseTimer <= 0) {
					SwitchGamePhase (GamePhase.TWO);
				}

				if (score >= 3) {
					SwitchGamePhase (GamePhase.TWO);
				}

				break;

			case GamePhase.TWO:
				if (aam.onCourtEnemies.Count >= 6) {
					if (moveEnemyIsRunning) {
						StopCoroutine ("StartEnemyMove");
						moveEnemyIsRunning = false;
					}

				} else {
					if (!moveEnemyIsRunning)
						StartCoroutine ("StartEnemyMove");
				}

				phaseTimer -= Time.deltaTime;
				if (phaseTimer <= 0) {
					SwitchGamePhase (GamePhase.THREE);
				}

				if (score >= 10) {
					SwitchGamePhase (GamePhase.THREE);
				}

				break;

			case GamePhase.THREE:
				if (aam.onCourtEnemies.Count >= 6) {
					if (moveEnemyIsRunning) {
						StopCoroutine ("StartEnemyMove");
						moveEnemyIsRunning = false;
					}

				} else {
					if (!moveEnemyIsRunning)
						StartCoroutine ("StartEnemyMove");
				}

				break;
			}

			//SCORE TEXT
			textManager.scoreText.text = "Score: " + score;

			//STREAK TEXT
			textManager.streakText.text = "Streak: " + streak + " (x" + streakMultiplier + ")";
			streakMultiplier = Mathf.Clamp (1 + Mathf.Clamp (streak, 0, 1) + (int)(streak / 3), 1, 3);

            if (timer <= 0)
            {
                timer = 0;
				textManager.timerText.text = "Time: 0:00";

                Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    enemy.StopCoroutine("ThrowRoutine");
                }

                if (!BallsAreStillInAir())
                {
                    //ballManager.StopAllCoroutines ();
                    StopAllCoroutines();
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.StopAllCoroutines();
                        enemy.gameObject.SetActive(false);
                    }

                    //timer = scoreboardTimer;
                    SwitchGameMode(GameMode.GAMEOVER);

                    // Fuck you eric
                    spawnFloor.ResetTilesFilled();
                    queueManager.Reset();

                    return;
                }
            }
            else
            {
				if (phase == GamePhase.WARMUP)
					timer = gameTimer;
				else
					timer -= Time.deltaTime;

                //TIMER TEXT
                int minutes = Mathf.FloorToInt(timer / 60F);
                int seconds = Mathf.FloorToInt(timer - minutes * 60);
                string stringTimer = string.Format("{0:0}:{1:00}", minutes, seconds);
				textManager.timerText.text = "Time: " + stringTimer;

				if (gamePhaseInt == 2 && timer > gameTimer - 4) 
				{
					textManager.gameStartingText.gameObject.SetActive (true);
				} 
				else if (gamePhaseInt >= 2 && timer <= gameTimer - 4)
				{
					textManager.gameStartingText.gameObject.SetActive (false);
					textManager.timerText.gameObject.SetActive (true);
					textManager.scoreText.gameObject.SetActive (true);
					textManager.streakText.gameObject.SetActive (true);
				}

                if (timer < 6)
                {
                    if (!am.playBeepOnce)
                        am.StartCoroutine("CountdownBoopRoutine", timer);
                }
            }

			break;
		case GameMode.GAMEOVER:
			foreach (Material mat in gridMats)
				mat.SetFloat ("_Opacity_Slider", 30f - timer * 10f);
			//gridMats [1].SetFloat ("_Opacity_Slider", 7f - timer * 2.3f);

			if( timer <= 0 )
			{
				SwitchGameMode( GameMode.SCORECARD );
				return;
			}

			timer -= Time.deltaTime;
			break;
		case GameMode.SCORECARD:
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
			break;
		}
	}

	void SwitchGameMode( GameMode gm )
	{
		switch( gm )
		{
		case GameMode.STANDBY:
			ac.Calibrate ();

			gamePhaseInt = 0;
			StaticPool.DestroyAllObjects ();
			textManager.countdownText.gameObject.SetActive (false);
			textManager.scoreText.gameObject.SetActive (false);
			textManager.timerText.gameObject.SetActive (false);
			textManager.streakText.gameObject.SetActive (false);
			//textManager.finalScoreText.gameObject.SetActive (false);
			DeactivateScoreCard();

			batHoldBox.SetActive (true);
			foreach (Material mat in gridMats)
				mat.SetFloat ("_Opacity_Slider", 30f);
			//gridMats [1].SetFloat ("_Opacity_Slider", 7f);
			break;
		case GameMode.COUNTDOWN:
			ac.Calibrate ();

			timer = 5f;
			textManager.countdownText.gameObject.SetActive (true);
			batHoldBox.SetActive (false);

			textManager.warmUpText.gameObject.SetActive (true);

			aam.AdjustAimAssist (easyMode);
			AdjustThrowDestinationHeightForNewPlayer ();

			newHighScore = false;
			bestStreak = 0;
			score = 0;
			streak = 0;
			streakMultiplier = 1;
			warmUpBallsThrown = 0;
			warmUpBallsDone = 0;
			break;
		case GameMode.TUTORIAL:
			textManager.countdownText.gameObject.SetActive (false);
			batHoldBox.SetActive (false);

			//inTutorialMode = true;
			//tm.StartTutorial();

			break;
		case GameMode.GAME:

			aam.ClearOnCourtEnemies ();
			SwitchGamePhase (GamePhase.WARMUP);

			GameObject[] staticPoolBalls = GameObject.FindGameObjectsWithTag ("Ball");
			if (staticPoolBalls.Length >= 1) 
			{
				foreach (GameObject ball in staticPoolBalls) 
				{
					ball.SetActive (false);
				}
			}

			am.PlayBackgroundMusic ();
			timer = gameTimer;
			textManager.countdownText.gameObject.SetActive (false);

			//StaticPool.DisableAllObjects();
			//StaticPool.DestroyAllObjects(); // Ghetto fix for now. Wasting an allocation somewhere also I think.
			queueManager.Reset();
			break;
		case GameMode.GAMEOVER:
			if (score > GetHighScore ()) 
			{
				SetNewHighScore (score);
				newHighScore = true;
			}

			timer = 3f;

			textManager.timerText.text = "Time: 0:00";
			textManager.scoreText.text = "Score: " + score;
			textManager.streakText.text = "Streak: " + streak + " (x" + streakMultiplier + ")";
			//textManager.finalScoreText.text = "Score: " + score;
			//textManager.finalScoreText.gameObject.SetActive (true);
			ActivateScoreCard();

			am.PlayAmbientCubeAudio ();
            am.StopAllCoroutines();
            aam.ClearOnCourtEnemies ();
			break;

		case GameMode.SCORECARD:
			timer = 7f;
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
		case GamePhase.WARMUP:
//			textManager.timerText.gameObject.SetActive (false);
//			textManager.scoreText.gameObject.SetActive (false);
//			textManager.streakText.gameObject.SetActive (false);

			textManager.warmUpText.gameObject.SetActive (false);
			textManager.hitXBallsText.gameObject.SetActive (true);
			textManager.hitXBallsNumberText.gameObject.SetActive (true);

			gamePhaseInt = 1;

			StartCoroutine ("SpawnEnemy");
			moveEnemyIsRunning = false;

			break;
		case GamePhase.ONE:
			textManager.hitXBallsText.gameObject.SetActive (false);
			textManager.hitXBallsNumberText.gameObject.SetActive (false);

//			textManager.timerText.gameObject.SetActive (true);
//			textManager.scoreText.gameObject.SetActive (true);
//			textManager.streakText.gameObject.SetActive (true);

			score = 0;
			streak = 0;
			streakMultiplier = 1;

			StartCoroutine( "StartEnemyMove" );

			phaseTimer = 30f;
			gamePhaseInt++;

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

	bool BallsAreStillInAir()
	{
		for (int i = 0; i < activeBalls.Count; i++) 
		{
			if (activeBalls[i].hitGround == true || activeBalls[i].gameObject.activeSelf == false) 
				activeBalls.Remove (activeBalls[i]);
		}

		if (activeBalls.Count > 0)
			return true;
		else
			return false;
	}

	IEnumerator SpawnEnemy() {
		while(true) {
			queueManager.SpawnNewEnemy( enemy );
			//Debug.Log ("Spawning Enemy from GM");
			yield return new WaitForSeconds( 0.25f );
		}
	}

	IEnumerator StartEnemyMove() {
		while( true ) {
			queueManager.StartNextInQueue();
			//Debug.Log ("Start Enemy Move from GM");
			moveEnemyIsRunning = true;
			yield return new WaitForSeconds( 1.5f );
		}
	}

	void ChangeScene( string scene ) {
		switch( scene )
		{
		case "Intro":
			timer = 0f;
			gameStarted = false;
			//playerManager.playerData.Clear();
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

	public void AddScore(int p_score) 
	{
		score += p_score * streakMultiplier;
		AddToStreak ();
	}

    public void StartCountdown(bool mode)
    {
		easyMode = mode;
    	SwitchGameMode(GameMode.COUNTDOWN);
    }

    public void StartGame()
	{
		SwitchGameMode (GameMode.GAME);
	}

	public void AddToStreak()
	{
		streak += 1;

		if (streak > bestStreak)
			bestStreak = streak;
	}

	public void ResetStreak()
	{
		streak = 0;
	}

	void AdjustThrowDestinationHeightForNewPlayer()
	{
		throwDestination.position = new Vector3(throwDestination.position.x, player.transform.position.y - 0.15f, throwDestination.position.z);
	}

	void ActivateScoreCard()
	{
		textManager.scoreCardTrans.gameObject.SetActive (true);
		if (newHighScore)
			textManager.scoreCardNewHighScoreTrans.gameObject.SetActive (true);
		else
			textManager.scoreCardNewHighScoreTrans.gameObject.SetActive (false);

		textManager.scHighScore.text = GetHighScore ().ToString();
		textManager.scScore.text = score.ToString();
		textManager.scBestStreak.text = bestStreak.ToString();
	}

	void DeactivateScoreCard()
	{
		textManager.scoreCardTrans.gameObject.SetActive (false);
		if (textManager.scoreCardNewHighScoreTrans.gameObject.activeSelf == true)
			textManager.scoreCardNewHighScoreTrans.gameObject.SetActive (false);
	}

	int GetHighScore()
	{
		string filePath = Application.persistentDataPath + "/SkyzoneDebugInfo.txt";

		string[] debugFile = System.IO.File.ReadAllLines(filePath);

		if( debugFile.Length <= 0 )
			return 0;

		return int.Parse( debugFile[0] );
	}

	void SetNewHighScore( int score )
	{
		string filePath = Application.persistentDataPath + "/SkyzoneDebugInfo.txt";

		string[] debugFile = System.IO.File.ReadAllLines(filePath);

		debugFile[0] = score.ToString();

		System.IO.File.WriteAllLines( filePath, debugFile );
	}

	int GetGameTime()
	{
		string filePath = Application.persistentDataPath + "/SkyzoneDebugInfo.txt";

		string[] debugFile = System.IO.File.ReadAllLines(filePath);

		if( debugFile.Length <= 1 )
			return 75; //default game time

		return int.Parse( debugFile[1] );
	}

	void SetGameTimeLog( int time )
	{
		string filePath = Application.persistentDataPath + "/SkyzoneDebugInfo.txt";
		string[] debugFile = System.IO.File.ReadAllLines(filePath);

		if( debugFile.Length <= 1 )
			System.IO.File.AppendAllText( filePath, "\n" + time.ToString() );
		else
		{
			debugFile[1] = time.ToString();
			System.IO.File.WriteAllLines( filePath, debugFile );
		}
	}

	void CheckDebugInfoLog()
	{
		string filePath = Application.persistentDataPath + "/SkyzoneDebugInfo.txt";
		if(!System.IO.File.Exists(filePath)) //TODO: check to see if certain info is missing
		{
			//System.IO.File.Create(filePath);
			string[] debugInfo = new string[2];

			debugInfo[0] = "0";
			debugInfo[1] = "75";

			System.IO.File.WriteAllLines( filePath, debugInfo );
		}
	}
}
 