﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bat : MonoBehaviour 
{

	public SteamVR_TrackedObject controller;
	public Rigidbody rb;
	Transform controllerTransform;
	bool triggerHaptic;
	float hapticTimer;
	ushort hapticIntensity;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		controllerTransform = controller.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
        rb.MovePosition(controllerTransform.position);
        rb.MoveRotation(controller.transform.rotation);

        if (controller.gameObject.activeInHierarchy == false)
            return;

		var device = SteamVR_Controller.Input ((int)controller.index);

        if (device != null)
        {
            if (triggerHaptic)
            {
                hapticTimer += Time.deltaTime;
                device.TriggerHapticPulse(hapticIntensity);
                if (hapticTimer >= 0.05f)
                {
                    triggerHaptic = false;
                    hapticTimer = 0;
                }
            }
        }
	}

	public void TriggerHaptic(ushort pulse)
	{
		triggerHaptic = true;
		hapticIntensity = pulse;
	}
}
