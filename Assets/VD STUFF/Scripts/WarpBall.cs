using UnityEngine;
using System.Collections;

public class WarpBall : MonoBehaviour {
	void Start () {
		Destroy (gameObject, GetComponent<Animation>().clip.length);
	}
}
