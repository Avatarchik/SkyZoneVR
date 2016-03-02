using UnityEngine;
using System.Collections;

public class StandbySelectionZone : MonoBehaviour 
{
	public enum SwitchTo
	{
		GAME,
		TUTORIAL,
		TUT_STEP3
	}

	public SwitchTo switchTo;

	GameManager gm;
	float timer;
	float timeToHold = 3f;

	void Start () 
	{
		timer = 0.0f;
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	void OnTriggerStay( Collider coll )
	{
		timer += Time.deltaTime;
		if (timer >= timeToHold) 
		{
			timer = 0.0f;
			switch( switchTo )
			{
			case SwitchTo.GAME:
				//gm.SetState (GameManager.GameState.COUNTDOWN);
				break;
			case SwitchTo.TUTORIAL:
				//gm.SetState (GameManager.GameState.TUTORIAL);
				break;
			case SwitchTo.TUT_STEP3:
				//gm.NextTutorialStep();
				break;
			}
		}
	}
	void OnTriggerExit( Collider coll )
	{
		timer = 0.0f;
	}
}
