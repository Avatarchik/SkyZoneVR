using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBall : MonoBehaviour 
{
	public float destroyTime = 10f;
	float lifeTime = 0;
	float colliderTimer = 0.0f;

	public bool tutorialBall = false;
	public bool fromEnemy = true;
	bool streakChain = false;
	public bool hitGround = false;
	bool addedToWarmUpBallsDone = false;

	GameObject aimAssistEnemy;
	//GameObject player;

	Rigidbody rb;
	TrailRenderer trail;
	Renderer renderer;

	GameManager gm;
	AudioManager am;
	AimAssistManager aam;
	EnemyBallManager ebm;

	public int powerUpInt;

    bool bounceBack = false;
	bool autoAim = false;
	bool bomb = false;

	int bounceBackVolleys = 0;

	bool shouldLerp;
	float lerpTimer = 0f;
	public float autoAimLerpTime = 0.3f;
	GameObject lerpEnemy;
	Vector3 lerpBallStart;

	public float bombExplosionRadius = 5f;
	public GameObject bombExplosionParticle;

	void Awake () 
	{
		rb = GetComponent<Rigidbody> ();
        renderer = GetComponent<Renderer>();
		trail = GetComponent<TrailRenderer> ();
		trail.enabled = false;

		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		aam = gm.GetComponent <AimAssistManager> ();
		ebm = gm.GetComponent<EnemyBallManager> ();

//		player = GameObject.Find ("Player");
	}

	void Update ()
	{
		if( colliderTimer > 0.0f )
		{
			colliderTimer -= Time.deltaTime;

			if( colliderTimer <= 0.0f )
				gameObject.GetComponent<SphereCollider>().enabled = true;
		}

		lifeTime += Time.deltaTime;
		if (lifeTime > destroyTime) 
		{
			SetInactive ();
		}

		if (trail.enabled && trail.time < 1) 
		{
			trail.time += Time.deltaTime;
			if (trail.time >= 1f)
				trail.time = 1;
		}

		if (tutorialBall) 
		{
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			lifeTime = 0;
		}
		else 
		{
			rb.useGravity = true;
			//GetComponent<Rigidbody> ().isKinematic = false;
		}

		if (autoAim && shouldLerp) 
		{
			if (aam.onCourtEnemies.Contains(lerpEnemy) == false) 
			{
				AutoAimPowerUp ();
				lerpTimer /= 2;
			}

			Vector3 lerpEnemyPos = lerpEnemy.transform.position + new Vector3(0, 1, 0);
			lerpTimer += Time.deltaTime;
			transform.position = Vector3.Lerp (lerpBallStart, lerpEnemyPos, lerpTimer / autoAimLerpTime);

			if (transform.position == lerpEnemyPos || lerpEnemy.activeSelf == false || lerpEnemy == null)
				shouldLerp = false;
		}

        if (hitGround && transform.GetChild(0).gameObject.activeSelf == true)
            transform.GetChild(0).gameObject.SetActive(false);
	}

	public void Reset()
	{
		lifeTime = 0;
		fromEnemy = true;
		if( trail == null )
			trail = GetComponent<TrailRenderer> ();
		trail.enabled = false;
        gameObject.layer = 11;
		streakChain = false;
		hitGround = false;
		addedToWarmUpBallsDone = false;
		aimAssistEnemy = null;
		ResetPowerUps ();

		//print ("Ball Reset");
	}

	void ResetPowerUps()
	{
		SetBallAndTrailMaterials("Standard");
		bounceBack = false;
		autoAim = false;
		bomb = false;

		bounceBackVolleys = 0;

		lerpEnemy = null;
		lerpBallStart = Vector3.zero;
		shouldLerp = false;
		lerpTimer = 0;
	}

	public void DebugHit() 
	{
		fromEnemy = false;
	}

    void OnCollisionEnter(Collision coll)
    {
//		if (hitGround)
//			return;
		
		if (coll.collider.tag == "Bat") 
		{
			//print (rb.velocity.magnitude);
			if(rb.velocity.magnitude > 5 && !autoAim)
				AimAssist ();

			if (tutorialBall)
				tutorialBall = false;

			if (autoAim) 
			{
				AutoAimPowerUp ();

				if (!shouldLerp)
					shouldLerp = true;
			}

			fromEnemy = false;
			trail.enabled = true;
			trail.time = 0;
			am.DodgeballHitSound ();
			gameObject.layer = 12;
		}

        if (coll.collider.tag == "Ball" && coll.collider.gameObject.GetComponent<EnemyBall>().fromEnemy == false)
            fromEnemy = false;

		if (gameObject.layer == 12 && coll.collider.gameObject.tag == "Enemy") 
		{
            if (coll.collider.gameObject.GetComponent<Enemy>() == null && !hitGround)
            {
                coll.collider.gameObject.SendMessageUpwards("Hit", this.gameObject, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                coll.collider.gameObject.GetComponent<Enemy>().CallHit(this.gameObject);
                print(this.gameObject + ": ball");
                print("Enemy: " + coll.collider.gameObject);
            }

			//gm.AddToStreak ();
			streakChain = true;

			if (bounceBack) {
				BounceBackPowerUp (coll.collider.gameObject.GetComponent<Enemy>());
				bounceBackVolleys += 1;
			}

			if (bomb)
				BombPowerUp ();

			shouldLerp = false;

			if (hitGround)
				SetInactive ();

			//Warm Up
			if (gm.gamePhaseInt == 1 && !hitGround && !addedToWarmUpBallsDone) 
			{
				gm.warmUpBallsDone += 1;
				addedToWarmUpBallsDone = true;
			}
		}

		if (gameObject.layer == 12 && coll.collider.gameObject.layer == 0 )//&& !streakChain) 
		{
			//Warm Up
			if (gm.gamePhaseInt == 1 && !hitGround && !addedToWarmUpBallsDone) 
			{
				gm.warmUpBallsDone += 1;
				addedToWarmUpBallsDone = true;
			}

			hitGround = true;

			if (bomb)
				BombPowerUp ();

			if( !streakChain )
				gm.ResetStreak ();
		}

		if (coll.collider.gameObject.layer == 0 )
		{
			//Warm Up
			if (gm.gamePhaseInt == 1 && !hitGround && !addedToWarmUpBallsDone) 
			{
				gm.warmUpBallsDone += 1;
				addedToWarmUpBallsDone = true;
			}

			hitGround = true;
		}
    }

	public void SetColliderEnableTime( float time )
	{
		gameObject.GetComponent<SphereCollider>().enabled = false;
		colliderTimer = time;
		trail.enabled = false;
	}

	void AimAssist()
	{
		aimAssistEnemy = aam.ClosestEnemyToBallDirection (this.gameObject);

		if (aimAssistEnemy != null) 
		{
			float timeToEnemy;

			if(rb.velocity.magnitude > 15)
				timeToEnemy = Vector3.Distance(aimAssistEnemy.transform.position, transform.position) / 4f;
			else
				timeToEnemy = Vector3.Distance(aimAssistEnemy.transform.position, transform.position) / 3f;

			float hVel = Vector3.Distance (aimAssistEnemy.transform.position, transform.position) / timeToEnemy;
			float vVel = (4f + 0.5f * -Physics.gravity.y * Mathf.Pow (timeToEnemy, 2) - transform.position.y) / timeToEnemy;

			Vector3 ballDir = aimAssistEnemy.transform.position - transform.position;
			ballDir *= hVel;
			ballDir.y = vVel/1.5f;

			rb.velocity = ballDir / 2;
			rb.AddTorque (Random.insideUnitSphere * 100f);
		}
	}

	void SetBallAndTrailMaterials(string matName)
	{
		if (matName != null) 
		{
			foreach (Material mat in ebm.materialList) 
			{
				if (mat.name.Contains (matName))
					renderer.material = mat;
			}

			foreach (Material mat in ebm.trailMaterialList) 
			{
				if (mat.name.Contains (matName))
					trail.material = mat;
			}
        } 
		else 
		{
			renderer.material = ebm.materialList [0];
			trail.material = ebm.trailMaterialList [0];
		}
	}

//    public void ChoosePowerUp()
//    {
//        int powerUpChoice = Random.Range(0, 99);
//		powerUpInt = powerUpChoice;
//
//		if (tutorialBall || gm.gamePhaseInt == 1)
//			powerUpChoice = 99;
//
//		//1st Power Up (Bounce Back)
//        if(powerUpChoice <= 14)
//        {
//			SetBallAndTrailMaterials ("BounceBack");
//            bounceBack = true;
//        }
//
//		//2nd Power Up (Heat Seeking / Auto Aim)
//        if(powerUpChoice >= 15 && powerUpChoice <= 20)
//        {
//			SetBallAndTrailMaterials ("HeatSeek");
//			autoAim = true;
//        }
//
//		//3rd Power Up (Bomb)
//		if (powerUpChoice >= 21 && powerUpChoice <= 25) 
//		{
//			SetBallAndTrailMaterials ("Bomb");
//			bomb = true;
//		}
//    }

	public void ChoosePowerUp(int powerUpChoice)
	{
		if (powerUpChoice >= 26)
			SetBallAndTrailMaterials ("Standard");

		//1st Power Up (Bounce Back)
		if(powerUpChoice <= 14)
		{
			SetBallAndTrailMaterials ("BounceBack");
			bounceBack = true;
		}

		//2nd Power Up (Heat Seeking / Auto Aim)
		if(powerUpChoice >= 15 && powerUpChoice <= 20)
		{
			SetBallAndTrailMaterials ("HeatSeek");
			autoAim = true;
		}

		//3rd Power Up (Bomb)
		if (powerUpChoice >= 21 && powerUpChoice <= 25) 
		{
			SetBallAndTrailMaterials ("Bomb");
			bomb = true;
		}
	}

    void BounceBackPowerUp(Enemy lastHitEnemy)
    {
        bounceBack = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.layer = 11;
        //fromEnemy = true;
        lifeTime -= 5f;

		if (bounceBackVolleys > 0)
			streakChain = true;

        float timeToPlayer = 0f;
		Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

		if (lastHitEnemy != null) 
		{
			if (lastHitEnemy.curRow != 2)
				timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 18f;
			else
				timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 12f;

			if (lastHitEnemy.curColumn <= 1)
				playerPos += new Vector3 (-1f, -0.25f, 1.25f);
			else
				playerPos += new Vector3 (1f, -0.25f, 1.25f);

		} 
		else 
		{
			timeToPlayer = 2 * Vector3.Distance (transform.position, playerPos) / 18f;
			playerPos += new Vector3 (1f, -0.25f, 1.25f);
		}

		if (bounceBackVolleys > 1)
			timeToPlayer /= (bounceBackVolleys / 2f);

        //SetColliderEnableTime( timeToPlayer / 4f );
        //ball.transform.position = transform.localPosition + new Vector3(0, 2.5f, 0) - Vector3.forward;

		float hVel = Vector3.Distance (playerPos, transform.position) / timeToPlayer;
		float vVel = (0.5f * Physics.gravity.y * Mathf.Pow (timeToPlayer, 2) + transform.position.y - playerPos.y) / -timeToPlayer;

		Vector3 ballDir = (playerPos - transform.position).normalized;
        ballDir *= hVel;
		ballDir.y = vVel; // 1.5f;

        rb.velocity = ballDir;
        rb.AddTorque(Random.insideUnitSphere * 100f);
    }

	void AutoAimPowerUp()
	{
		streakChain = true;

		lerpEnemy = ClosestEnemy();
		lerpBallStart = transform.position;

		shouldLerp = true;
	}

	GameObject ClosestEnemy()
	{
		GameObject closestEnemy = null;
		float closestDistance = 30;

		foreach (GameObject enemy in aam.onCourtEnemies) 
		{
			float distance = Vector3.Distance (transform.position, enemy.transform.position);

			if (distance < closestDistance) 
			{
				closestDistance = distance;
				closestEnemy = enemy;
			}
		}

		if (closestEnemy == null) 
		{
			closestEnemy = aam.onCourtEnemies[0];
		}
			
		return closestEnemy;
	}

	void BombPowerUp()
	{
		streakChain = true;

		Collider[] hitColliders = Physics.OverlapSphere (transform.position, bombExplosionRadius);

		int i = 0;
		while (i < hitColliders.Length) 
		{
			if (hitColliders [i].gameObject.tag == "Enemy") 
			{
				if (hitColliders [i].gameObject.GetComponent<Enemy> () == null)
					hitColliders[i].gameObject.GetComponentInParent<Enemy>().CallHit(this.gameObject);
				else
					hitColliders[i].gameObject.GetComponent<Enemy>().CallHit(this.gameObject);
			}

			i++;
		}

		am.BombExplosionSound ();
		Instantiate (bombExplosionParticle, transform.position, Quaternion.Euler(Vector3.right * 90));
		SetInactive ();
	}

	void SetInactive()
	{
		gameObject.SetActive (false);
		gm.activeBalls.Remove (this);
	}
}
