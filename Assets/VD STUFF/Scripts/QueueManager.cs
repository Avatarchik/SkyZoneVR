using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QueueManager : MonoBehaviour {
	
	public Waypoint m_queueFront;

	public Waypoint m_lastWaypoint;

	[System.NonSerialized]
	public Waypoint m_queueEnd;

	public Waypoint[] waypoints;

	SpawnFloor floor;

	// Use this for initialization
	void Awake () {
		floor = GameObject.Find("Floor").GetComponent<SpawnFloor>();
		m_queueEnd = m_queueFront;

		waypoints = transform.GetComponentsInChildren<Waypoint> ();
	}
	
	public void SpawnNewEnemy(GameObject enemyObject) {
		// If the next spawn point in line is not aleady taken
		if( m_queueEnd.m_occupant == null && m_queueEnd.m_reserved == false ) {
			GameObject enemy = StaticPool.GetObj(enemyObject);
			enemy.transform.position = m_queueEnd.transform.position;
			enemy.GetComponent<Enemy>().Reset();
			// (GameObject)Instantiate( enemyObject, m_queueEnd.transform.position, Quaternion.identity );

			// Turn colliders off when they spawn in the line
			Collider[] colliders = enemy.GetComponentsInChildren<Collider>();
			foreach( Collider col in colliders )
				col.enabled = false;

			m_queueEnd.m_occupant = enemy;

			FaceNextWaypoint(enemy.transform, m_queueEnd);

			// If we aren't at the end of the queue, move the spawnpoint one position back
			if( m_queueEnd.m_previous != null )
				m_queueEnd = m_queueEnd.m_previous;
		}
	}

	/// <summary>
	/// Makes the enemy at the front of the queue start its Move coroutine.
	/// </summary>
	public void StartNextInQueue() {
		if( m_queueFront.m_occupant != null ) {
			List<int> availableColumns = new List<int>();
			for(int i = 0; i < floor.columns; i++) {
				if(!floor.tilesFilled[i, 0]) {
					availableColumns.Add(i);
				}
			}
			if(availableColumns.Count > 0) {
				int column = availableColumns[Random.Range (0, availableColumns.Count)];
				floor.tilesFilled[column, 0] = true;
				Enemy t_obj = m_queueFront.m_occupant.GetComponent<Enemy>();

				// Turn colliders back on when they start moving
				Collider[] cols = t_obj.GetComponentsInChildren<Collider>();
				foreach( Collider col in cols )
					col.enabled = true;

	//			t_obj.collider.enabled = false;		// IDK why this is turned off but it is by default and it works -Eric
				t_obj.curColumn = column;
				t_obj.StartMove();
				m_queueFront.m_occupant = null;
			}
		}

		MoveQueueForward( m_queueFront );
	}

	/// <summary>
	/// Moves the queue forward.
	/// </summary>
	/// <param name="wp">Point on queue to move foward from.</param>
	void MoveQueueForward( Waypoint wp ) {
		Waypoint m_tempWp = wp;

		// Go forward from wp position to find the next open position
		if( wp.m_occupant != null ) {
			// Keep going forward until we reach the last unpopulated waypoint
			while( m_tempWp.m_next != null  ) {
				if(  m_tempWp.m_next.m_reserved == false && m_tempWp.m_next.m_occupant == null )
					m_tempWp = m_tempWp.m_next;
				else
					break;
			}

			FaceNextWaypoint( wp.m_occupant.transform, wp );
			wp.m_occupant.GetComponent<Enemy>().GoToQueuePos( m_tempWp );
			m_tempWp.m_reserved = true;
			wp.m_occupant = null;
			//wp.m_reserved = false;

//			if( wp.m_previous != null ) {
//				m_queueEnd = wp.m_previous;
//				return;
//			}
		}

		// If we aren't at the end of the line, continue moving the next enemy forward
		if( wp.m_previous != null ) 
			MoveQueueForward( wp.m_previous );
		else {
			if( wp == m_queueEnd.m_next ) 
				m_queueEnd = wp.m_previous;
		}
	}

	public void Reset() {
		StopAllCoroutines();

		foreach (Waypoint wp in waypoints) 
		{
			if (wp.m_occupant != null) 
			{
				wp.m_occupant.GetComponent<Enemy> ().StopAllCoroutines ();
				wp.m_occupant.SetActive (false);
			}
			wp.m_occupant = null;
			wp.m_reserved = false;
		}

//		Waypoint temp = m_lastWaypoint;
//		while( temp != null ) {
//			if( temp.m_occupant != null ) {
//				temp.m_occupant.GetComponent<Enemy>().StopAllCoroutines();
//				temp.m_occupant.SetActive( false );
//				temp.m_occupant = null;
//				temp.m_reserved = false;
//			}
//
//			temp = temp.m_next;
//		}
		m_queueEnd = m_queueFront;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
//		Gizmos.DrawSphere( m_queueEnd.transform.position, 0.3f );
	}

	/// <summary>
	/// Has enemy face the next point in queue.
	/// </summary>
	/// <param name="enemy">Enemy transform to be rotated.</param>
	/// <param name="currWp">The current enemy's waypoint.</param>
	void FaceNextWaypoint( Transform enemy, Waypoint currWp ) {
		// I'm using m_previous for the way they look because the enemy model's world forward is facing backwards

		// If we are not at the front of the line look at the point behind you, otherwise look away from the camera
		if( currWp.m_next != null ) {
			enemy.transform.LookAt( new Vector3( currWp.m_next.transform.position.x, enemy.transform.position.y, currWp.m_next.transform.position.z ) );
			enemy.Rotate( new Vector3( 0f, 180f, 0f ) );
		} else {
			enemy.transform.rotation = Quaternion.Euler( ( Camera.main.transform.position - enemy.transform.position ).normalized  * -1f);
		}
	}
}
