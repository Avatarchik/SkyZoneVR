using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public bool debugMode;
	public Camera debugCamera;

	void Start () 
	{
		if (debugMode) 
			GameObject.Find ("Bat Holder").SetActive (false);
	}

	void Update () 
	{
		//Re-Center HMD
		if(Input.GetKeyDown(KeyCode.C))
			OVRManager.display.RecenterPose();

		//Keyboard Camera Movement
		if (Input.GetKey (KeyCode.A))
			transform.Rotate (Vector3.up, -1F, Space.World);

		if (Input.GetKey (KeyCode.D))
			transform.Rotate (Vector3.up, 1F, Space.World);

		if (Input.GetKey (KeyCode.W))
			transform.Rotate (Vector3.right, 1F, Space.Self);

		if (Input.GetKey (KeyCode.S))
			transform.Rotate (Vector3.right, -1F, Space.Self);

		//Debug Mode (Click screen for bat hitting)
		if (debugMode) 
		{
			Ray ray = debugCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();

			if (Input.GetButtonDown ("Fire1")) 
			{	
				if (Physics.Raycast (ray, out hit, 100)) 
				{
					if (hit.rigidbody != null) 
					{
						//hit.rigidbody.AddRelativeForce (0, 0, hit.distance * 100);
						hit.rigidbody.AddForce (transform.forward * hit.distance * 10000);

						if (hit.rigidbody.gameObject.tag == "Ball") 
							hit.rigidbody.GetComponent<EnemyBall> ().PlayerHit ();
					}
				}
				//print ("You clicked this: " + hit.rigidbody);
			}
		}
	}
}

