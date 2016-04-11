using UnityEngine;
using System.Collections;
using System;

public class AutoCenter : MonoBehaviour {

	public Transform oculusCamera;
	OptitrackRigidBody currBody; 

	public Vector3 currentOffset;
	// Use this for initialization
	void Start () {
		currBody = GetComponent<OptitrackRigidBody>();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 targetOffset = new Vector3(0,((-1*oculusCamera.localEulerAngles.y%360)+((currBody.currRot.y%360))),0f);
		transform.localEulerAngles = new Vector3(0f,Mathf.LerpAngle(transform.localEulerAngles.y, targetOffset.y, 0.1f),0f);
	}
}
