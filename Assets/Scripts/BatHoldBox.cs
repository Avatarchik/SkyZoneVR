using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BatHoldBox : MonoBehaviour {

	GameManager gm;
	float timer;
	public float timeToHold = 1.5f;
	public Image loadingBar;
	public GameObject loadingBarGO;
	public BatHoldBox otherBatHoldBox;
	public bool batEntered = false;
	public bool easyMode;

	void Start () 
	{
		timer = 0f;
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		loadingBarGO.SetActive(false);
		loadingBar.fillAmount = 0;
	}

	void Update()
	{
		if (timer <= 0) 
		{
			loadingBarGO.SetActive (false);
		}

		if (gm.gamePhaseInt > 0 && loadingBarGO.activeSelf == true)
			loadingBarGO.SetActive (false);
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.tag == "Bat") 
		{
			batEntered = true;
			if(otherBatHoldBox.batEntered == false)
				loadingBarGO.SetActive (true);
		}
	}

	void OnTriggerStay (Collider coll) 
	{
		if (coll.gameObject.tag == "Bat") 
		{
			if (otherBatHoldBox.batEntered == false) 
			{
				timer += Time.deltaTime;
				loadingBar.fillAmount = timer / timeToHold;
				if (timer >= timeToHold) 
				{
					timer = 0f;
					gm.StartCountdown (easyMode);
					loadingBarGO.SetActive (false);
				} 
			} 
			else 
			{
				loadingBarGO.SetActive (false);
			}
		}
	}

	void OnTriggerExit(Collider coll)
	{
		timer = 0f;
		loadingBar.fillAmount = 0;
		batEntered = false;
	}
}
