using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Ennemi : MonoBehaviour {

    public Camera vision;

    private bool detecteJoueur = false;
    private Transform joueur;
    private Renderer joueurRender;

	// Use this for initialization
	void Start () {
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
        joueurRender = joueur.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(joueur);
        transform.position = Vector3.MoveTowards(transform.position, joueur.position, Time.deltaTime);

        if (joueurRender.IsVisibleFrom(vision))
        {
            detecteJoueur = true;
            Debug.Log("detection");
        }
	}

    void OnDestroy()
    {
        Debug.Log("destruction");
        EnnemiSpawner.instance.MonstreAbsent();
    }
}
