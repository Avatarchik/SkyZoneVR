using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBall : MonoBehaviour {

	public float destroyTime = 10f;
	float lifeTime = 0;
	float colliderTimer = 0.0f;

	public bool tutorialBall = false;
	public bool fromEnemy = true;
	bool streakChain = false;
	bool hitGround = false;

	GameObject autoAimEnemy;
	//GameObject player;

	Rigidbody rb;
	TrailRenderer trail;

	GameManager gm;
	AudioManager am;
	AimAssistManager aam;

    Renderer renderer;
    public Material defaultMat;
    public Material bounceBackMat;
	public Material autoAimMat;
	public Material bombMat;

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
			gameObject.SetActive (false);
		}

		if (trail.enabled && trail.time < 1) {
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
			Vector3 lerpEnemyPos = lerpEnemy.transform.position;
			lerpTimer += Time.deltaTime;
			transform.position = Vector3.Lerp (lerpBallStart, lerpEnemyPos, lerpTimer / autoAimLerpTime);

			if (transform.position == lerpEnemyPos || lerpEnemy.activeSelf == false || lerpEnemy == null)
				shouldLerp = false;
		}
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
		autoAimEnemy = null;
		ResetPowerUps ();

		//print ("Ball Reset");
	}

	void ResetPowerUps()
	{
		GetComponent<Renderer>().material = defaultMat;
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
		if (hitGround)
			return;
		
		if (coll.collider.tag == "Bat") 
		{
			//print (rb.velocity.magnitude);

			if(rb.velocity.magnitude > 5 && !autoAim)
				AimAssist ();

			if (tutorialBall) {
				tutorialBall = false;
			}

			if (autoAim) 
			{
				//AutoAimPowerUp ();
				lerpEnemy = ClosestEnemy();
				lerpBallStart = transform.position;
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
			if (coll.collider.gameObject.GetComponent<Enemy> () == null)
				coll.collider.gameObject.SendMessageUpwards ("Hit", this.gameObject, SendMessageOptions.DontRequireReceiver);
			else
				coll.collider.gameObject.GetComponent<Enemy> ().CallHit (this.gameObject);

			//gm.AddToStreak ();
			streakChain = true;

			if (bounceBack) {
				BounceBackPowerUp ();
				bounceBackVolleys += 1;
			}

			if (bomb)
				BombPowerUp ();

			shouldLerp = false;
		}

		if (gameObject.layer == 12 && coll.collider.gameObject.layer == 0 )//&& !streakChain) 
		{
			hitGround = true;

			if (bomb)
				BombPowerUp ();

			if( !streakChain )
				gm.ResetStreak ();
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
		autoAimEnemy = aam.ClosestEnemyToBallDirection (this.gameObject);

		if (autoAimEnemy != null) 
		{
			float timeToEnemy;

			if(rb.velocity.magnitude > 15)
				timeToEnemy = Vector3.Distance(autoAimEnemy.transform.position, transform.position) / 4f;
			else
				timeToEnemy = Vector3.Distance(autoAimEnemy.transform.position, transform.position) / 3f;

			float hVel = Vector3.Distance (autoAimEnemy.transform.position, transform.position) / timeToEnemy;
			float vVel = (4f + 0.5f * -Physics.gravity.y * Mathf.Pow (timeToEnemy, 2) - transform.position.y) / timeToEnemy;

			Vector3 ballDir = autoAimEnemy.transform.position - transform.position;
			ballDir *= hVel;
			ballDir.y = vVel/1.5f;

			rb.velocity = ballDir / 2;
			rb.AddTorque (Random.insideUnitSphere * 100f);
		}
	}

    public void ChoosePowerUp()
    {
        int powerUpChoice = Random.Range(0, 99);

		if (tutorialBall)
			powerUpChoice = 99;

		//1st Power Up (Bounce Back)
        if(powerUpChoice <= 14)
        {
            renderer.material = bounceBackMat;
            bounceBack = true;
        }

		//2nd Power Up (Heat Seeking / Auto Aim)
        if(powerUpChoice >= 15 && powerUpChoice <= 20)
        {
			renderer.material = autoAimMat;
			autoAim = true;
        }

		//3rd Power Up (Bomb)
		if (powerUpChoice >= 21 && powerUpChoice <= 25) 
		{
			renderer.material = bombMat;
			bomb = true;
		}
    }

    void BounceBackPowerUp()
    {
        bounceBack = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.layer = 11;
        //fromEnemy = true;
        lifeTime -= 5f;

		if (bounceBackVolleys > 0)
			streakChain = true;

        float timeToPlayer;
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 4f, -4f);

        if (Vector3.Distance(transform.position, playerPos) < 8f)
        {
			playerPos = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 1f, 0.5f);
            timeToPlayer = 4 * Vector3.Distance(transform.position, playerPos) / 14f;
        }
        else
        {
            timeToPlayer = 2 * Vector3.Distance(transform.position, playerPos) / 14f;
        }
        //print(Vector3.Distance (transform.position, playerPos));

        //ball.GetComponent<EnemyBall> ().SetColliderEnableTime( timeToPlayer * 1f / 4f );
        //ball.transform.position = transform.localPosition + new Vector3(0, 2.5f, 0) - Vector3.forward;

        float hVel = Vector3.Distance(playerPos + new Vector3(0, -0.25f, 0.25f), transform.position) / timeToPlayer;
        float vVel = (4f + 0.5f * -Physics.gravity.y * Mathf.Pow(timeToPlayer, 2) - transform.position.y) / timeToPlayer;

        Vector3 ballDir = playerPos - transform.position;
        ballDir.Normalize();
        ballDir *= hVel;
        ballDir.y = vVel / 1.5f;

        rb.velocity = ballDir;
        rb.AddTorque(Random.insideUnitSphere * 100f);
    }

	void AutoAimPowerUp()
	{
		streakChain = true;

		lerpEnemy = ClosestEnemy();
		lerpBallStart = transform.position;
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
		this.gameObject.SetActive (false);
	}
}
