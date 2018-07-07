﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    
    [Range(0,1)] float movementFactor; // 0 for not moved, 1 for fully moved

    Vector3 startingPos; // must be stored for absolute movement

	// Use this for initialization
	void Start ()
    {
        startingPos = transform.position;
        PeriodNaNCatcher();
    }

    // Update is called once per frame
    void Update () {
        // todo protect against period is zero
        PeriodNaNCatcher();
        float cycles = Time.time / period; // grows continually from 0

        const float tau = Mathf.PI * 2; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
	}

    private void PeriodNaNCatcher()
    {
        if (period == Mathf.Epsilon)
        { 
            period = 2f;
        }
    }
}
