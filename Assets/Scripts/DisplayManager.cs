using UnityEngine;
using System.Collections;

public class DisplayManager : MonoBehaviour 
{

	public Camera standbyCamera;
	public MovieTexture standbyVideoTexture;

	public Camera tutorialCamera;
	public MovieTexture tutorialVideo;

	// Use this for initialization
	void Start () 
	{
		if (Display.displays.Length <= 1) {
			print ("NOT ENOUGH DISPLAYS TO PLAY TUTORIAL VIDEO");
			return;
		}

		if (!tutorialCamera.isActiveAndEnabled)
			tutorialCamera.enabled = true;
		tutorialVideo.Play ();
		tutorialVideo.loop = true;
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
