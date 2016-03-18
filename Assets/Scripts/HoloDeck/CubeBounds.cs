using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeBounds : MonoBehaviour 
{
	public Material mat;
	MeshFilter mf;

	public List<Transform> trackingPositions;

	void Start () 
	{

	}

	void Update () 
	{
		float maxDist = 0.5f;
		Color col = mat.color;
		col.a = 0;
		Vector3 right = transform.right;

		foreach( Transform trans in trackingPositions )
		{
			float dist;// = plane.GetDistanceToPoint(trans.position);

			if( Mathf.Abs( right.x ) > Mathf.Abs( right.z ) )
				dist = Mathf.Abs(transform.position.z - trans.position.z);
			else
				dist = Mathf.Abs(transform.position.x - trans.position.x);
			

			if( dist < maxDist )
			{
				maxDist = dist;
				col.a = (.85f - dist);// * 130;
			}
		}

		col.a = Mathf.Clamp(col.a, 0, .85f);
		
		mat.color = col;
	}
}