using UnityEngine;
using System.Collections;

public class UI_Tracking : MonoBehaviour 
{

	public Transform playerHeadTransform;
	public Transform uiTrackingTransform;

	void Start () 
	{
	
	}

	void Update () 
	{
		transform.position = uiTrackingTransform.position;
		transform.rotation = Quaternion.LookRotation((playerHeadTransform.position - uiTrackingTransform.position) * -1);
	}
}
