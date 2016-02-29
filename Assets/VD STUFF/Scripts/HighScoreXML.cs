using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

[XmlRoot("HighScore")]
public class HighScoreXML : XMLBase {

	override protected void Init() {
		dailyScores = new List<Score>();
		weeklyScores = new List<Score>();
		monthlyScores = new List<Score>();
		Debug.Log("init");
		base.Init ();
	} 
	
	[XmlArray("DailyScores"), XmlArrayItem("ScoreXML")]
	public List<Score> dailyScores;

	[XmlArray("weeklyScores"), XmlArrayItem("ScoreXML")]
	public List<Score> weeklyScores;

	[XmlArray("monthlyScores"), XmlArrayItem("ScoreXML")]
	public List<Score> monthlyScores;

	public void UpdateScores() {
		for(int i = 0; i < dailyScores.Count; i++) {
			if(DateTime.Compare(dailyScores[i].time, DateTime.Now.Subtract(TimeSpan.FromDays(1.0))) < 0) {
				dailyScores.RemoveAt(i);
				i--;
			}
		}
		for(int i = 0; i < weeklyScores.Count; i++) {
			if(DateTime.Compare(weeklyScores[i].time, DateTime.Now.Subtract(TimeSpan.FromDays(7.0))) < 0) {
				weeklyScores.RemoveAt(i);
				i--;
			}
		}
		for(int i = 0; i < monthlyScores.Count; i++) {
			if(DateTime.Compare(monthlyScores[i].time, DateTime.Now.Subtract(TimeSpan.FromDays(30.0))) < 0) {
				monthlyScores.RemoveAt(i);
				i--;
			}
		}
		Save();
		
	}

	public bool AddScore(Score score) {
		bool added = false;

		added = ScoreParseAndTrim(score, dailyScores, 3);
		added = ScoreParseAndTrim(score, weeklyScores, 3);
		added = ScoreParseAndTrim(score, monthlyScores, 3);
		Save();
		return added;	
	}

	bool ScoreParseAndTrim(Score score, List<Score> scoresList, int maxSize) {
		bool added = false;
		for(int i = 0; i < scoresList.Count; i++) {
			if(scoresList[i].score < score.score) {
				scoresList.Insert(i, score);
				added = true;
				break;
			}
		}
		//TODO WHY MORE THAN SIZE
		if(scoresList.Count > maxSize) {
			scoresList.RemoveRange(maxSize, scoresList.Count - maxSize);
		} else {
			if(!added) {
				scoresList.Insert(scoresList.Count, score);
				added = true;
			}
		}
		return added;
	}

	public void ClearDailyScore(int index) {
		if(dailyScores.Count - 1 < index)
			return;
		dailyScores.RemoveAt(index);
	}

	public void ClearWeeklyScore(int index) {
		if(weeklyScores.Count - 1 < index)
			return;
		weeklyScores.RemoveAt(index);
	}

	public void ClearMonthlyScore(int index) {
		if(monthlyScores.Count - 1 < index)
			return;
		monthlyScores.RemoveAt(index);
	}

	public void ClearAllScores() {
		dailyScores.Clear();
		weeklyScores.Clear();
		monthlyScores.Clear();
	}
}

public class Score {
	public Score() {
		score = 0;
		time = DateTime.Now;
	}
	public Score(int p_score, DateTime p_time) {
		score = p_score;
		time = p_time;
	}
	public int score;
	public DateTime time;
}