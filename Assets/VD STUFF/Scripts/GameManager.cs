using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private static GameManager _instance;

	enum GameMode {Intro, Main, Scoreboard, Config}

	private GameObject scoreText;
	private int score;

	public GameObject enemy;
	public Transform enemySpawnPoint;
	public float spawnRate = 3f;

	public bool randomSpawnTime = false;
	bool screenChanged;
	public float randomRange = 1f;

	GameMode mode = GameMode.Intro;
	bool gameStarted = false;

	public float joinTimer = 5f;
	public float gameTimer = 180f;
	public float scoreboardTimer = 15f;

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

	IntroGUI introGUI;

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

		//Application.LoadLevel ("Config");
	}

	void OnLevelWasLoaded(int level) {
		if(level == 0) {
			introGUI = GameObject.Find("IntroGUI").GetComponent<IntroGUI>();
			gameStarted = false;
		} else if(level == 1) {
			StaticPool.DestroyAllObjects(); // Ghetto fix for now. Wasting an allocation somewhere also I think.
			queueManager = GameObject.Find( "QueueManager" ).GetComponent<QueueManager>();

			// Find Spider and set corresponding values
			InitSpider();

//			redGameScore = GameObject.Find("RedScore").GetComponent<GUIText>();
//			yellowGameScore = GameObject.Find("YellowScore").GetComponent<GUIText>();
//			greenGameScore = GameObject.Find("GreenScore").GetComponent<GUIText>();
//			purpleGameScore = GameObject.Find("PurpleScore").GetComponent<GUIText>();

			redScoreBox = GameObject.Find( "InGameScoreRed" );
//			yellowScoreBox = GameObject.Find( "InGameScoreYellow" );
//			blueScoreBox = GameObject.Find( "InGameScoreBlue" );
			greenScoreBox = GameObject.Find( "InGameScoreGreen" );

			StartCoroutine ("SpawnEnemy");
			StartCoroutine( "StartEnemyMove" );
			GameObject.Find("GameCamera").GetComponent<Camera>().enabled = true;
			GameObject.Find("Timer").SetActive(true);

			TextMesh[] texts = redScoreBox.GetComponentsInChildren<TextMesh>();			
			foreach( TextMesh text in texts ) {
				if( text.name.Contains( "Score" ) )
					redScoreTxt = text;
				if( text.name.Contains( "Accuracy" ) )
					redAccTxt = text;
			}		

//			texts = yellowScoreBox.GetComponentsInChildren<TextMesh>();			
//			foreach( TextMesh text in texts ) {
//				if( text.name.Contains( "Score" ) )
//					yellowScoreTxt = text;
//				if( text.name.Contains( "Accuracy" ) )
//					yellowAccTxt = text;
//			}			
//			texts = blueScoreBox.GetComponentsInChildren<TextMesh>();			
//			foreach( TextMesh text in texts ) {
//				if( text.name.Contains( "Score" ) )
//					blueScoreTxt = text;
//				if( text.name.Contains( "Accuracy" ) )
//					blueAccTxt = text;
//			}			
			texts = greenScoreBox.GetComponentsInChildren<TextMesh>();			
			foreach( TextMesh text in texts ) {
				if( text.name.Contains( "Score" ) )
					greenScoreTxt = text;
				if( text.name.Contains( "Accuracy" ) )
					greenAccTxt = text;
			}

			redScoreBox.SetActive( false );
//			yellowScoreBox.SetActive( false );
//			blueScoreBox.SetActive( false );
			greenScoreBox.SetActive( false );
		} else if(level == 2) { //Config level
			OSCSender.SendEmptyMessage("/config/start");
			mode = GameMode.Config;
			kinectErrorObj = GameObject.Find("KinectError");
			kinectErrorObj.SetActive(false);
		}
	}

	void Update() {

		//Score
		scoreText = GameObject.Find ("ScoreText");
		scoreText.GetComponent<Text> ().text = "Score: " + score;

		if(DebugMode.SHOWGUI) {
			if(Input.GetKeyDown(KeyCode.R)) {
				QualitySettings.DecreaseLevel();
			}
			if(Input.GetKeyDown(KeyCode.T)) {
				QualitySettings.IncreaseLevel();
			}
		}
		
		switch(mode)
		{
		case GameMode.Intro:
			if(gameStarted) {
				if(timer > 0f) {
					timer -= Time.deltaTime;
					if(timer <= 0f) {
						timer = 0f;
						StartCoroutine("ShowInstructions");
//						ChangeScene( "Main" );
						return;
					}
				}
			}
			if(Input.GetKeyDown(KeyCode.C)) {
				Application.LoadLevel("Config");

				ballManager.StopAllCoroutines();
				StopAllCoroutines();
				
				Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
				foreach( Enemy enemy in enemies ) {
					enemy.StopAllCoroutines();
//					enemy.gameObject.SetActive( false );
				}
				
				Spider[] spiders = GameObject.FindObjectsOfType<Spider>();
				foreach(Spider spider in spiders ) {
					spider.StopAllCoroutines();
//					spider.gameObject.SetActive( false );
				}
				
				StaticPool.DestroyAllObjects();

			}
			break;
		case GameMode.Main:
			if(Input.GetKeyDown(KeyCode.C)) {
				Application.LoadLevel("Config");

				ballManager.StopAllCoroutines();
				StopAllCoroutines();

				Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
				foreach( Enemy enemy in enemies ) {
					enemy.StopAllCoroutines();
//					enemy.gameObject.SetActive( false );
				}

				Spider[] spiders = GameObject.FindObjectsOfType<Spider>();
				foreach(Spider spider in spiders ) {
					spider.StopAllCoroutines();
//					spider.gameObject.SetActive( false );
				}

				StaticPool.DestroyAllObjects();
			}
			// Once timer goes down to zero
			if(timer <= 0) {
				ballManager.StopAllCoroutines();
				StopAllCoroutines();
				timer = scoreboardTimer;
				mode = GameMode.Scoreboard;

				redScoreBox.SetActive( false );
//				yellowScoreBox.SetActive( false );
//				blueScoreBox.SetActive( false );
				greenScoreBox.SetActive( false );

				GameObject.Find("ScoreGUI").GetComponent<ScoreGUI>().Activate();
				GameObject.Find("Timer").SetActive(false);

				for(int i = 0; i < playerManager.playerData.Count; i++) {
					HighScoreManager.AddScore(playerManager.playerData[i].score);
				}

				Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
				foreach( Enemy enemy in enemies ) {
					enemy.StopAllCoroutines();
//					enemy.gameObject.SetActive( false );
				}

				// Fuck you eric
				Spider[] spiders = GameObject.FindObjectsOfType<Spider>();
				foreach(Spider spider in spiders ) {
					spider.StopAllCoroutines();
//					spider.gameObject.SetActive( false );
				}

				queueManager.Reset();

				return;
			}

			// Update score gui
			if( redScoreBox.activeSelf == false && playerManager.Added( PlayerColor.Red ))
				redScoreBox.SetActive( true );
//			if( yellowScoreBox.activeSelf == false && playerManager.Added( PlayerColor.Yellow ))
//				yellowScoreBox.SetActive( true );
//			if( blueScoreBox.activeSelf == false && playerManager.Added( PlayerColor.Blue ))
//				blueScoreBox.SetActive( true );
			if( greenScoreBox.activeSelf == false && playerManager.Added( PlayerColor.Green ))
				greenScoreBox.SetActive( true );

			for(int i = 0; i < playerManager.playerData.Count; i++) {
				string tempScoreStr = playerManager.playerData[i].score.ToString();

				string tempAccStr;
				if( playerManager.playerData[i].totalShots > 0 ) {
					float percent = ((float)playerManager.playerData[i].shotsHit/(float)playerManager.playerData[i].totalShots) * 100f;
					tempAccStr = ((int)percent).ToString();
				} else {
					tempAccStr = "0";
				}
				
				if(tempScoreStr.Length < 2)
					tempScoreStr = " " + tempScoreStr;
				if(tempAccStr.Length < 2)
					tempAccStr = " " + tempAccStr;
				
				switch( playerManager.playerData[i].color ) {
				case PlayerColor.Red:
					redScoreTxt.text = "Score: " + tempScoreStr;
					redAccTxt.text = "Accuracy: " + tempAccStr + "%";
					break;
//				case PlayerColor.Yellow:
//					yellowScoreTxt.text = "Score: " + tempScoreStr;
//					yellowAccTxt.text = "Accuracy: " + tempAccStr + "%";
//					break;
				case PlayerColor.Green:
					greenScoreTxt.text = "Score: " + tempScoreStr;
					greenAccTxt.text = "Accuracy: " + tempAccStr + "%";
					break;
//				case PlayerColor.Blue:
//					blueScoreTxt.text = "Score: " + tempScoreStr;
//					blueAccTxt.text = "Accuracy: " + tempAccStr + "%";
//					break;
				}
			}

			timer -= Time.deltaTime;
			break;
		case GameMode.Scoreboard:
			if(Input.GetKeyDown(KeyCode.C)) {
				Application.LoadLevel("Config");
			}
			if(timer <= 0) {
				ChangeScene( "Intro" );
				return;
			}
			timer -= Time.deltaTime;
			break;
		case GameMode.Config:
			if(Input.GetKeyDown(KeyCode.V)) {
				mode = GameMode.Intro;
				Application.LoadLevel("Intro");
			}
			break;
		}
	}

	public void BallHit(ArrayList args) {
		float x = (float)(args[0]);
		x = Mathf.Abs(x - 1);
		float y = (float)(args[1]);
		y = Mathf.Abs(y - 1);
		Vector2 pos = new Vector2(x,y);

		int colorID =  (int)args[2];

		PlayerColor color = PlayerColor.Red;

		switch(colorID) {
		case 0:
			color = PlayerColor.Red;
			break;
		case 3:
			color = PlayerColor.Green;
			break;
//		case 1:
//			color = PlayerColor.Yellow;
//			break;
//		case 2:
//			color = PlayerColor.Blue;
//			break;
		default:
			print("Bad Color");
			break;
		}

		if(!playerManager.Added(color)) {
			playerManager.AddPlayer(color);
			if(mode == GameMode.Intro) {
				//introGUI.TurnOnColor(color);
				timer = joinTimer;
			}
		}
		if(!gameStarted) {
			gameStarted = true;
			timer = joinTimer;
		}

		if(mode == GameMode.Main) {
			pos.x *= Screen.width;
			pos.y = 1 - pos.y;
			pos.y *= Screen.height;
			ballManager.Shoot(pos, color);
		}
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
			mode = GameMode.Intro;			
			Application.LoadLevel("Intro");
			break;
		case "Main":
			timer = gameTimer;
			mode = GameMode.Main;
			Application.LoadLevel("Main");
			break;
		}
	}

	public void OSCMessageReceived(OSC.NET.OSCMessage message){
		if(message.Address == "/checkColor") {
			if(mode != GameMode.Intro) {
				print ("RENDERER IS TRUE");
				Camera.main.GetComponent<screenChange>().screenChanged = true;
				float xPos = (1 - (float)message.Values[0]) * Screen.width;
				float yPos = (float)message.Values[1] * Screen.height;
				Instantiate(warpParticles[DebugMode.CURWARPPARTICLE], Camera.main.ScreenToWorldPoint(new Vector3(xPos, yPos, 4f)), Quaternion.Euler(0f, 180f, 0f));
			} else {
				ArrayList args = new ArrayList();
				args.Add(0f);
				args.Add(0f);
				args.Add(0);
				BallHit(args);
			}
		} else if(message.Address == "/shoot"){
			if(mode != GameMode.Config) {
				//whiteScreen.SetActive(false);
				BallHit(message.Values); 
			}
		} else if(message.Address == "/config/done") {
//			print ("config done");
			Application.LoadLevel("Intro");
			mode = GameMode.Intro;
		} else if(message.Address == "/config/start") {
			if(mode != GameMode.Config) {
				StopAllCoroutines();
				Enemy[] enemies = FindObjectsOfType<Enemy>();
				foreach(Enemy enemy in enemies) {
					enemy.StopAllCoroutines();
				}

				Spider[] spiders = FindObjectsOfType<Spider>();
				foreach(Spider spider in spiders) {
					spider.StopAllCoroutines();
				}

				StaticPool.DestroyAllObjects();

				mode = GameMode.Config;
				Application.LoadLevel("Config");
			}
		} else if(message.Address == "/config/noKinect") {
			if(mode == GameMode.Config) {
				kinectErrorObj.SetActive(true);
			}
		} else if(message.Address == "/config/kinectFound") {
			if(mode == GameMode.Config) {
				kinectErrorObj.SetActive(false);
			}
		}
	}
	
	/// <summary>
	/// Finds spider in scene and sets values corresponding to it.
	/// </summary>
	private void InitSpider() {
		// Find spider.
		gameSpider = FindObjectOfType<Spider>();

		if( gameSpider == null ) {
			Debug.LogWarning( "No spider found in scene." );
			return;
		}

		// Calculate interval of spiders appearing in game.
		float appearInterval = gameTimer / (float)numSpiderAppearances;

		gameSpider.Init( numSpiderAppearances, appearInterval );
	}
	
	public void AdjustGameSetting(string setting, float value) {
		switch(setting) 
		{
		case "Game Time":
			gameTimer = value;
			break;
		default:
			break;
		}
	}

	public void AdjustGameSetting(string setting, bool value) {
		switch(setting)
		{
		case "Quit Game":
			ChangeScene( "Intro" );
			break;
		default:
			break;
		}
	}

	public void AddScore(int p_score) {
		score += p_score;
	}
}
 