using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//=============================================================================----
// Author: Bradley Newman - USC Worldbuilding Media Lab - worldbuilding.usc.edu
//=============================================================================----

// This script will read the tracking data from OptitrackRigidBodyManager.cs
// for the rigid body that corresponds to the ID defined in this script.
// Usage: Attach OptitrackRigidBody.cs to an empty Game Object
// and enter the ID number as specified in the Motive > Rigid Body Settings > Advanced > User Data field.
// Requirements:
// 1. Instance of OptitrackRigidBodyManager.cs

public class OptitrackRigidBody : MonoBehaviour {	
	public int ID;

	private bool foundIndex = false;
	[HideInInspector]
    public int index;

    public bool usePostionTracking = true;
    public bool useRotationTracking = true;

    public GameObject originOverride;

	public bool predictLostData = false;

	List<Vector3> pastPositions = new List<Vector3>();
	Vector3 lastPosition = Vector3.zero;
	public Vector3 currRot;

    void Start() 
	{
		for (int i = 0; i < 3; ++i) 
		{
			pastPositions.Add(Vector3.zero);
		}
        //optitrackTransform  = new GameObject().transform;
    }

	void Update () {
		//If we have received a packet from Motive then look for the rigid body ID index
		if(foundIndex == false) 
		{
			if(OptitrackRigidBodyManager.instance.receivedFirstRigidBodyPacket) 
			{
				if(foundIndex == false) 
				{
					for(int i = 0; i < OptitrackRigidBodyManager.instance.rigidBodyIDs.Length; i++) 
					{
						//Looking for ID in array of rigid body IDs
						if(OptitrackRigidBodyManager.instance.rigidBodyIDs[i] == ID) 
						{
							index = i; //Found ID
						}
					}
					foundIndex = true;
				}
			}
		}
		else 
		{
            if (usePostionTracking)
			{
                if (originOverride != null)
                {
					if( gameObject.GetComponent<Rigidbody>() )
						gameObject.GetComponent<Rigidbody>().MovePosition( (OptitrackRigidBodyManager.instance.rigidBodyPositions[index] - OptitrackRigidBodyManager.instance.origin.position) + originOverride.transform.position );
					else
                    	transform.position = (OptitrackRigidBodyManager.instance.rigidBodyPositions[index] - OptitrackRigidBodyManager.instance.origin.position) + originOverride.transform.position;
                }
				else if( gameObject.GetComponent<Rigidbody>() )
				{
					gameObject.GetComponent<Rigidbody>().MovePosition( OptitrackRigidBodyManager.instance.rigidBodyPositions[index] );
				}
				else
				{
					Vector3 newPos = OptitrackRigidBodyManager.instance.rigidBodyPositions[index];
					if( predictLostData )
					{
						if( Vector3.SqrMagnitude(lastPosition - newPos) < 9.99999944E-11f )//lost data
						{
							Vector3 velocity = pastPositions[pastPositions.Count - 1] - pastPositions[pastPositions.Count - 2];
							newPos = pastPositions[pastPositions.Count - 1] + velocity;
						}

						lastPosition = OptitrackRigidBodyManager.instance.rigidBodyPositions[index];

						pastPositions.RemoveAt(0);

						pastPositions.Add(newPos);
					}
					transform.position = newPos;//OptitrackRigidBodyManager.instance.rigidBodyPositions[index];
				}
			}
			Quaternion newRot = OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
			currRot = newRot.eulerAngles;
            if (useRotationTracking)
			{
                if (originOverride != null)
                {
                    //Subtract the origin rotation used by OptitrackRigidBodyManager
                    transform.rotation = Quaternion.Inverse(OptitrackRigidBodyManager.instance.origin.rotation) * OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
                    //Add the originOverride rotation applied to this rigid body
                    transform.rotation = originOverride.transform.rotation * transform.rotation;
				}
//				else if( gameObject.GetComponent<Rigidbody>() )
//				{
//					gameObject.GetComponent<Rigidbody>().MoveRotation( OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index] );
//				}
                else
				{
//					float angle = Quaternion.Angle( transform.rotation, newRot );
//					if( angle <= 120 )
						transform.rotation = newRot;
				}
			}
		}
	}

    public Vector3 GetPostion() {
        return OptitrackRigidBodyManager.instance.rigidBodyPositions[index];
    }

    public Quaternion GetRotationQuaternion() {
        if (foundIndex)
            return OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
        else
            return Quaternion.identity;
    }

    public Vector3 GetRotationEuler() {
        if (foundIndex)
            return OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index].eulerAngles;
        else
            return Vector3.zero;
    }

    /*
    public Transform GetTransform() {
        optitrackTransform.position = OptitrackRigidBodyManager.instance.rigidBodyPositions[index];
        optitrackTransform.rotation = OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
        return optitrackTransform;
    }*/
}