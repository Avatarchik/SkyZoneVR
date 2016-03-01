using UnityEngine;
using System.Collections;

public class EnemyBall : MonoBehaviour {

	public float destroyTime = 10f;
	float lifeEndTime = 0;
	float colliderTimer = 0.0f;

	public bool fromEnemy = true;

	// Use this for initialization
	void Start () 
	{
		
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
			
	}

	public void Reset()
	{
		lifeEndTime = Time.time + destroyTime;
		fromEnemy = true;

		//print (Time.time + " , " + lifeEndTime);
	}

	public void PlayerHit() 
	{
		fromEnemy = false;
	}

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Bat")
            fromEnemy = false;

        if (coll.collider.tag == "Ball" && coll.collider.gameObject.GetComponent<EnemyBall>().fromEnemy == false)
            fromEnemy = false;
    }

	public void SetColliderEnableTime( float time )
	{
		gameObject.GetComponent<SphereCollider>().enabled = false;
		colliderTimer = time;
	}
}
