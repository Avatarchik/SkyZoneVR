using UnityEngine;
using System;
using System.Collections;

public class HighScoreManager : MonoBehaviour {

	[System.NonSerialized]
	public HighScoreXML highScores;

//	public bool showScores = false;

	public static HighScoreManager s_instance;

	void Awake () {
		highScores = HighScoreXML.Load<HighScoreXML>();
		InvokeRepeating("UpdateScores", 0f, 300f);
		s_instance = this;
	}

	public static void AddScore(int score) {
		s_instance.highScores.AddScore(new Score(score, DateTime.Now));

	}

	void UpdateScores() {
		highScores.UpdateScores();
	}
}