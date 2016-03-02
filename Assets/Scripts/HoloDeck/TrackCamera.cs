using UnityEngine;
using System.Collections;

public class TrackCamera : MonoBehaviour 
{
	public GameObject target;
	float dist = 0.2f;
	// Use this for initialization

	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 dir = Camera.main.transform.position - target.transform.position;
		dir.Normalize ();
		transform.position = target.transform.position + (dir * dist);
		transform.LookAt (Camera.main.transform.position);
		transform.Rotate (transform.up, 180f);
	}
}