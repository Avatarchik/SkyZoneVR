using UnityEngine;
using System.Collections;

public class Lifetime : MonoBehaviour 
{
	public float lifetime;
	float timer = 0.0f;
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;

		if (timer >= lifetime) 
		{
			timer = 0.0f;
			Destroy(this.gameObject);
			//this.gameObject.SetActive(false);
		}
	}

//	void OnDisable()
//	{
//		timer = 0.0f;
//	}
}
