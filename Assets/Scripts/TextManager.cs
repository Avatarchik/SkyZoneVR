using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextManager : MonoBehaviour 
{
	//Scoreboard
	public Text scoreText;
	public Text timerText;
	public Text streakText;

	public Text warmUpText;
	public Text hitXBallsText;
	public Text hitXBallsNumberText;
	public Text gameStartingText;

	//HUD
	public Text countdownText;
	//public Text finalScoreText;

	//ScoreCard
	public Text scHighScore;
	public Text scScore;
	public Text scBestStreak;
	public Transform scoreCardTrans;
	public Transform scoreCardNewHighScoreTrans;
	public Image easyModeStamp;
	public Image hardModeStamp;

	//TutorialScreen
	public Text tutorialScreenText;
	public Text tutorialScreenDollarsText;
	public Text tutorialPlaysText;
	public Text tutorialReadyText;
	public Text tutorialPleaseEnterText;
    public Text tutorialGameInProgressText;
    public Text tutorialGameEndingText;
	public Text tutorialFlashText;
	public Text tutorialCardDeclinedText;
	public Text tutorialProcessingCardText;

	bool flash;
	float flashTimer;

	bool pulse;
	float pulseTimer;
	Color originalTextColor;
	Color pulseColor;
	Text pulseText;

	void Start () 
	{

	}

	void Update () 
	{
		if (flash) 
		{
			flashTimer -= Time.deltaTime;
			tutorialFlashText.color = new Color (255, 255, 255, flashTimer * 3); //new Color (25, 230, 8, flashTimer * 3);
			if (flashTimer <= 0) 
			{
				tutorialFlashText.gameObject.SetActive (false);
				flash = false;
			}
		}

		if (pulse) 
		{
			pulseTimer -= Time.deltaTime;
			pulseText.color = Color.Lerp(originalTextColor, pulseColor, Mathf.PingPong(pulseTimer/1, 1));
			if (pulseTimer <= 0) 
			{
				pulseText.color = originalTextColor;
				pulse = false;
				pulseText = null;
			}
		}
	}

	public void FlashDollarAmount(int dollarAmount)
	{
		tutorialFlashText.gameObject.SetActive (true);
		flash = true;
		flashTimer = 1;
		tutorialFlashText.color = new Color (255, 255, 255, 255); //new Color (25, 230, 8, 255);
		tutorialFlashText.text = "+$" + dollarAmount;
	}

	public IEnumerator CardDeclinedText()
	{
		tutorialCardDeclinedText.gameObject.SetActive (true);
		yield return new WaitForSeconds (3);
		tutorialCardDeclinedText.gameObject.SetActive (false);
	}

	public void PulseScoreboardText(Text textToPulse)
	{
		originalTextColor = textToPulse.color;
		pulseColor = new Color(1f, 0.67f, 0.67f, 1f);
		pulse = true;
		pulseTimer = 1;
		pulseText = textToPulse;
		//textToPulse.color = Color.Lerp(originalColor, pulseColor, Mathf.PingPong(
	}
}
