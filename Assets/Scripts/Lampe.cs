﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class Lampe : MonoBehaviour {

    public OVRInput.Button switcher;
    [Range(0f, 8f)]
    public float intensiteMin;
    [Range(0f, 8f)]
    public float intensiteMax;
    public Collider[] cones;

    private Light lampe;
    private float frequenceClignotement = 0f;
    private Collider col;

    // Use this for initialization
    void Start() {
        lampe = GetComponent<Light>();
        if (intensiteMax < intensiteMin)
        {
            intensiteMax = intensiteMin;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (frequenceClignotement > 0f)
        {
            frequenceClignotement -= Time.deltaTime;
            if (frequenceClignotement <= 0f)
            {
                frequenceClignotement = 0f;
            }
        }
        lampe.intensity = (Mathf.Cos(Time.time * frequenceClignotement / 10) * (intensiteMax - intensiteMin) + intensiteMax + intensiteMin) / 2;
    }

    public void SetFreq(float freq)
    {
        frequenceClignotement = freq;
    }

    public void Switch()
    {
        lampe.enabled = !lampe.enabled;
        foreach (Collider cone in cones)
        {
            cone.enabled = !cone.enabled;
        }
    }
}
