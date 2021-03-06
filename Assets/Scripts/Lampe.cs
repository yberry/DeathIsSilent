﻿using UnityEngine;

[RequireComponent(typeof(Light))]
public class Lampe : MonoBehaviour {

    [Tooltip("Intensité minimum de la lampe quand elle clignote")]
    [Range(0f, 8f)]
    public float intensiteMin;
    [Tooltip("Intensité maximum de la lampe quand elle clignote (par défaut)")]
    [Range(0f, 8f)]
    public float intensiteMax;
    [Tooltip("Colliders du champ lumineux de la lampe")]
    public Collider[] cones;
    [Tooltip("Event sonore pour allumer la lampe")]
    public string eventON;
    [Tooltip("Event sonore pour éteindre la lampe")]
    public string eventOFF;

    [HideInInspector]
    public Light lampe;
    private float frequenceClignotement = 0f;
    private float max = 1f;
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
        lampe.intensity = (1f - frequenceClignotement / max) * (Mathf.Cos(Time.time * frequenceClignotement / 10) * (intensiteMax - intensiteMin) + intensiteMax + intensiteMin) / 2;
    }

    public void SetFreq(float freq)
    {
        frequenceClignotement = freq;
        max = freq;
    }

    public void Switch(bool en)
    {
        AkSoundEngine.PostEvent(en ? eventON : eventOFF, gameObject);
        lampe.enabled = en;
        foreach (Collider cone in cones)
        {
            cone.enabled = en;
        }
    }
}
