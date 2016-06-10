using UnityEngine;

[RequireComponent(typeof(Light))]
public class LampeRadar : MonoBehaviour {

    private Lampe lampe;
    private Light lumiere;

	// Use this for initialization
	void Start () {
        lampe = FindObjectOfType<Lampe>();
        lumiere = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        lumiere.enabled = lampe.lampe.enabled;
        lumiere.intensity = lampe.lampe.intensity * 8f / lampe.intensiteMax;
	}
}
