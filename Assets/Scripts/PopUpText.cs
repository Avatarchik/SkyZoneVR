using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour 
{

	public float destroyTime = 1.5f;
	float timer = 0;

	GameManager gm;
	TextMesh text;

	Vector3 startPos;
	Vector3 endPos;
	Vector3 startRot;

	void Start () 
	{
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		text = GetComponentInChildren<TextMesh> ();
		text.text = "+" + (int) 1 * gm.streakMultiplier;

		switch (gm.streakMultiplier) 
		{
		case 1:
			text.color = new Color (255, 0, 0);
			break;
		case 2:
			text.color = new Color (255, 227, 0);
			break;
		case 3:
			text.color = new Color (0, 255, 170);
			break;
		}

		startPos = transform.position;
		endPos = transform.position + new Vector3 (0, 0.5f, 0);
	}

	void Update () 
	{
		timer += Time.deltaTime;

		if (timer > destroyTime) 
		{
			Destroy (this.gameObject);
		}

		transform.position = Vector3.Lerp (startPos, endPos, timer / destroyTime);
		transform.rotation = Quaternion.Euler (Vector3.Lerp (Vector3.zero, new Vector3 (0, -360, 0), timer / 0.3f));
	}
}
