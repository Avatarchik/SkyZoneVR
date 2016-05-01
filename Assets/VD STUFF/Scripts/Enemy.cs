using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour 
{

	[System.NonSerialized]
	public int curColumn;
	[System.NonSerialized]
	public int omittedColumns = 1;
	[System.NonSerialized]
	public int curRow;

	public float jumpRate = 2f;
	public float hopHeight = 2.25f;

	private GameManager gameMan;
	private AudioManager audioMan;
	private TutorialManager tutMan;
	private AimAssistManager aam;
	private EnemyBallManager ebm;

	GameObject player;

	public GameObject ballPrefab;
	public float throwInterval = 2f;
	public float timeToPlayer = 3f;
	Vector3 dir;
	Vector3 playerPos;

	float throwTimer;
	public int jumps = 0;
	public GameObject activeFakeBall;
	public List<GameObject> fakeBalls;
	int powerUpChoice;

    public bool inWarmUp = false;
	bool canThrow = false;
	public bool onCourt;

	public Transform throwPoint;
	private Transform throwDestination;

	public GameObject hitParticle;
	public GameObject hitTextPopUp;

	SpawnFloor floor;

	Animator animator;

	public List<Renderer> allRenderers;
	public Renderer[] renderers;
	Material[] materials;
	public List<Texture> maleTextures;
	public List<Texture> femaleTextures;

	public Rigidbody[] rbs;

	bool hit;
	float lifeEndTime;

	public float fadeOutTime = 1.5f;
	float fadeOutTimer;
	bool fade;

	[System.NonSerialized]
	public GameObject hitBy;

	public class HopData {
		public HopData(Vector3 p_dest, float p_time) {dest = p_dest; time = p_time;}
		public Vector3 dest;
		public float time;
	}

	struct int2 {
		public int2(int p_x, int p_y) {x = p_x; y = p_y;}
		public int x;
		public int y;
	}

	public HopData tutorialHop;// = new HopData();
	public bool inTutorialMode;
	public bool waitToThrow;
	public float throwWaitTime = 0f;

    void Awake()
    {
        //rbs = gameObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            //rb.mass *= 8;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        //tutorialHop = new HopData(transform.position, 1.7f);

        gameMan = GameObject.Find ("GameManager").GetComponent<GameManager>();
		audioMan = GameObject.Find ("AudioManager").GetComponent<AudioManager>();
		tutMan = gameMan.gameObject.GetComponent<TutorialManager> ();
		aam = gameMan.gameObject.GetComponent<AimAssistManager> ();
		ebm = gameMan.gameObject.GetComponent<EnemyBallManager> ();

		floor = GameObject.Find ("Floor").GetComponent<SpawnFloor> ();
		player = GameObject.Find("Player");
		playerPos = player.transform.position;
		throwDestination = GameObject.Find ("ThrowDestination").transform;

		//materials = gameObject.GetComponentsInChildren<Material>();
		renderers = gameObject.GetComponentsInChildren<Renderer> ();

//		tutorialHop.dest = transform.position + new Vector3(0, hopHeight, 0);
//		tutorialHop.time = 2f;

		//renderers = gameObject.GetComponentsInChildren<Renderer> ();
    }

	void OnEnable() 
	{
		tutorialHop = new HopData(new Vector3(transform.position.x, 3f, transform.position.z), 1.7f);

		fadeOutTimer = fadeOutTime;
		fade = false;

		for (int i = 0; i < allRenderers.Count; i++) 
		{
			allRenderers[i].material.color = new Color(allRenderers[i].material.color.r, allRenderers[i].material.color.g, allRenderers[i].material.color.b, 1);
		}

		Reset();
	}
	
	void SetKinematic(bool newValue) {
		Component[] components = animator.GetComponentsInChildren(typeof(Rigidbody));

		foreach (Component c in components) {
			(c as Rigidbody).isKinematic = newValue;
		}

		floor = GameObject.Find ("Floor").GetComponent<SpawnFloor> ();
	}

	void OnLevelWasLoaded(int level) {
		if(level == 0) {
			StopAllCoroutines();
			gameObject.SetActive(false);
		}
		if(level == 1)
			floor = GameObject.Find ("Floor").GetComponent<SpawnFloor> ();
	}

	public void Reset() {
		for(int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(false);
		}

		int randChar = Random.Range(0, 2);
		transform.GetChild(randChar).gameObject.SetActive(true);
		animator = transform.GetChild(randChar).GetComponent<Animator> ();
		if(randChar == 3) {
			int rand = Random.Range(0, maleTextures.Count - 1);
			animator.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = maleTextures[rand];
		} else if(randChar == 4) {
			int rand = Random.Range(0, femaleTextures.Count - 1);
			animator.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = femaleTextures[rand];
		} 

        foreach(Rigidbody rb in rbs)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

		gameObject.layer = 10; //reset layer to enemy layer
		SetKinematic(true);
		curRow = 0;
		hit = false;
		lifeEndTime = -1f;
		animator.enabled = true;
		onCourt = false;
        if (gameMan.gamePhaseInt > 1)
            inWarmUp = false;

		activeFakeBall = fakeBalls [randChar];
        activeFakeBall.SetActive(true);
		ChooseBallPowerUp ();
	}

	void Update() {
		//Die
		if(lifeEndTime > 0f) {
			if(Time.time > lifeEndTime) {
				//gameObject.SetActive(false);
				fade = true;
			}
		}

		//Fade
		if (fade) 
		{
			fadeOutTimer -= Time.deltaTime;
			FadeOut (fadeOutTimer / fadeOutTime);

			if (fadeOutTimer <= 0) 
			{
				fade = false;
				StopAllCoroutines ();
				gameObject.SetActive (false);
				aam.onCourtEnemies.Remove (this.gameObject);
                onCourt = false;
			}
		} 
		else 
		{
//			if (renderers [0].material.color.a == 1 && renderers.Length >= 1)
//				return;
			if (allRenderers [0].material.color.a == 1 && allRenderers.Count >= 1)
				return;
			FadeOut (1);
		}
    }

	void Hit(GameObject p_hitBy) {
		if(!hit) {
			hitBy = p_hitBy;

			if (inTutorialMode) 
			{
				tutMan.tutorialEnemiesActive -= 1;
			}

			aam.onCourtEnemies.Remove (this.gameObject);
            onCourt = false;

			int pointsToAdd = 1;
			gameMan.GetComponent<GameManager> ().AddScore (pointsToAdd);
			audioMan.GetComponent<AudioManager> ().EnemyHitSound ();

			Instantiate (hitParticle, p_hitBy.transform.position, Quaternion.LookRotation(dir));
			Instantiate (hitTextPopUp, transform.position + new Vector3(0,1.5f,0), Quaternion.identity); //rotation is set in the PopUpText script because of lerp

			Vector3 ragdollDir = (transform.position - p_hitBy.transform.position).normalized;
			foreach (Rigidbody rb in rbs)
			{
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.velocity = (ragdollDir * 25);
			}
				
			gameObject.layer = 11; //puts enemy on the enemy ball layer temporarily

			if(animator == transform.GetChild(2).GetComponent<Animator>())
				pointsToAdd++;
			
			hit = true;
			StopCoroutine ("Move");
			StopCoroutine ("Hop");
			floor.tilesFilled[curColumn, curRow] = false;

			SetKinematic(false);
			animator.enabled = false;

			StopAllCoroutines();
			lifeEndTime = Time.time + 1f;
		}
	}

	IEnumerator Move() {
		canThrow = true;
		float timer = 0f;
		while(curRow <= floor.rows - 6) {
			float t_time = Time.time;

			yield return StartCoroutine ("Hop", new HopData (floor.tiles[curColumn, curRow].transform.position, Random.Range(1.5f, 1.7f)));
			//yield return StartCoroutine ("Hop", new HopData (floor.tiles[curColumn, curRow].transform.position, 1.78f));

			timer += Time.time - t_time;

			if(timer > jumpRate) {
				timer = 0f;
				floor.tilesFilled[curColumn, curRow] = false;

				List<int2> availableTiles = new List<int2>();
				if(curColumn > 0) {
					if(!floor.tilesFilled[curColumn-1, curRow])
						availableTiles.Add(new int2(curColumn-1, curRow));
					if(!floor.tilesFilled[curColumn-1, curRow+1] && (curRow != 2 || ( curColumn != 1 && curColumn != 2 ) ) )
						availableTiles.Add(new int2(curColumn-1, curRow+1));
				}
				if(curColumn < floor.columns - 1) {
					if(!floor.tilesFilled[curColumn+1, curRow])
						availableTiles.Add(new int2(curColumn+1, curRow));
					if(!floor.tilesFilled[curColumn+1, curRow+1] && (curRow != 2 || ( curColumn != 1 && curColumn != 2 ) ) )
						availableTiles.Add(new int2(curColumn+1, curRow+1));
				}
				if(!floor.tilesFilled[curColumn, curRow+1])
					availableTiles.Add(new int2(curColumn, curRow+1));

				if(availableTiles.Count > 0) {
					int2 nextTile = availableTiles[Random.Range(0, availableTiles.Count)];
					curRow = nextTile.y;
					curColumn = nextTile.x;
				}
				floor.tilesFilled[curColumn, curRow] = true;


//				float rand = Random.Range(0f, 100f);
//				if(rand < moveForwardChancePct) {
//					curRow++; 			// Forward
//				} else if(rand < moveForwardChancePct + ((100f - moveForwardChancePct)/2f)) {
//					if(curColumn > 0)
//						curColumn--; 	// Left
//				} else {
//					if(curColumn < floor.columns - 1)
//						curColumn++; 	// Right
//				}
			}
		}
		floor.tilesFilled[curColumn, curRow] = false;
//		print ("Ouch!");
		//PlayerManager.ReducePoints(1);

		aam.onCourtEnemies.Remove (this.gameObject);
		fade = true;
        onCourt = false;

		yield return StartCoroutine ("Hop", new HopData (floor.tiles [curColumn, curRow-1].transform.position, Random.Range (1.5f, 1.7f)));
		//StopAllCoroutines();
	}

	IEnumerator Hop(HopData data) {
		bool coroutineRunning = true;
		bool throwBall;
		bool turnTowardsPlayer;

		animator.transform.localPosition = Vector3.zero;
		animator.SetBool("Jump", true);
		if (ReadyToThrow ()) {
			animator.SetInteger ("RandomJump", -1);
			throwBall = true;
			turnTowardsPlayer = true;
		} else {
			animator.SetInteger ("RandomJump", 1);
			throwBall = false;
			turnTowardsPlayer = false;
		}
		
//		if(animator == transform.GetChild(2).GetComponent<Animator>())
//			animator.SetInteger("RandomJump", 1);
//		else
//			animator.SetInteger("RandomJump", Random.Range(1, 10));

		ClosestTile(transform.position).GetComponent<Animator>().SetTrigger("Bounce");
		Vector3 startPos = transform.position;
		if (!inTutorialMode) {
			startPos = transform.position;
		}
		else {
			startPos = new Vector3 (transform.position.x, 3f, transform.position.z);
			onCourt = true;
		}
		float timer = 0.0f;

		switch(animator.GetInteger("RandomJump")) {
			case -1:
				animator.Play (Animator.StringToHash ("jump " + -1), 0, 0f);
				break;
			case 1:
				animator.Play(Animator.StringToHash("jump " + 1), 0, 0f);
				break;
			case 2:
				animator.Play(Animator.StringToHash("jump " + 2), 0, 0f);
				break;
			case 3:
				animator.Play(Animator.StringToHash("jump " + 3), 0, 0f);
				break;
			case 4:
				animator.Play(Animator.StringToHash("jump " + 4), 0, 0f);
				break;
			case 5:
				animator.Play(Animator.StringToHash("jump " + 5), 0, 0f);
				break;
			case 6:
				animator.Play(Animator.StringToHash("jump " + 6), 0, 0f);
				break;
			case 7:
				animator.Play(Animator.StringToHash("jump " + 7), 0, 0f);
				break;
			case 8:
				animator.Play(Animator.StringToHash("jump " + 8), 0, 0f);
				break;
			case 9:
				animator.Play(Animator.StringToHash("jump " + 9), 0, 0f);
				break;
			default:
				print ("wtf");
				break;
		}

		float temp_hopHeight = hopHeight * Random.Range(0.8f, 1.2f);

		while (timer <= 1.0f) {
			if(timer > 0.01f)
				animator.Play(animator.GetCurrentAnimatorStateInfo(0).nameHash, 0, timer);

			float height = Mathf.Sin(Mathf.PI * timer) * temp_hopHeight;
			transform.position = Vector3.Lerp(startPos, data.dest, timer) + Vector3.up * height; 
			
			timer += Time.deltaTime / data.time;

			if (throwBall) 
			{
				throwTimer += Time.deltaTime;
				if (throwTimer >= .34f) 
				{
					if (activeFakeBall.activeSelf == false) 
					{
						activeFakeBall.SetActive (true);
						ChooseBallPowerUp ();
					}
				}

				if (throwTimer >= 0.87f) 
				{
					throwBall = false;
					Throw ();
					activeFakeBall.SetActive (false);
				}
			}

			if (turnTowardsPlayer) 
			{
				Vector3 currentPlayerPos = player.transform.position;
				dir = currentPlayerPos - transform.position;
				dir.y = 0;
				transform.rotation = Quaternion.LookRotation (dir.normalized * -1);
			}

			yield return null;
		}
		jumps++;
		throwTimer = 0f;

		coroutineRunning = false;
		if (inTutorialMode && !coroutineRunning) {
			yield return StartCoroutine ("Hop", tutorialHop);
		}
	}

	void Throw() {

		Transform activeFakeBallTransform = activeFakeBall.transform;

		GameObject ball = StaticPool.GetObj (ballPrefab);
		gameMan.activeBalls.Add (ball.GetComponent<EnemyBall>());
		if (gameMan.gamePhaseInt == 1)
			gameMan.warmUpBallsThrown += 1;

		ball.GetComponent<EnemyBall>().ChoosePowerUp(powerUpChoice);
		//SwitchFakeBallMaterial (ball.GetComponent<EnemyBall> ().powerUpInt);

		playerPos = throwDestination.position;
		float randomX = Random.Range (0.8f, 1.6f);

		if(curColumn <= 1)
			playerPos += new Vector3 (-1f * randomX, 0, 0);
		else
			playerPos += new Vector3 (1f * randomX, 0, 0);

		if(curRow != 2)
			timeToPlayer = 2 * Vector3.Distance (activeFakeBall.transform.position, playerPos) / 18f;
		else
			timeToPlayer = 2 * Vector3.Distance (activeFakeBall.transform.position, playerPos) / 12f;

		ball.GetComponent<EnemyBall> ().tutorialBall = false;
		ball.GetComponent<EnemyBall> ().Reset ();
		ball.GetComponent<EnemyBall> ().ChoosePowerUp (powerUpChoice);
		//ball.GetComponent<EnemyBall> ().SetColliderEnableTime( timeToPlayer * 1f / 4f );
		ball.transform.position = activeFakeBall.transform.position; //throwPoint.position;
		ball.transform.rotation = activeFakeBall.transform.rotation;

        if (ball.transform.childCount > 0)
            for (int i = 0; i < ball.transform.childCount; i++)
                Destroy(ball.transform.GetChild(i).gameObject);
        GameObject particle = activeFakeBall.transform.GetChild(0).gameObject;
        particle.transform.parent = ball.transform;
        particle.transform.localPosition = Vector3.zero;

        float hVel = Vector3.Distance (playerPos, throwPoint.position) / timeToPlayer;
		//float vVel = (4f + 0.5f * -Physics.gravity.y * Mathf.Pow (timeToPlayer, 2) - throwPoint.position.y) / timeToPlayer;
		float vVel = (0.5f * Physics.gravity.y * Mathf.Pow (timeToPlayer, 2) + throwPoint.position.y - playerPos.y) / -timeToPlayer;

		Vector3 ballDir = (playerPos - throwPoint.position).normalized;
		ballDir *= hVel;
		ballDir.y = vVel;// /1.5f;

		Rigidbody ballRB = ball.GetComponent<Rigidbody> ();
			
		ballRB.velocity = ballDir;
		ballRB.AddTorque (Random.insideUnitSphere * 100f);

		//print("Ball thrown by enemy");
		//print("PlayerPos: " + playerPos + ", Distance: " + Vector3.Distance(transform.position, playerPos));
		//print ("Time to player: " + timeToPlayer + ", Current Row: " + curRow + ", Current Column: " + curColumn + ", PlayerPos: " + playerPos);
	}

	Transform ClosestTile(Vector3 pos) {
		Transform closestTile = floor.tiles[0, 0].transform;
		for(int i = 0; i < floor.columns; i++) {
			for(int j = 0; j < floor.rows; j++) {
				if(Vector3.Distance(floor.tiles[i, j].transform.position, pos) < Vector3.Distance(closestTile.position, pos)) {
					closestTile = floor.tiles[i, j].transform;
				}
			}
		}
		return closestTile;
	}

	public void StartMove() {
		StartCoroutine( "Move" );

		if (aam == null) {
			aam = GameObject.Find ("GameManager").GetComponent<AimAssistManager> ();
		}
		aam.onCourtEnemies.Add (this.gameObject);
        onCourt = true;
	}

	public void GoToQueuePos( Waypoint wp ) {
		StartCoroutine( "MoveToPos", wp );
	}

	IEnumerator MoveToPos( Waypoint dest ) {
		animator.SetBool("Walk", true);
		animator.Play("Walk", 0, 0f);
		Vector3 startPos = transform.position;
		float moveTime = 0.5f;
		float timer = 0.0f;

		while( timer <= 1.0f ) {
			transform.position = Vector3.Lerp( startPos, dest.transform.position, timer );
			timer += Time.deltaTime / moveTime;
			yield return null;
		}

		dest.m_occupant = gameObject;
		dest.m_reserved = false;
		animator.SetBool("Walk", false);
		animator.CrossFade("Start", 0.2f, 0);
	}

	bool ReadyToThrow()
	{
		SwitchThrowInterval ();
		if (jumps > throwInterval && gameMan.timer > 0) 
		{
			jumps = 0;
			return true;
		} 
		else 
		{
			return false;
		}
	}

	void SwitchThrowInterval()
	{
		switch (gameMan.gamePhaseInt) 
		{
		case 1:
			throwInterval = 2;
			break;
		case 2:
			throwInterval = 2; //~4
			break;
		case 3:
			throwInterval = 3; //~6
			break;
		case 4:
			throwInterval = 3; //~6
			break;
		}
	}
		
	public void ChooseBallPowerUp()
	{
		//powerUpChoice = Random.Range(0, 99);
		string matName = "";

        if(gameMan.gamePhaseInt == 1)
        {
            if (inWarmUp || onCourt)
            {
                powerUpChoice = 99;
            }
        }
        else
        {
            powerUpChoice = Random.Range(0, 99);
        }

		if (powerUpChoice >= 26) 
		{
			matName = "Standard";
			SwitchFakeBallMaterial (matName);
		}

		//1st Power Up (Bounce Back)
		if(powerUpChoice <= 14)
		{
			matName = "BounceBack";
			SwitchFakeBallMaterial (matName);
		}

		//2nd Power Up (Heat Seeking / Auto Aim)
		if(powerUpChoice >= 15 && powerUpChoice <= 20)
		{
			matName = "HeatSeek";
			SwitchFakeBallMaterial (matName);
		}

		//3rd Power Up (Bomb)
		if (powerUpChoice >= 21 && powerUpChoice <= 25) 
		{
			matName = "Bomb";
			SwitchFakeBallMaterial (matName);
		}
	}

	void SwitchFakeBallMaterial(string matName)
	{
		foreach (Material mat in ebm.materialList) 
		{
			if(mat.name.Contains(matName))
			{
				activeFakeBall.GetComponent<Renderer> ().material = mat;
			}
		}

        foreach(GameObject particle in ebm.particlesList)
        {
            if (particle.name.Contains(matName))
            {
                if(activeFakeBall.transform.childCount > 0)
                {
                    for(int i = 0; i < activeFakeBall.transform.childCount; i++)
                    {
                        Destroy(activeFakeBall.transform.GetChild(i).gameObject);
                    }
                }

                GameObject newParticle = Instantiate(particle);
                newParticle.transform.parent = activeFakeBall.transform;
                newParticle.transform.localPosition = Vector3.zero;
            }
        }
	}

	void FadeOut(float timer)
	{
//		for (int i = 0; i < renderers.Length; i++) 
//		{
//			renderers[i].material.color = new Color(renderers[i].material.color.r, renderers[i].material.color.g, renderers[i].material.color.b, timer);
//		}
		for (int i = 0; i < allRenderers.Count; i++) 
		{
			allRenderers[i].material.color = new Color(allRenderers[i].material.color.r, allRenderers[i].material.color.g, allRenderers[i].material.color.b, timer);
		}
	}

	public void CallHit(GameObject ball)
	{
		Hit (ball);
	}

	public void CallFade()
	{
		fade = true;
	}
}
