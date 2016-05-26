using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BatHoldBox : MonoBehaviour {

	GameManager gm;
	AudioManager am;
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
		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		loadingBarGO.SetActive(false);
		loadingBar.fillAmount = 0;
	}

	void OnEnable()
	{
		timer = 0f;
	}

	void Update()
	{
		if (timer <= 0) 
		{
			loadingBarGO.SetActive (false);
			batEntered = false;
			if(!otherBatHoldBox.batEntered)
				am.loading.Stop ();
		}
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.tag == "Bat") 
		{
			batEntered = true;
			am.LoadingSound ();
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
				coll.gameObject.GetComponent<Bat> ().TriggerHaptic (1000);
				if (timer >= timeToHold) 
				{
					timer = 0f;
					gm.StartCountdown (easyMode);
					loadingBarGO.SetActive (false);
					am.loading.Stop ();
					am.LoadingDoneSound ();
				} 
			} 
			else 
			{
				loadingBarGO.SetActive (false);
				timer = 0f;
			}
		}
	}

	void OnTriggerExit(Collider coll)
	{
		timer = 0f;
		loadingBar.fillAmount = 0;
		batEntered = false;
		if (am.loading.isPlaying && !otherBatHoldBox.batEntered)
			am.loading.Stop ();
	}
}
