using UnityEngine;
using System.Collections;

public class ColliderDisableZone : MonoBehaviour 
{
	void OnTriggerEnter(Collider coll)
	{
		if (coll.tag == "TargetClone") 
		{
			coll.tag = "Untagged";
		}
	}
}
