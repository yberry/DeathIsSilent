using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Ennemi : NetworkBehaviour {

    public Camera vision;

    private bool detecteJoueur = false;
    private Transform joueur;
    private RaycastHit hit;

	// Use this for initialization
	void Start () {
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(joueur);
        transform.position = Vector3.MoveTowards(transform.position, joueur.position, Time.deltaTime);

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            detecteJoueur = hit.transform == joueur;
            if (detecteJoueur)
            {
                Debug.Log("détection");
            }
        }
	}

    void OnDestroy()
    {
        Debug.Log("destruction");
        if (isServer)
        {
            EnnemiSpawner.instance.MonstreAbsent();
        }
    }
}
