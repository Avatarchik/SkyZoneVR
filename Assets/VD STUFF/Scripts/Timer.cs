using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

	GameManager manager;
	TextMesh textMesh;
	bool finalSecondsSequenceStarted = false;
	
	void OnLevelWasLoaded(int level) {
		manager = GameManager.instance;

		if(manager == null) {
			Debug.Log( gameObject.name + " could not find GameManager instance." );
			Destroy(this);
		}

		textMesh = GetComponent<TextMesh>();
		transform.LookAt (Camera.main.transform.position + Camera.main.transform.forward * 1000f);
		textMesh.GetComponent<Renderer>().sortingOrder = 15;
	}

	void Update () {
		if(manager) {
			string minutes = ((int)(manager.timer / 60)).ToString();
			string seconds = ((int)(manager.timer % 60f)).ToString();
		
			if(seconds.Length == 1) {
				seconds = "0"+seconds;
			}

			textMesh.text = minutes + ":" + seconds;

			// Star final sequence for timer
			if( manager.timer <= 12f && !finalSecondsSequenceStarted ){
				finalSecondsSequenceStarted = true;
				StartFinalSequence();
			}
		}
	}

	void StartFinalSequence() {
		StartCoroutine( "MoveToFinalPos" );
	}

	IEnumerator MoveToFinalPos() {
		Vector3 startPos = transform.position;
		Vector3 tartgetPos = new Vector3( 0f, 6.5f, startPos.z );
		Vector3 startScale = transform.localScale;
		Vector3 targetScale = new Vector3( 0.04f, 0.04f, startScale.z );
		float lerpTime = 2f;
		float timer = 0.0f;

		while( timer <= 1.0f ) {
			transform.position = Vector3.Lerp(startPos, tartgetPos, timer);
			transform.localScale = Vector3.Lerp(startScale, targetScale, timer);
			
			timer += Time.deltaTime / lerpTime;
			yield return null;
		}

		StartCoroutine( "ScaleText" );
	}

	IEnumerator ScaleText() {
		Vector3 startScale = transform.localScale;
		Vector3 largeScale =  new Vector3( 0.06f, 0.06f, startScale.z );
		int lastTime = (int)manager.timer;
		float lerpTime = 1.0f;
		float timer = 0.0f;

		while( manager.timer > 0 ) {
			while( timer <= 1.0f ) {
				// if the display time changes before we are finished scaling, start enlarge the display again
				if(lastTime > manager.timer)
					break;

				transform.localScale = Vector3.Lerp( largeScale, startScale, timer );

				timer += Time.deltaTime / lerpTime;
				yield return null;
			}

			lastTime = (int)manager.timer;
			timer = 0.0f;
		}
	}
}
