using UnityEngine;
using System.Collections;

public class EnemyBall : MonoBehaviour {

	public float destroyTime = 10f;
	float lifeEndTime = 0;
	float colliderTimer = 0.0f;

	public bool fromEnemy = true;

	TrailRenderer trail;

	AudioManager am;

	// Use this for initialization
	void Start () 
	{
		trail = GetComponent<TrailRenderer> ();
		trail.enabled = false;

		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( colliderTimer > 0.0f )
		{
			colliderTimer -= Time.deltaTime;

			if( colliderTimer <= 0.0f )
				gameObject.GetComponent<SphereCollider>().enabled = true;
		}

		if (Time.time > lifeEndTime) 
		{
			gameObject.SetActive (false);
		}

		if (!fromEnemy) {
			trail.enabled = true;
			trail.time = 1;
		} 
		else 
		{
			trail.enabled = false;
			trail.time = 0;		}
	}

	public void Reset()
	{
		lifeEndTime = Time.time + destroyTime;
		fromEnemy = true;

		//print ("Ball Reset");
	}

	public void PlayerHit() 
	{
		fromEnemy = false;
	}

    void OnCollisionEnter(Collision coll)
    {
		if (coll.collider.tag == "Bat") 
		{
			fromEnemy = false;
			am.DodgeballHitSound ();
		}

        if (coll.collider.tag == "Ball" && coll.collider.gameObject.GetComponent<EnemyBall>().fromEnemy == false)
            fromEnemy = false;
    }

	public void SetColliderEnableTime( float time )
	{
		gameObject.GetComponent<SphereCollider>().enabled = false;
		colliderTimer = time;
	}
}
