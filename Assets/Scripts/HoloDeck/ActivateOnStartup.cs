using UnityEngine;
using System.Collections;

public class ActivateOnStartup : MonoBehaviour 
{

	void Start () 
	{
		gameObject.SetActive (true);
	}

	void Update()
	{
		gameObject.SetActive (true);

	}
}
