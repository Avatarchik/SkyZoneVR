using UnityEngine;
using System.Collections;

public class DisplayManager : MonoBehaviour 
{

	public Camera standbyCamera;
	public MovieTexture standbyVideoTexture;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void EnableStandbyCamera()
	{
		if (!standbyCamera.isActiveAndEnabled)
			standbyCamera.enabled = true;

		if(!standbyVideoTexture.isPlaying)
			standbyVideoTexture.Play ();

		standbyVideoTexture.loop = true;
	}

	public void DisableStandbyCamera()
	{
		standbyCamera.enabled = false;
		standbyVideoTexture.Stop ();
	}
}
