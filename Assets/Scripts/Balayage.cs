using UnityEngine;
using System.Collections;

public class Balayage : MonoBehaviour {

    public static bool detect = false;
    public static GameObject detected;

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<EnnemiRadar>() == null)
        {
            return;
        }
        detect = true;
        detected = col.gameObject;
        col.GetComponent<EnnemiRadar>().dejaEuCollision = true;
        col.GetComponent<Renderer>().enabled = true;

    }

    void OnDestroy()
    {
        detect = false;
        detected = null;
    }
}
