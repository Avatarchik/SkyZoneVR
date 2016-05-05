using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bat : MonoBehaviour 
{

	public SteamVR_TrackedObject controller;
	public Rigidbody rb;
	Transform controllerTransform;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		controllerTransform = controller.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		rb.MovePosition (controllerTransform.position);
		rb.MoveRotation(controller.transform.rotation);
	}
}
