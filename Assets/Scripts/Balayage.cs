using UnityEngine;
using System.Collections;

public class Balayage : MonoBehaviour {

    [HideInInspector]
    public bool detect = false;

    void OnTriggerEnter(Collider col)
    {
        detect = true;
        col.GetComponent<EnnemiRadar>().dejaEuCollision = true;
        col.GetComponent<Renderer>().enabled = true;
    }
}
