using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spider : MonoBehaviour {

	[System.NonSerialized]
	public bool isActive = false;

	public float[] appearanceTimes;
	public float hangTime = 5;
	public Transform leftBoundary;
	public Transform rightBoundary;
	public Renderer spiderWeb;
	

	bool hit = false;
//	GameObject hitBy;
	Animator myAnimator;
	int numAppearances;
	int currentAppearance = 0;
	float hangTimer = 0f;
	float deathTimer = 1f;

	private void Start() {
		myAnimator = GetComponent<Animator>();
		myAnimator.SetBool( "Up", false );
		myAnimator.SetBool( "Drop", false );
	}

	public void Init( int _numAppearances, float appearInterval ) {
		numAppearances = _numAppearances;
		appearanceTimes = new float[numAppearances];

		for( int i = 0; i < _numAppearances; i++ ) {
			appearanceTimes[i] = Random.Range( 0, appearInterval ) + ( (float)i * appearInterval );
		}
	}

	void Update() {
		if( currentAppearance < numAppearances ) {
			if( !isActive ) {
				// If it's time to appear, change position, drop, and increase times that has appeared
				if( ( GameManager.instance.gameTimer - GameManager.instance.timer ) > appearanceTimes[currentAppearance] ) {
					GoToNewPos();

					myAnimator.SetBool( "Drop", true );
					myAnimator.SetBool( "Up", false );
					isActive = true;
					currentAppearance++;
				}
			} else {
				// If hasn't been hit, start hang timer
				if( !hit ) {
					// If spider hangs the ammount of time it should and hasn't been hit, repel back up
					if( hangTimer >= hangTime ) {
						myAnimator.SetBool( "Up", true );
						myAnimator.SetBool( "Drop", false );
						hangTimer = 0f;

						isActive = false;
						return;
					}

					hangTimer += Time.deltaTime;
				}
			}
		}
	}

	void GoToNewPos() {
		float translationScale = Random.Range( 0f, 1f );

		Vector3 diffVector = leftBoundary.position - rightBoundary.position;
		transform.position = rightBoundary.position + diffVector * translationScale;
	}

	void Hit(GameObject p_hitBy) {
		if(!hit) {
//			hitBy = p_hitBy;
			Ball hitBall = p_hitBy.GetComponent<Ball>();			
			int pointsToAdd = 3;
			spiderWeb.enabled = false;
			
			if( hitBall.color == PlayerColor.Red ) {
				PlayerManager.AddPoints(PlayerColor.Red, pointsToAdd);
			} 
//			else if( hitBall.color == PlayerColor.Yellow ) {
//				PlayerManager.AddPoints(PlayerColor.Yellow, pointsToAdd);
//			} 
			else if( hitBall.color == PlayerColor.Green ) {
				PlayerManager.AddPoints(PlayerColor.Green, pointsToAdd);
//			} else if( hitBall.color == PlayerColor.Blue ) {
//				PlayerManager.AddPoints(PlayerColor.Blue, pointsToAdd);
			} else {
				print ( "wtf" );
			}
			
			hit = true;
			myAnimator.SetBool( "Drop", false );

			// Ragdoll stuff
			myAnimator.enabled = false;

			Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
			foreach( Rigidbody rB in rigidbodies )
				rB.isKinematic = false;

			StartCoroutine( "Die", deathTimer );
		}
	}

	IEnumerator Die( float time ) {
		yield return new WaitForSeconds( time );
		Reset();
	}

	void Reset() {
		myAnimator.enabled = true;
		
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		foreach( Rigidbody rB in rigidbodies )
			rB.isKinematic = true;

		hit = false;
//		hitBy = null;
		isActive = false;
		hangTimer = 0f;
		spiderWeb.enabled = true;
		myAnimator.Play( "Start" );
		myAnimator.SetBool( "Up", false );
		myAnimator.SetBool( "Drop", false );
	}
}
