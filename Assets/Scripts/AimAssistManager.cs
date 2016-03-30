using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AimAssistManager : MonoBehaviour 
{

	public List<GameObject> onCourtEnemies;

	void Start () 
	{

	}

	public void ClearOnCourtEnemies()
	{
		onCourtEnemies.Clear();
	}

	public GameObject ClosestEnemyToBallDirection(GameObject ball)
	{
		Vector3 ballDir = ball.GetComponent<Rigidbody> ().velocity;
		ballDir.y = 0f;
		ballDir.Normalize ();

		float highestNumber = -1f;
		GameObject closestEnemy = null;

		foreach (GameObject enemy in onCourtEnemies) 
		{
			Vector3 dir = enemy.transform.position - ball.transform.position;
			dir.y = 0f;
			dir.Normalize ();

			if (Vector3.Dot (ballDir, dir) > highestNumber) 
			{
				highestNumber = Vector3.Dot(ballDir, dir);

				if (Vector3.Dot (ballDir, dir) > 0.75f) 
				{
					closestEnemy = enemy;
				} 
				else 
				{
					closestEnemy = null;
				}
			}
		}

		return closestEnemy.gameObject;
	}
}
