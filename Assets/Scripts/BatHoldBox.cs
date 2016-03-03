using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BatHoldBox : MonoBehaviour {

	GameManager gm;
	float timer;
	public float timeToHold = 2f;
	public Image loadingBar;
	public GameObject loadingBarGO;

	void Start () 
	{
		timer = 0f;
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		loadingBarGO.SetActive(false);
		loadingBar.fillAmount = 0;
	}

	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.tag == "Bat")
			loadingBarGO.SetActive (true);
	}

	void OnTriggerStay (Collider coll) 
	{
		if (coll.gameObject.tag == "Bat") 
		{
			timer += Time.deltaTime;
			loadingBar.fillAmount = timer / timeToHold;
			if (timer >= timeToHold) {
				timer = 0f;
				gm.StartGame ();
				loadingBarGO.SetActive (false);
			}
		}
	}

	void OnTriggerExit(Collider coll)
	{
		timer = 0f;
		loadingBar.fillAmount = 0;
	}
}
