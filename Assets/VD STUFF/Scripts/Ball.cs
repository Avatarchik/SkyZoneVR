using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	public bool hasCollided = false;
	public GameObject hitParticle;

	float impactEndTime = 0;
	Rigidbody impactTarget = null;
	Vector3 impact;

	AudioSource audioSource;
	
	float lifeEndTime = 0;

	public PlayerColor color;

	void Start() {
		audioSource = GetComponent<AudioSource>();
		GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * 10);
		Reset();
	}

	void Update() {
		if (Time.time < impactEndTime) {
//			if(impactTarget != null)
				if(impactTarget.GetComponent<Rigidbody>() != null)
					impactTarget.AddForce(impact, ForceMode.VelocityChange);
		}
		if(Time.time > lifeEndTime) {
			transform.position = Vector3.one * 1000f;
			gameObject.SetActive(false);
		}
	}

	public void Reset() {
		lifeEndTime = Time.time + 10f;
		hasCollided = false;
		transform.rotation = Random.rotation;
	}

	void OnCollisionEnter(Collision col) {
		if(!hasCollided) {
			hasCollided = true;

			if(col.transform.tag == "Enemy") {
				PlayerManager.IncreaseHits(color);

				col.transform.SendMessageUpwards("Hit", gameObject, SendMessageOptions.DontRequireReceiver);

				//set the impact target to whatever the ray hit
				impactTarget = col.rigidbody;
				
				//impact direction also according to the ray
				impact = GetComponent<Rigidbody>().velocity * 0.5f;
				
				//the impact will be reapplied for the next 100ms
				//to make the connected objects follow even though the simulated body joints
				//might stretch
				impactEndTime = Time.time+0.10f;

				audioSource.Play();
				Destroy(Instantiate(hitParticle, col.contacts[0].point, Quaternion.identity), 4f); //TODO Eric: change to a better method

				Transform parent = col.transform;
				while(parent.name != "Enemy(Clone)" && parent.name != "PF_spiderFriend")
					parent = parent.parent;

				int pointsToAdd = 1;
				if( parent.name == "PF_spiderFriend" ) {
					pointsToAdd += 2;
				} else if (parent.GetChild(2).gameObject.activeInHierarchy) {
					pointsToAdd++;
				}

				if(color == PlayerColor.Red ) {
					FloatingTextManager.instance.CreateFloatingText( col.transform.position, pointsToAdd, Color.red );
				}
				else if(color == PlayerColor.Green) {
					FloatingTextManager.instance.CreateFloatingText( col.transform.position, pointsToAdd, Color.green );
				}
//				else if(color == PlayerColor.Yellow) {
//					FloatingTextManager.instance.CreateFloatingText( col.transform.position, pointsToAdd, Color.yellow );
//				}
//				else if(color == PlayerColor.Blue) {
//					FloatingTextManager.instance.CreateFloatingText( col.transform.position, pointsToAdd, Color.blue );
//				}
			}

			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().AddForce((transform.position - col.contacts[0].point).normalized * 200f);
			GetComponent<Rigidbody>().useGravity = true;
		}
	}

	public void PlayAudio() {
		audioSource.Play();
	}
}
