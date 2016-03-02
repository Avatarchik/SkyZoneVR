using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	//public List<AudioSource> soundEffects;

	public AudioSource dodgeballHit;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void DodgeballHitSound()
	{
		dodgeballHit.Play ();
	}
}
