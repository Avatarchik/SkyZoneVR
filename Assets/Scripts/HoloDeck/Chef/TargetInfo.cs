using UnityEngine;
using System.Collections;

public class TargetInfo : MonoBehaviour 
{
	int score;
	float time;
	bool special;
	int specialID;
	bool badTarget;
	public GameObject particlePrefab;

	public int Score
	{
		get { return score; }
		set { score = value; }
	}

	public float Time
	{
		get { return time; }
		set { time = value; }
	}

	public bool Special
	{
		get { return special; }
		set { special = value; }
	}

	public int SpecialID
	{
		get { return specialID; }
		set { specialID = value; }
	}

	public bool BadTarget
	{
		get { return badTarget; }
		set { badTarget = value; } 
	}
}
