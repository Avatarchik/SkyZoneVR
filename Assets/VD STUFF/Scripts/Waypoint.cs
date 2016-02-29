using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	public Waypoint m_next;
	public Waypoint m_previous;
	public GameObject m_occupant;
	public bool m_reserved;


	void OnDrawGizmosSelected() {

		if( m_next != null ) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine( transform.position, m_next.transform.position );

		}
		if( m_previous != null ) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine( transform.position, m_previous.transform.position );
		}
	}
}
