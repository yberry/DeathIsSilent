using UnityEngine;

[RequireComponent(typeof(Light))]
public class Oeil : MonoBehaviour {

    private float range;
    private float rayon;

	// Use this for initialization
	void Start () {
        Light oeil = GetComponent<Light>();
        oeil.type = LightType.Spot;
        range = oeil.range;
        rayon = range * Mathf.Tan(oeil.spotAngle * Mathf.Deg2Rad / 2f);
        oeil.enabled = false;
    }
	
	public Vector3 GetRandomDir()
    {
        Vector3 z = range * transform.forward;
        float angle = Random.Range(0f, 2 * Mathf.PI);
        float r = Random.Range(0f, rayon);
        Vector3 disque = r * (Mathf.Cos(angle) * transform.right + Mathf.Sin(angle) * transform.up);
        return z + disque;
    }
}
