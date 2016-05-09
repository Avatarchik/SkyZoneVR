using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	public AudioSource backgroundMusic;
	public AudioSource ambientCubeAudio;

    bool updateBackgroundAudio = false;
    AudioSource switchToAudio;
    AudioSource switchFromAudio;

    public AudioSource dodgeballHit;
	public AudioSource enemyHit;
	public AudioSource bounceBack;
	public AudioSource heatSeek;
	public AudioSource bombExplosion;
    public AudioSource countdownBoop;
    public AudioSource finalBuzzer;
	public AudioSource loading;
	public AudioSource loadingDone;
	public AudioSource newHighScore;

    public bool playBeepOnce;

	void Start()
	{
		updateBackgroundAudio = false;
	}

	void Update () 
	{
		if (!updateBackgroundAudio)
			return;

		if (switchToAudio.volume < 1f) 
		{
			switchToAudio.volume += Time.deltaTime;
			switchFromAudio.volume = 1f - switchToAudio.volume;

			if (switchToAudio.volume >= 1f) 
			{
				switchToAudio.volume = 1f;
				switchFromAudio.Stop ();
				updateBackgroundAudio = false;
			}
		}
	}

	public void PlayBackgroundMusic()
	{
		updateBackgroundAudio = true;
		switchToAudio = backgroundMusic;
		switchFromAudio = ambientCubeAudio;
		backgroundMusic.Play ();
	}

	public void PlayAmbientCubeAudio()
	{
		updateBackgroundAudio = true;
		switchToAudio = ambientCubeAudio;
		switchFromAudio = backgroundMusic;
		ambientCubeAudio.Play ();
	}

	public void LoadingSound()
	{
		loading.Play ();
	}

	public void LoadingDoneSound()
	{
		loadingDone.Play ();
	}

	public void DodgeballHitSound()
	{
		dodgeballHit.Play ();
	}

	public void EnemyHitSound()
	{
		enemyHit.Play ();
	}

	public void BounceBackSound()
	{
		bounceBack.Play ();
	}

	public void HeatSeekSound()
	{
		heatSeek.Play ();
	}

	public void BombExplosionSound()
	{
		bombExplosion.Play ();
	}

	public void NewHighScoreSound()
	{
		newHighScore.Play ();
	}

    public IEnumerator CountdownBoopRoutine(float gmTimer)
    {
        playBeepOnce = true;
        if (gmTimer < 1)
            finalBuzzer.Play();
        else
            countdownBoop.Play();
        yield return new WaitForSeconds(1);
        playBeepOnce = false;
    }
}
