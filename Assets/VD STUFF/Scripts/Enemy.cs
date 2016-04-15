using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	[System.NonSerialized]
	public int curColumn;
	[System.NonSerialized]
	public int omittedColumns = 1;

	int curRow;

	public float jumpRate = 2f;
	public float hopHeight = 2.25f;

	private GameObject gameMan;
	private GameObject audioMan;
	private TutorialManager tutMan;

	public GameObject ballPrefab;
	public float throwInterval = 2f;
	public float timeToPlayer = 3f;
	Vector3 dir;
	Vector3 playerPos;
	bool canThrow = false;
	GameObject player;
	public bool onCourt;

	public Transform throwPoint; 

	AimAssistManager aam;

	public GameObject hitParticle;

	SpawnFloor floor;

	Animator animator;

	public List<Texture> maleTextures;
	public List<Texture> femaleTextures;

	Rigidbody[] rbs;

//	public float moveForwardChancePct = 50f;

	bool hit;

	float lifeEndTime;

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

    void Start()
    {
        rbs = gameObject.GetComponentsInChildren<Rigidbody>();

//        foreach (Rigidbody rb in rbs)
//        {
//            //rb.mass *= 8;
//			rb.useGravity = false;
//			rb.isKinematic = true;
//        }

		//tutorialHop = new HopData(transform.position, 1.7f);

		gameMan = GameObject.Find ("GameManager");
		audioMan = GameObject.Find ("AudioManager");
		tutMan = gameMan.GetComponent<TutorialManager> ();
		aam = GameObject.Find("GameManager").GetComponent<AimAssistManager> ();

		floor = GameObject.Find ("Floor").GetComponent<SpawnFloor> ();
		player = GameObject.Find("Player");
		playerPos = player.transform.position;

//		tutorialHop.dest = transform.position + new Vector3(0, hopHeight, 0);
//		tutorialHop.time = 2f;
    }

	void OnEnable() {
//		moveForwardChancePct = Mathf.Clamp(moveForwardChancePct, 0f, 100f);

//		gameMan = GameObject.Find ("GameManager");
//		audioMan = GameObject.Find ("AudioManager");
//		floor = GameObject.Find ("Floor").GetComponent<SpawnFloor> ();

		tutorialHop = new HopData(new Vector3(transform.position.x, 3f, transform.position.z), 1.7f);

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
		if(randChar == 0) {
			int rand = Random.Range(0, maleTextures.Count - 1);
			animator.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = maleTextures[rand];
		} else if(randChar == 1) {
			int rand = Random.Range(0, femaleTextures.Count - 1);
			animator.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = femaleTextures[rand];
		} 

		gameObject.layer = 10; //reset layer to enemy layer

		SetKinematic(true);

		curRow = 0;

		hit = false;

		lifeEndTime = -1f;

		animator.enabled = true;

		onCourt = false;

//		foreach (Rigidbody rb in rbs)
//		{
//			rb.useGravity = false;
//			rb.isKinematic = true;
//		}
	}

	void Update() {
		if(lifeEndTime > 0f) {
			if(Time.time > lifeEndTime) {
				gameObject.SetActive(false);
			}
		}

//		//Face the player
//		playerPos = player.transform.position;
//		dir = playerPos - transform.position;
//		dir.y = 0;
//		transform.rotation = Quaternion.LookRotation (dir.normalized * -1);

		//Throw
		if (canThrow) {
			//Throw ();
			StartCoroutine ("ThrowRoutine");
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

			int pointsToAdd = 1;
			gameMan.GetComponent<GameManager> ().AddScore (pointsToAdd);
			audioMan.GetComponent<AudioManager> ().EnemyHitSound ();

			Instantiate (hitParticle, p_hitBy.transform.position, Quaternion.LookRotation(dir));

			Vector3 ragdollDir = (transform.position - p_hitBy.transform.position).normalized;
			foreach (Rigidbody rb in rbs)
			{
//				rb.useGravity = true;
//				rb.isKinematic = false;
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
		PlayerManager.ReducePoints(1);

		aam.onCourtEnemies.Remove (this.gameObject);
		gameObject.SetActive(false);
		StopAllCoroutines();
	}

	IEnumerator Hop(HopData data) {
		bool coroutineRunning = true;
		animator.transform.localPosition = Vector3.zero;
		animator.SetBool("Jump", true);
		if(animator == transform.GetChild(2).GetComponent<Animator>())
			animator.SetInteger("RandomJump", 1);
		else
			animator.SetInteger("RandomJump", Random.Range(1, 10));

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
			yield return null;
		}
		coroutineRunning = false;

		if (inTutorialMode && !coroutineRunning) {
			yield return StartCoroutine ("Hop", tutorialHop);
		}
	}

	IEnumerator ThrowRoutine(){

		SwitchThrowInterval ();

		if (inTutorialMode && waitToThrow) {
			//throwWaitTime = Random.Range (0, 4);

			waitToThrow = false;

		} 
		else if (!waitToThrow) 
		{
			throwWaitTime = 0;
		}

		canThrow = false;

		yield return new WaitForSeconds (throwInterval);

		Vector3 currentPlayerPos = player.transform.position;
		dir = currentPlayerPos - transform.position;
		dir.y = 0;
		transform.rotation = Quaternion.LookRotation (dir.normalized * -1);

		yield return new WaitForSeconds (throwWaitTime); //this is for tutorial mode

		Throw ();

		canThrow = true;
	}

	void Throw() {

		GameObject ball = StaticPool.GetObj (ballPrefab);
		//playerPos = player.transform.FindChild("Sphere").transform.position;//shpere

		playerPos = player.transform.position;

		if(curColumn <= 1)
			playerPos += new Vector3 (-1f, -0.25f, 1.25f);
		else
			playerPos += new Vector3 (1f, -0.25f, 1.25f);
		
		//timeToPlayer = 2 * Vector3.Distance (throwPoint.position, playerPos) / 18f;

		if(curRow != 2)
			timeToPlayer = 2 * Vector3.Distance (throwPoint.position, playerPos) / 18f;
		else
			timeToPlayer = 2 * Vector3.Distance (throwPoint.position, playerPos) / 12f;

//		switch (curRow) {
//		case 0:
//			playerPos += new Vector3 (0, 1.25f, 0);
//			timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 18f;
//			break;
//
//		case 1:
//			playerPos += new Vector3 (0, 1.25f, 0);
//			timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 16f;
//			break;
//
//		case 2:
//			if (curColumn == 1 || curColumn == 2) 
//			{
//				playerPos += new Vector3 (0, -2f, 1f);
//				timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 8f;
//			} 
//			else 
//			{
//				playerPos += new Vector3 (0, -2f, 3f);
//				timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 10f;
//			}
//			break;
//		}
		print ("Time to player: " + timeToPlayer + ", Current Row: " + curRow + ", Current Column: " + curColumn + ", PlayerPos: " + playerPos);

//		if (Vector3.Distance (transform.position, playerPos) < 8f) 
//		{
//			playerPos -= new Vector3 (0, 2f, -0.5f);
//			timeToPlayer = 4 * Vector3.Distance (transform.position, playerPos) / 14f;
//		} 
//		else 
//		{
//			timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 16f;
//		}
//
//		if (Vector3.Distance (transform.position, playerPos) > 14f) 
//		{
//			timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 18f;
//		}

		//print(Vector3.Distance (transform.position, playerPos));

		ball.GetComponent<EnemyBall> ().tutorialBall = false;
		ball.GetComponent<EnemyBall> ().Reset ();
		//ball.GetComponent<EnemyBall> ().SetColliderEnableTime( timeToPlayer * 1f / 4f );
		ball.transform.position = throwPoint.position;

		float hVel = Vector3.Distance (playerPos, throwPoint.position) / timeToPlayer;
		//float vVel = (4f + 0.5f * -Physics.gravity.y * Mathf.Pow (timeToPlayer, 2) - throwPoint.position.y) / timeToPlayer;
		float vVel = (0.5f * Physics.gravity.y * Mathf.Pow (timeToPlayer, 2) + throwPoint.position.y - playerPos.y) / -timeToPlayer;

		Vector3 ballDir = (playerPos - throwPoint.position).normalized;
		ballDir *= hVel;
		ballDir.y = vVel;///1.5f;

		Rigidbody ballRB = ball.GetComponent<Rigidbody> ();
			
		ballRB.velocity = ballDir;
		ballRB.AddTorque (Random.insideUnitSphere * 100f);

        ball.GetComponent<EnemyBall>().ChoosePowerUp();

		//print("Ball thrown by enemy");
		//print("PlayerPos: " + playerPos + ", Distance: " + Vector3.Distance(transform.position, playerPos));
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

//	void OnCollisionEnter(Collision coll) {
//		if (coll.collider.tag == "Ball") {
//			if (coll.collider.gameObject.GetComponent<EnemyBall> ().fromEnemy == false) {
//				Hit (coll.collider.gameObject);
//
//
//			}
//		}
//	}

	void SwitchThrowInterval()
	{
		switch (gameMan.GetComponent<GameManager> ().gamePhaseInt) 
		{
		case 1:
			throwInterval = Random.Range(3, 6); //~4
			break;
		case 2:
			throwInterval = Random.Range (5, 10); //~6
			break;
		case 3:
			throwInterval = Random.Range(5, 10); //~6
			break;
		}
	}

	//	void OnCollisionEnter(Collision col) {
	//		if(GameManager.instance.enemiesKnockback) {
	//			if(col.gameObject.tag == "Enemy") {
	//				hitBy = col.transform.GetComponentInParent<Enemy>().hitBy;
	//				Hit (hitBy);
	//			//	col.gameObject.SendMessageUpwards("Hit", hitBy, SendMessageOptions.DontRequireReceiver);
	//			}
	//		}
	//	}

	public void CallHit(GameObject ball)
	{
		Hit (ball);
	}
}
