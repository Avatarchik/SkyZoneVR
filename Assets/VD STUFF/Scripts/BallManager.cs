using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallManager : MonoBehaviour {
	
	public GameObject[] ballPrefabs;
	public float shootStrength = 1000f;
	public GameObject hitParticle;

	float redBallTimer, greenBallTimer, yellowBallTimer, blueBallTimer;
	bool redOnCd, greenOnCd, yellowOnCd, blueOnCd = false;
	float ballShootCooldown = 0.5f;

	Dictionary<string, GameObject> ballPrefabDict;

	public GameObject nearPlane;

	void Start () {
//		nearPlane = GameObject.Find("NearPlane");
		char[] delimChar = new char[1] {'_'};

		ballPrefabDict = new Dictionary<string, GameObject> ();
		for(int i = 0; i < ballPrefabs.Length; i++) {
			ballPrefabDict.Add(ballPrefabs[i].name.Split(delimChar)[0], ballPrefabs[i]);
			StaticPool.InitObj(ballPrefabs[i]);
		}
	}

	class ShootData {
		public ShootData(Vector2 p_start, Vector3 p_dest, PlayerColor p_color) {
			start = p_start; dest = p_dest; color = p_color;
		}
		public ShootData() { }
		public Vector2 start;
		public Vector3 dest;
		public PlayerColor color;
	}

	void Update () {
		if( redOnCd ) {
			if( redBallTimer > ballShootCooldown ) {
				redOnCd = false;
				redBallTimer = 0f;
			}
			redBallTimer += Time.deltaTime;
		}
		if( yellowOnCd ) {
			if( yellowBallTimer > ballShootCooldown ) {
				yellowOnCd = false;
				yellowBallTimer = 0f;
			}
			yellowBallTimer += Time.deltaTime;
		}
		if( greenOnCd ) {
			if( greenBallTimer > ballShootCooldown ) {
				greenOnCd = false;
				greenBallTimer = 0f;
			}
			greenBallTimer += Time.deltaTime;
		}
		if( blueOnCd ) {
			if( blueBallTimer > ballShootCooldown ) {
				blueOnCd = false;
				blueBallTimer = 0f;
			}
			blueBallTimer += Time.deltaTime;
		}
	}

	public void Shoot(Vector2 pos, PlayerColor color) {
		switch( color ) 
		{
		case PlayerColor.Red:
			if( redOnCd )
				return;
			else
				redOnCd = true;
			break;

//		case PlayerColor.Yellow:
//			if( yellowOnCd )
//				return;
//			else
//				yellowOnCd = true;
//			break;

		case PlayerColor.Green:
			if( greenOnCd ) 
				return;
			else
				greenOnCd = true;
			break;
//		case PlayerColor.Blue:
//			if( blueOnCd )
//				return;
//			else
//				blueOnCd = true;
//			break;
		}

//		if(!Camera.main)
//			return;

		RaycastHit hit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hit)) {
//			StartCoroutine("Shoot", new ShootData(pos, hit.point, color));
			Shoot(new ShootData(pos, hit.point, color));
		}
	}
	

	void Shoot(ShootData shootData) {
//		print (shootData.start);

		Vector3 startPos = shootData.start;
		if(DebugMode.CENTERSPAWN)
			startPos.z -= Camera.main.transform.position.z/4f;// - nearPlane.transform.position.z;

		GameObject ball = StaticPool.GetObj(ballPrefabDict[shootData.color.ToString()]);
		ball.GetComponent<Ball>().Reset();

		ball.transform.position = Camera.main.ScreenToWorldPoint(startPos + Camera.main.transform.forward * 4f);
		ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

		PlayerManager.IncreaseShots(shootData.color);

		Vector3 shootDir = shootData.dest - ball.transform.position;
		shootDir.Normalize();

		if(DebugMode.FORWARDMODE) {
			ball.GetComponent<Rigidbody>().AddForce(Vector3.forward * (shootStrength + DebugMode.FORCECHANGE));
		} else {
			ball.GetComponent<Rigidbody>().AddForce(shootDir * (shootStrength + DebugMode.FORCECHANGE));
		}
			
//		Debug.Break();

		ball.GetComponent<Rigidbody>().useGravity = DebugMode.GRAVITY;
//		Destroy (ball, 10f);
	}
}
