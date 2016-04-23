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
	public AudioSource bombExplosion;
    public AudioSource countdownBoop;

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

			if (switchToAudio.volume >= 1) 
			{
				switchToAudio.volume = 1;
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

	public void DodgeballHitSound()
	{
		dodgeballHit.Play ();
	}

	public void EnemyHitSound()
	{
		enemyHit.Play ();
	}

	public void BombExplosionSound()
	{
		bombExplosion.Play ();
	}

    public void CountdownBoopSound()
    {
        if (playBeepOnce)
            return;

        countdownBoop.Play();
        countdownBoop.PlayDelayed(1);
        countdownBoop.PlayDelayed(2);
        countdownBoop.PlayDelayed(3);
        countdownBoop.PlayDelayed(4);

        playBeepOnce = true;
    }

    public IEnumerator CountdownBoopRoutine()
    {
        playBeepOnce = true;
        countdownBoop.Play();
        yield return new WaitForSeconds(1);
        playBeepOnce = false;
    }
}
