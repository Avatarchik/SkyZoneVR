using UnityEngine;
using System.Collections;

public class AnalyticsManager : MonoBehaviour 
{

	public GoogleAnalyticsV3 googleAnalytics;
	public GameManager gameManager;

	bool sessionStarted;

	//Gameplay
	bool easyMode;
	int score;

	//Payment / Bill denominations
	public int ones;
	public int fives;
	public int tens;
	public int credit;
	public int free;
	public int totalPayment;
	int plays;

	void Start () 
	{
		gameManager = GetComponent<GameManager> ();

		googleAnalytics.LogScreen ("Start Up - No Plays");
	}

	void Update () 
	{
	
	}

	public void StartSession()
	{
		if (sessionStarted)
			return;
		
		sessionStarted = true;
		googleAnalytics.StartSession ();
		googleAnalytics.LogScreen ("Game Start");
	}

	public void StopSession()
	{
		sessionStarted = false;
		googleAnalytics.StopSession ();
		googleAnalytics.LogScreen ("Game End");

		if (credit >= 1)
			totalPayment = 3;
		else
			totalPayment = ones + (fives * 5) + (tens * 10) - free;

		switch (totalPayment) 
		{
		case 3:
			plays = 1;
			break;
		case 5:
			plays = 2;
			break;
		case 10:
			plays = 4;
			break;
		}
	}

	public void SendAnalyticsData(int gmScore, bool mode)
	{
		score = gmScore;
		easyMode = mode;

		googleAnalytics.LogEvent ("Gameplay", "Score", "Player's Final Score", score);

		if (easyMode) 
		{
			googleAnalytics.LogEvent ("Gameplay", "Mode", "Game Mode Selected", 0);

//			googleAnalytics.LogEvent (new EventHitBuilder ()
//				.SetEventCategory ("Gameplay")
//				.SetEventAction ("Mode")
//				.SetEventLabel ("Easy or Hard Mode")
//				.SetEventValue (0)
//				.SetCustomMetric (0, "Easy"));
		} 
		else 
		{
			googleAnalytics.LogEvent ("Gameplay", "Mode", "Game Mode Selected", 1);

//			googleAnalytics.LogEvent(new EventHitBuilder()
//				.SetEventCategory("Gameplay")
//				.SetEventAction("Mode")
//				.SetEventLabel("Easy or Hard Mode")
//				.SetEventValue(1)
//				.SetCustomMetric(1, "Hard"));
		}

		if (ones - free >= 1)
			googleAnalytics.LogEvent ("Payment", "Cash", "$1 Bill", ones);
		if(fives >= 1)
			googleAnalytics.LogEvent ("Payment", "Cash", "$5 Bill", fives);
		if(tens >= 1)
			googleAnalytics.LogEvent ("Payment", "Cash", "$10 Bill", tens);
		if (credit >= 1)
			googleAnalytics.LogEvent ("Payment", "Credit", "Paid with Credit Card", credit);
		if (free >= 1)
			googleAnalytics.LogEvent ("Payment", "Free", "Free play", free);
		googleAnalytics.LogEvent ("Payment", "Total", "Total Amount Paid Per Session", totalPayment);

		googleAnalytics.LogEvent ("Gameplay", "Plays", "Plays paid for", plays);

		ResetData ();
	}

	void ResetData()
	{
		score = 0;
		easyMode = false;
		ones = 0;
		fives = 0;
		tens = 0;
		credit = 0;
		free = 0;
		totalPayment = 0;
		plays = 0;
	}
}
