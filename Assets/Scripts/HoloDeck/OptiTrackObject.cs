/**
 * Adapted from johny3212
 * Written by Matt Oskamp
 */
using UnityEngine;
using System.Collections;

public class OptiTrackObject : MonoBehaviour {

	public int rigidbodyIndex;
	public Vector3 rotationOffset;

	public bool trackRotation = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//print(Quaternion.Euler (rotationOffset));
		Vector3 pos = OptiTrackManager.Instance.getPosition(rigidbodyIndex);
		//pos = new Vector3 (-pos.x, pos.y, -pos.z);
		//pos *= 3;
		//pos *= .5f;
//		pos.x *= 3;
//		pos.y *= 2;
//		pos.z *= 3;
		this.transform.position = pos;

		if (trackRotation) 
		{
			Quaternion rot = OptiTrackManager.Instance.getOrientation(rigidbodyIndex);
			rot = Quaternion.Euler(-rot.eulerAngles.x, -rot.eulerAngles.y, rot.eulerAngles.z);
			//Vector3 vRot = rot.eulerAngles;
			//rot = Quaternion.Euler(0, -vRot.z, 0);
			//rot = rot * Quaternion.Euler(rotationOffset);
			this.transform.rotation = rot;
		}
	}
}
