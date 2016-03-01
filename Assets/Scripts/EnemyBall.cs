using UnityEngine;
using System.Collections;

public class EnemyBall : MonoBehaviour {

	public float destroyTime = 10f;
	float lifeEndTime = 0;

	public bool fromEnemy = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > lifeEndTime) {
			gameObject.SetActive (false);
		}
	}

	public void Reset(){
		lifeEndTime = Time.time + destroyTime;
		fromEnemy = true;

		//print (Time.time + " , " + lifeEndTime);
	}

	public void PlayerHit() {
		fromEnemy = false;
	}

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Bat")
            fromEnemy = false;

        if (coll.collider.tag == "Ball" && coll.collider.gameObject.GetComponent<EnemyBall>().fromEnemy == false)
            fromEnemy = false;
    }
}
