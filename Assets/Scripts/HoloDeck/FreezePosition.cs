using UnityEngine;
using System.Collections;

public class FreezePosition : MonoBehaviour 
{
	void LateUpdate () 
	{
		transform.position = Vector3.zero;
	}
}
