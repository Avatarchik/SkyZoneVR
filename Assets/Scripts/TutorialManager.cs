using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour 
{
	AudioManager am;
	GameManager gm;
	AimAssistManager aam;

	public List<Material> gridMats;

	public bool inTutorial = false;

	enum Tutorial
	{
		ROTATE,
		HITBALL,
		HITENEMY,
		ENEMYJUMP,
		ENEMYTHROW
	}

	Tutorial step = Tutorial.ROTATE;

	public GameObject ballPrefab;
	public GameObject[] tutorialEnemies;
	public int tutorialEnemiesActive;

	GameObject tutorialBall1;
	GameObject tutorialBall2;
	Vector3 tutorialBallSpawnPos1;
	Vector3 tutorialBallSpawnPos2;
	bool inTutorialMode;
	float tutBall1Timer;
	float tutBall2Timer;
	float tutTransitionTimer = 0f;

	//ROTATE Variables
	float initialYRot;
	float minRot;
	float maxRot;
	float rotationProgress;
	public GameObject rotationBarBG;
	public Image rotationBar;
	Transform cameraTransform;

	//HITBALL Variables
	float tutBallLerpTimer = 0f;

	void Start () 
	{
		am = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		aam = gm.gameObject.GetComponent<AimAssistManager> ();

		tutorialBallSpawnPos1 = GameObject.Find ("TutBallSpawn1").transform.position;
		tutorialBallSpawnPos2 = GameObject.Find ("TutBallSpawn2").transform.position;
		cameraTransform = Camera.main.transform;

		rotationBarBG.SetActive (false);

		for (int i = 0; i < tutorialEnemies.Length; i++) 
		{
			tutorialEnemies [i].SetActive (false);
		}
	}
	

	void Update () 
	{
		if (!inTutorial)
			return;

		switch (step) 
		{
		case Tutorial.ROTATE:

			float rot = cameraTransform.rotation.eulerAngles.y - initialYRot;

			if( rot < 0 )
				rot += 360;

			if ( rot > minRot && Mathf.Abs( rot - minRot ) < 10  )
				minRot = rot;
			else if ( rot < maxRot && Mathf.Abs( rot - maxRot ) < 10 )
				maxRot = rot;

			rotationProgress = 1 - (maxRot - minRot) / 360;// (maxRot - minRot) / 360;

			rotationBar.fillAmount = rotationProgress;

			if ( rotationProgress >= 0.98f )
			{
				rotationBar.fillAmount = 1;
				SwitchStep(Tutorial.HITBALL);
			}

			break;

		case Tutorial.HITBALL:
			if (tutorialBall1.GetComponent<EnemyBall> ().fromEnemy && tutorialBall2.GetComponent<EnemyBall>().fromEnemy) 
			{
				tutBallLerpTimer += Time.deltaTime;

				if (tutBallLerpTimer >= 2.5f)
					tutBallLerpTimer = 2.5f;

				tutorialBall1.transform.position = Vector3.Lerp(new Vector3(tutorialBallSpawnPos1.x, 10f, tutorialBallSpawnPos1.z), tutorialBallSpawnPos1, tutBallLerpTimer/2.5f);
				tutorialBall2.transform.position = Vector3.Lerp(new Vector3(tutorialBallSpawnPos2.x, 10f, tutorialBallSpawnPos2.z), tutorialBallSpawnPos2, tutBallLerpTimer/2.5f);
				//print (tutBallLerpTimer);
			} 
			else 
			{
				tutTransitionTimer += Time.deltaTime;

				foreach( Material mat in gridMats )
					mat.SetFloat("_Opacity_Slider", 8f - tutTransitionTimer * 1.6f );

				if (tutTransitionTimer > 2.5f) {

					tutTransitionTimer = 0;
					SwitchStep (Tutorial.HITENEMY);
				}
			}

			break;

		case Tutorial.HITENEMY:

			if (tutorialBall1.GetComponent<EnemyBall> ().tutorialBall == false) {
				tutBall1Timer -= Time.deltaTime;
			}

			if (tutorialBall2.GetComponent<EnemyBall> ().tutorialBall == false) {
				tutBall2Timer -= Time.deltaTime;
			}

			if (tutBall1Timer < 0) {
				tutorialBall1 = null;
				TutorialBallSpawn (tutorialBallSpawnPos1, 1);
				tutBall1Timer = 2.5f;
			}
				
			if (tutBall2Timer < 0) {
				tutorialBall2 = null;
				TutorialBallSpawn (tutorialBallSpawnPos2, 2);
				tutBall2Timer = 2.5f;
			}

			if (tutorialEnemiesActive < 4) 
			{
				tutTransitionTimer += Time.deltaTime;
				tutorialBall1.SetActive (false);
				tutorialBall2.SetActive (false);

				if (tutTransitionTimer > 3) 
				{
					SwitchStep (Tutorial.ENEMYJUMP);
					tutTransitionTimer = 0f;
				}						
			}

			break;

		case Tutorial.ENEMYJUMP:
			if (tutorialBall1.GetComponent<EnemyBall> ().tutorialBall == false) {
				tutBall1Timer -= Time.deltaTime;
			}

			if (tutorialBall2.GetComponent<EnemyBall> ().tutorialBall == false) {
				tutBall2Timer -= Time.deltaTime;
			}

			if (tutBall1Timer < 0) {
				tutorialBall1 = null;
				TutorialBallSpawn (tutorialBallSpawnPos1, 1);
				tutBall1Timer = 2.5f;
			}

			if (tutBall2Timer < 0) {
				tutorialBall2 = null;
				TutorialBallSpawn (tutorialBallSpawnPos2, 2);
				tutBall2Timer = 2.5f;
			}

			if (tutorialEnemiesActive < 4) 
			{
				tutTransitionTimer += Time.deltaTime;
				tutorialBall1.SetActive (false);
				tutorialBall2.SetActive (false);

				if (tutTransitionTimer > 3) 
				{
					SwitchStep (Tutorial.ENEMYTHROW);
					tutTransitionTimer = 0f;
				}	
			}

			break;

		case Tutorial.ENEMYTHROW:

			if (tutorialEnemiesActive < 4) 
			{
				EndTutorial ();
				gm.StartGame();
			}

			break;
		}
	}

	void SwitchStep(Tutorial newStep)
	{
		switch (newStep) 
		{
		case Tutorial.ROTATE:

			initialYRot = cameraTransform.rotation.eulerAngles.y;
			minRot = 0;
			maxRot = 360;
			rotationBarBG.SetActive (true);
			rotationBar.gameObject.SetActive (true);
			break;

		case Tutorial.HITBALL:
			rotationBarBG.SetActive (false);
			tutBallLerpTimer = 0f;
			TutorialBallSpawn (new Vector3(0, 5f, 0) + tutorialBallSpawnPos1, 1);
			TutorialBallSpawn (new Vector3(0, 5f, 0) + tutorialBallSpawnPos2, 2);
			break;

		case Tutorial.HITENEMY:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;
				tutorialEnemies [i].transform.position = new Vector3 (tutorialEnemies [i].transform.position.x, 3f, tutorialEnemies [i].transform.position.z);

				aam.onCourtEnemies.Add (tutorialEnemies [i]);
			}
			tutorialEnemiesActive = tutorialEnemies.Length;

			GameObject[] staticPoolBalls = GameObject.FindGameObjectsWithTag ("Ball");
			foreach (GameObject ball in staticPoolBalls) {
				ball.SetActive (false);
			}

			tutBall1Timer = 2.5f;
			tutBall2Timer = 2.5f;
			TutorialBallSpawn (tutorialBallSpawnPos1, 1);
			TutorialBallSpawn (tutorialBallSpawnPos2, 2);

			break;

		case Tutorial.ENEMYJUMP:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().StartCoroutine ("Hop", tutorialEnemies [i].GetComponent<Enemy> ().tutorialHop);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;

				aam.onCourtEnemies.Add (tutorialEnemies [i]);
			}
			tutorialEnemiesActive = tutorialEnemies.Length;

			staticPoolBalls = GameObject.FindGameObjectsWithTag ("Ball");
			foreach (GameObject ball in staticPoolBalls) {
				ball.SetActive (false);
			}

			if (tutorialBall1.activeSelf == false)
				tutorialBall1.SetActive (true);

			if (tutorialBall2.activeSelf == false)
				tutorialBall2.SetActive (true);

			tutBall1Timer = 1f;
			tutBall2Timer = 1f;

			if (tutorialBall1 == null)
				TutorialBallSpawn (tutorialBallSpawnPos1, 1);
			else
				tutorialBall1.transform.position = tutorialBallSpawnPos1;

			if (tutorialBall2 == null)
				TutorialBallSpawn (tutorialBallSpawnPos2, 2);
			else
				tutorialBall2.transform.position = tutorialBallSpawnPos2;

			break;

		case Tutorial.ENEMYTHROW:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().StartCoroutine ("Hop", tutorialEnemies [i].GetComponent<Enemy> ().tutorialHop);
				//tutorialEnemies[i].GetComponent<Enemy>().InvokeRepeating("Hop" 2f, 2f);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;
				tutorialEnemies [i].GetComponent<Enemy> ().waitToThrow = true;
				tutorialEnemies [i].GetComponent<Enemy> ().throwWaitTime = i + i;
				tutorialEnemies [i].GetComponent<Enemy> ().StartCoroutine ("ThrowRoutine");

				aam.onCourtEnemies.Add (tutorialEnemies [i]);
			}
			tutorialEnemiesActive = tutorialEnemies.Length;

			staticPoolBalls = GameObject.FindGameObjectsWithTag("Ball");
			foreach (GameObject ball in staticPoolBalls) 
			{
				ball.SetActive (false);
				ball.GetComponent<EnemyBall> ().tutorialBall = false;
			}

			break;
		}

		step = newStep;
	}

	void TutorialBallSpawn(Vector3 spawnPos, int ballNum)
	{
		if (ballNum == 1) 
		{
			tutorialBall1 = StaticPool.GetObj (ballPrefab);
			tutorialBall1.GetComponent<EnemyBall> ().Reset ();
			tutorialBall1.GetComponent<EnemyBall> ().tutorialBall = true;
			tutorialBall1.transform.position = spawnPos;
		}

		if (ballNum == 2) 
		{
			tutorialBall2 = StaticPool.GetObj (ballPrefab);
			tutorialBall2.GetComponent<EnemyBall> ().Reset ();
			tutorialBall2.GetComponent<EnemyBall> ().tutorialBall = true;
			tutorialBall2.transform.position = spawnPos;
		}
	}

	public void StartTutorial()
	{
		inTutorial = true;
		SwitchStep (Tutorial.ROTATE);
	}

	public void EndTutorial()
	{
		inTutorial = false;
		gm.inTutorialMode = false;

		foreach (GameObject tutEnemy in tutorialEnemies) 
		{
			tutEnemy.SetActive (false);
		}
	}
}
