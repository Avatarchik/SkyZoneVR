using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationManager : MonoBehaviour 
{
	public Animator chefAnimator;

	Dictionary<string, List<float>> animDelays = new Dictionary<string, List<float>>(); 

	string lastAnim = "";

	void Start () 
	{
		List<float> frames = new List<float>();

		frames.Add(7f/30f); frames.Add(24f/30f);  //frames.Add(65f/30f);
		animDelays.Add("RunAndThrowClockwise", frames );
		frames = new List<float>();

		frames.Add(10f/30f); frames.Add(21f/30f);  //frames.Add(63f/30f);
		animDelays.Add("RunAndThrowCC", frames );
		frames = new List<float>();

		frames.Add(84f/30f); frames.Add(92f/30f);  frames.Add(110f/30f);
		animDelays.Add("CartSpray", frames );
		frames = new List<float>();

		frames.Add(70f/30f); frames.Add(86f/30f); frames.Add(92f/30f); frames.Add(105f/30f); 
		frames.Add(111f/30f); frames.Add(127f/30f); frames.Add(133f/30f); frames.Add(148f/30f); frames.Add(170f/30f); //CCC: This is awful, I'm sorry, please kill me.
		animDelays.Add("CartRapid", frames );
		frames = new List<float>();

		frames.Add(62f/30f); frames.Add(67f/30f);  frames.Add(110f/30f);
		animDelays.Add("BasketSpray", frames );
		frames = new List<float>();

		frames.Add(54f/30f); frames.Add(60f/30f); frames.Add(75f/30f); frames.Add(83f/30f); 
		frames.Add(97f/30f); frames.Add(104f/30f); frames.Add(118f/30f); frames.Add(124f/30f); frames.Add(164f/30f); //CCC: Please.
		animDelays.Add("BasketRapid", frames );
	}

	public List<float> PlayAnim( string animName )
	{
		chefAnimator.Play( animName );

		List<float> delays = new List<float>();
		if( animDelays.ContainsKey(animName) )
			delays = animDelays[animName];
		lastAnim = animName;
		return delays;
	}

	public void ChangeAnimSpeed( float speed )
	{
		chefAnimator.speed = speed;
	}
}
