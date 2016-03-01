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

	public GameObject ballPrefab;
	public float throwInterval = 2f;
	public float timeToPlayer = 3f;
	Vector3 dir;
	Vector3 playerPos;
	bool canThrow = false;

	SpawnFloor floor;

	Animator animator;

	public List<Texture> maleTextures;
	public List<Texture> femaleTextures;

//	public float moveForwardChancePct = 50f;

	bool hit;

	float lifeEndTime;

	[System.NonSerialized]
	public GameObject hitBy;

	class HopData {
		public HopData(Vector3 p_dest, float p_time) {dest = p_dest; time = p_time;}
		public Vector3 dest;
		public float time;
	}

	struct int2 {
		public int2(int p_x, int p_y) {x = p_x; y = p_y;}
		public int x;
		public int y;
	}

	void OnEnable() {
//		moveForwardChancePct = Mathf.Clamp(moveForwardChancePct, 0f, 100f);

		gameMan = GameObject.Find ("GameManager");

		floor = GameObject.Find ("Floor").GetComponent<SpawnFloor> ();

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

		int randChar = Random.Range(0, 3);
		transform.GetChild(randChar).gameObject.SetActive(true);
		animator = transform.GetChild(randChar).GetComponent<Animator> ();
		if(randChar == 0) {
			int rand = Random.Range(0, maleTextures.Count - 1);
			animator.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = maleTextures[rand];
		} else if(randChar == 1) {
			int rand = Random.Range(0, femaleTextures.Count - 1);
			animator.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = femaleTextures[rand];
		} 

		SetKinematic(true);

		curRow = 0;

		hit = false;

		lifeEndTime = -1f;

		animator.enabled = true;
	}

	void Update() {
		if(lifeEndTime > 0f) {
			if(Time.time > lifeEndTime) {
				gameObject.SetActive(false);
			}
		}

		//Face the player
		playerPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
		dir = playerPos - transform.position;
		dir.y = 0;
		transform.rotation = Quaternion.LookRotation (dir.normalized * -1);

		//Throw
		if (canThrow) {
			//Throw ();
			StartCoroutine ("ThrowRoutine");
		}
	}

	void Hit(GameObject p_hitBy) {
		if(!hit) {
			hitBy = p_hitBy;

			int pointsToAdd = 1;
			gameMan.GetComponent<GameManager> ().AddScore (pointsToAdd);

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
		while(curRow <= floor.rows - 2) {
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
					if(!floor.tilesFilled[curColumn-1, curRow+1])
						availableTiles.Add(new int2(curColumn-1, curRow+1));
				}
				if(curColumn < floor.columns - 1) {
					if(!floor.tilesFilled[curColumn+1, curRow])
						availableTiles.Add(new int2(curColumn+1, curRow));
					if(!floor.tilesFilled[curColumn+1, curRow+1])
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

		gameObject.SetActive(false);
		StopAllCoroutines();
	}

	IEnumerator Hop(HopData data) {
		animator.transform.localPosition = Vector3.zero;
		animator.SetBool("Jump", true);
		if(animator == transform.GetChild(2).GetComponent<Animator>())
			animator.SetInteger("RandomJump", 1);
		else
			animator.SetInteger("RandomJump", Random.Range(1, 10));

		ClosestTile(transform.position).GetComponent<Animator>().SetTrigger("Bounce");
		Vector3 startPos = transform.position;
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
	}

	IEnumerator ThrowRoutine(){
		canThrow = false;

		yield return new WaitForSeconds (throwInterval);

		Throw ();

		canThrow = true;
	}

	void Throw() {

		GameObject ball = StaticPool.GetObj (ballPrefab);

		ball.GetComponent<EnemyBall> ().Reset ();
		ball.GetComponent<EnemyBall> ().SetColliderEnableTime( timeToPlayer / 4f );
		ball.transform.position = transform.localPosition + new Vector3(0,2.5f,0) - Vector3.forward;

		float hVel = Vector3.Distance (playerPos, transform.position) / timeToPlayer;
		float vVel = (playerPos.y + 0.5f * -Physics.gravity.y * Mathf.Pow (timeToPlayer, 2) - transform.position.y) / timeToPlayer;

		Vector3 ballDir = dir.normalized;
		ballDir *= hVel;
		ballDir.y = vVel/1.5f;

		Rigidbody ballRB = ball.GetComponent<Rigidbody> ();
			
		ballRB.velocity = ballDir;
		ballRB.AddTorque (Random.insideUnitSphere * 100f);

		//print("Ball thrown by enemy");
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

	void OnCollisionEnter(Collision coll) {
		if (coll.collider.tag == "Ball") {
			if (coll.collider.gameObject.GetComponent<EnemyBall> ().fromEnemy == false) {
				Hit (coll.collider.gameObject);
			}
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
}
