using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntroGUI : MonoBehaviour {

//	public GameObject[] unlitBlocks;
//	public GameObject[] litBlocks;

	HighScoreXML highScores;

	public TextMesh dailyScores, weeklyScores, monthlyScores; 

	float timer = 0f;

	enum State {
		Splash, Directions, Score
	}

	State state;

	GameObject splash, score, directions;

	public float splashDuration = 10f, directionDuration = 3f, scoreDuration = 6f;

	public float fadeInSpeed = 1f, fadeOutSpeed = 1f;

	bool gameStarted = false;

	public void TurnOnColor(PlayerColor color) {
//		if(unlitBlocks[(int)color])
//			unlitBlocks[(int)color].SetActive(false);
//		if(litBlocks[(int)color])
//			litBlocks[(int)color].SetActive(true);
	}

	IEnumerator FadeIn(GameObject parent) {
		parent.SetActive(true);
		Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
		
		List<float> endAlphas = new List<float>();
		
		Color t_color = Color.white;
		
		for(int i = 0; i < renderers.Length; i++) {
			t_color = renderers[i].material.color;
			endAlphas.Add(t_color.a);
			t_color.a = 0f;
			renderers[i].material.color = t_color;
		}

		if(!gameStarted) {
			float timer = 0f;
			while(timer < 1f) {
				for(int i = 0; i < renderers.Length; i++) {
					t_color.a = Mathf.Lerp(0f, endAlphas[i], timer);
					renderers[i].material.color = t_color;
				}
				timer += Time.deltaTime/fadeInSpeed;
				yield return null;
			}
		}

		for(int i = 0; i < renderers.Length; i++) {
			t_color.a = Mathf.Lerp(0f, endAlphas[i], 1f);
			renderers[i].material.color = t_color;
		}
	}

	IEnumerator FadeOut(GameObject parent) {
		Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
		
		List<float> endAlphas = new List<float>();
		
		Color t_color = Color.white;

		for(int i = 0; i < renderers.Length; i++) {
			t_color = renderers[i].material.color;
			endAlphas.Add(t_color.a);
			t_color.a = 0f;
			renderers[i].material.color = t_color;
		}

		if(!gameStarted) {
			float timer = 0f;
			while(timer < 1f) {
				for(int i = 0; i < renderers.Length; i++) {
					t_color.a = Mathf.Lerp(endAlphas[i], 0f, timer);
					renderers[i].material.color = t_color;
				}
				timer += Time.deltaTime/fadeOutSpeed;
				yield return null;
			}
		}

		for(int i = 0; i < renderers.Length; i++) {
			t_color = renderers[i].material.color;
			t_color.a = endAlphas[i];
			renderers[i].material.color = t_color;
		}

		parent.SetActive(false);
	}

	void Start () {
		splash = GameObject.Find("Splash");
		score = GameObject.Find("Score");
		directions = GameObject.Find("Directions");

		highScores = HighScoreManager.s_instance.highScores;

		FormatScores(highScores.dailyScores, dailyScores);
		FormatScores(highScores.weeklyScores, weeklyScores);
		FormatScores(highScores.monthlyScores, monthlyScores);

		dailyScores.GetComponent<Renderer>().sortingOrder = 5;
		weeklyScores.GetComponent<Renderer>().sortingOrder = 5;
		monthlyScores.GetComponent<Renderer>().sortingOrder = 5;

		GameObject.Find("Daily Title").GetComponent<Renderer>().sortingOrder = 5;
		GameObject.Find("Weekly Title").GetComponent<Renderer>().sortingOrder = 5;
		GameObject.Find("Monthly Title").GetComponent<Renderer>().sortingOrder = 5;

		score.SetActive(false);

		state = State.Splash;
		//directions.SetActive(false);
	}

	void Update() {
//		if(!gameStarted) {
//			timer += Time.deltaTime;
//
//			switch(state) {
//			case State.Splash:
//				if(timer > splashDuration) {
//					timer = 0f;
//					state = State.Score;
//					StartCoroutine("FadeOut", splash);
//					StartCoroutine("FadeIn", score);
//				}
//				break;
//			case State.Score:
//				if(timer > scoreDuration) {
//					timer = 0f;
//					state = State.Directions;
//					StartCoroutine("FadeOut", score);
//					StartCoroutine("FadeIn", directions);
//				}
//				break;
//			case State.Directions:
//				if(timer > directionDuration) {
//					timer = 0f;
//					state = State.Splash;
//					StartCoroutine("FadeOut", directions);
//					StartCoroutine("FadeIn", splash);
//				}
//				break;
//			default:
//				break;
//			}
//		}
	}

	public void ShowInstructions() {
		GameObject.Find ("startText").GetComponent<Animation>().Stop ();
		GameObject.Find ("text_start").GetComponent<TextMesh>().text = "GAME STARTING!";
		gameStarted = true;
		switch(state) {
		case State.Splash:
			splash.SetActive(false);
			directions.SetActive(true);
//			StartCoroutine("FadeOut", splash);
			StartCoroutine("FadeIn", directions);
			break;
		case State.Score:
			StartCoroutine("FadeOut", score);
			StartCoroutine("FadeIn", directions);
			break;
		case State.Directions:
//				StartCoroutine("FadeOut", directions);
			StartCoroutine("FadeIn", directions);
			break;
		}
	}

	void FormatScores(List<Score> scores, TextMesh textMesh) {
		textMesh.text = "";
		for(int i = 0; i < scores.Count; i++) {
			textMesh.text += scores[i].score.ToString();
			if(scores[i].score/10 == 0) {
				textMesh.text += "  ";
			}

			textMesh.text += " - " + scores[i].time.Day + "/" + scores[i].time.Month + "/";
			textMesh.text += (scores[i].time.Year - scores[i].time.Year/100 * 100) + " ";
			textMesh.text += scores[i].time.Hour + ":";

			if(scores[i].time.Minute < 10)
				textMesh.text += "0";
			textMesh.text += scores[i].time.Minute;
//			if(scores[i].time.Minute % 10 == 0) {
//				textMesh.text += "0";
//			}

			textMesh.text += "\n";
		}
	}
}