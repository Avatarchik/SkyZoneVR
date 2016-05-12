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
	public Text tutorialPleaseEnterText;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
