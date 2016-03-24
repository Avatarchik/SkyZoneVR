using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour 
{
	AudioManager am;
	GameManager gm;

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

	GameObject tutorialBall;
	Vector3 tutorialBallSpawnPos;
	bool inTutorialMode;
	float tutBallTimer;
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

		tutorialBallSpawnPos = GameObject.Find ("TutBallSpawn").transform.position;
		cameraTransform = Camera.main.transform;

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
			if (tutorialBall.GetComponent<EnemyBall> ().fromEnemy) 
			{
				tutBallLerpTimer += Time.deltaTime;

				if (tutBallLerpTimer >= 2.5f)
					tutBallLerpTimer = 2.5f;

				tutorialBall.transform.position = Vector3.Lerp(new Vector3(tutorialBallSpawnPos.x, -3f, tutorialBallSpawnPos.z), tutorialBallSpawnPos, tutBallLerpTimer/2.5f);
				print (tutBallLerpTimer);
			} 
			else 
			{
				tutTransitionTimer += Time.deltaTime;

				foreach( Material mat in gridMats )
					mat.SetFloat("_Opacity_Slider", 2.5f - tutTransitionTimer );

				if (tutTransitionTimer > 2.5f) {

					tutTransitionTimer = 0;
					SwitchStep (Tutorial.HITENEMY);
				}
			}

			break;

		case Tutorial.HITENEMY:

			if (tutorialBall.GetComponent<EnemyBall> ().tutorialBall == false) {
				tutBallTimer -= Time.deltaTime;
			}

			if (tutBallTimer < 0) {
				tutorialBall = null;
				TutorialBallSpawn (tutorialBallSpawnPos);
				tutBallTimer = 2.5f;
			}
				

			if (tutorialEnemiesActive < 4) 
			{
				tutTransitionTimer += Time.deltaTime;
				tutorialBall.SetActive (false);

				if (tutTransitionTimer > 3) 
				{
					SwitchStep (Tutorial.ENEMYJUMP);
					tutTransitionTimer = 0f;
				}						
			}

			break;

		case Tutorial.ENEMYJUMP:
			if (tutorialBall.GetComponent<EnemyBall> ().tutorialBall == false) {
				tutBallTimer -= Time.deltaTime;
			}

			if (tutBallTimer < 0) {
				tutorialBall = null;
				TutorialBallSpawn (tutorialBallSpawnPos);
				tutBallTimer = 2.5f;
			}

			if (tutorialEnemiesActive < 4) 
			{
				tutTransitionTimer += Time.deltaTime;
				tutorialBall.SetActive (false);

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
			TutorialBallSpawn (new Vector3(0, -3f, 0) + tutorialBallSpawnPos);
			break;

		case Tutorial.HITENEMY:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;
				tutorialEnemies[i].transform.position = new Vector3(tutorialEnemies[i].transform.position.x, 3f, tutorialEnemies[i].transform.position.z);
			}
			tutorialEnemiesActive = tutorialEnemies.Length;

			GameObject[] staticPoolBalls = GameObject.FindGameObjectsWithTag("Ball");
			foreach (GameObject ball in staticPoolBalls) 
			{
				ball.SetActive (false);
			}

			tutBallTimer = 2.5f;
			TutorialBallSpawn (tutorialBallSpawnPos);

			break;

		case Tutorial.ENEMYJUMP:
			for (int i = 0; i < tutorialEnemies.Length; i++) {
				tutorialEnemies [i].SetActive (true);
				tutorialEnemies [i].GetComponent<Enemy> ().StartCoroutine ("Hop", tutorialEnemies [i].GetComponent<Enemy> ().tutorialHop);
				tutorialEnemies [i].GetComponent<Enemy> ().inTutorialMode = true;
			}
			tutorialEnemiesActive = tutorialEnemies.Length;

			staticPoolBalls = GameObject.FindGameObjectsWithTag("Ball");
			foreach (GameObject ball in staticPoolBalls) 
			{
				ball.SetActive (false);
			}

			if(tutorialBall.activeSelf == false)
				tutorialBall.SetActive (true);

			tutBallTimer = 1f;
			if(tutorialBall == null)
				TutorialBallSpawn (tutorialBallSpawnPos);

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

	void TutorialBallSpawn(Vector3 spawnPos)
	{
		tutorialBall = StaticPool.GetObj (ballPrefab);
		tutorialBall.GetComponent<EnemyBall> ().Reset ();
		tutorialBall.GetComponent<EnemyBall> ().tutorialBall = true;
		tutorialBall.transform.position = spawnPos;
	}

	public void StartTutorial()
	{
		inTutorial = true;
		SwitchStep (Tutorial.ROTATE);
	}

	public void EndTutorial()
	{
		inTutorial = false;
	}
}
