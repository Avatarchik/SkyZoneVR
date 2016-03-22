using UnityEngine;
using System.Collections;

public class EnemyBall : MonoBehaviour {

	public float destroyTime = 10f;
	float lifeTime = 0;
	float colliderTimer = 0.0f;

	public bool tutorialBall = false;
	public bool fromEnemy = true;
	bool streakChain = false;
	bool hitGround = false;

	Rigidbody rb;
	TrailRenderer trail;

	GameManager gm;
	AudioManager am;

	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		trail = GetComponent<TrailRenderer> ();
		trail.enabled = false;

		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
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
		//print ("Ball Reset");
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
			if (tutorialBall) {
				tutorialBall = false;
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
			//gm.AddToStreak ();
			streakChain = true;
		}

		if (gameObject.layer == 12 && coll.collider.gameObject.layer == 0 )//&& !streakChain) 
		{
			hitGround = true;
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
}
