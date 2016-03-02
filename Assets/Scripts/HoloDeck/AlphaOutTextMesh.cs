using UnityEngine;
using System.Collections;

public class AlphaOutTextMesh : MonoBehaviour 
{
	public float timeToAlpha = 2; //in seconds

	Color color;
	TextMesh tm;
	// Use this for initialization
	void Start () 
	{
		tm = gameObject.GetComponent<TextMesh> ();
	}

	void Update () 
	{
		color = tm.color;

		if (color.a > 0)
		{
			color.a -= timeToAlpha * Time.deltaTime;
			tm.color = color;
		}
	}
}
