using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Ennemi : NetworkBehaviour {

    public Camera vision;
    public float vitesseNormale = 1f;
    public float vitesseAttaque = 2f;

    private bool deplacementRandom = false;
    private Vector3 cible;
    private Vector3 cibleTemp;
    private float tempsDeplacement;

    private bool detecteLumiere = false;
    private bool detecteJoueur = false;
    private Transform joueur;
    private Collider lumiere;
    private RaycastHit hit1;
    private RaycastHit hit2;
    private int zones = 1;

	// Use this for initialization
	void Start () {
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {

        if (!detecteLumiere)
        {
            detecteJoueur = false;
            MouvementRandom();
        }
        else
        {
            if (!detecteJoueur)
            {
                DetectionLumiere();
            }
            else
            {
                DetectionJoueur();
            }
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit1))
        {
            if (hit1.collider == lumiere)
            {
                if (Physics.Raycast(hit1.point, joueur.position, out hit2))
                {
                    if (hit2.transform == joueur)
                    {
                        detecteLumiere = true;
                        Debug.Log("détection lumière");
                    }
                }
            }

            detecteJoueur = detecteLumiere && hit1.transform == joueur;
            if (detecteJoueur)
            {
                Debug.Log("détection joueur");
            }
        }
	}

    void MouvementRandom()
    {
        if (!deplacementRandom)
        {
            cibleTemp = transform.position + 5 * transform.forward;
            float angle = Random.Range(0f, 2 * Mathf.PI);
            cible = cibleTemp + 5 * new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            tempsDeplacement = Random.Range(2f, 6f);
            deplacementRandom = true;
            Debug.Log(transform.position);
            Debug.Log(cibleTemp);
            Debug.Log(cible);
        }
        else
        {
            Debug.Log("déplacement");
            cibleTemp = Vector3.MoveTowards(cibleTemp, cible, vitesseNormale * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, cibleTemp, vitesseNormale * Time.deltaTime);
            tempsDeplacement -= Time.deltaTime;
            if (tempsDeplacement <= 0f)
            {
                deplacementRandom = false;
            }
        }
    }

    void DetectionLumiere()
    {
        transform.LookAt(joueur);
        transform.position = Vector3.MoveTowards(transform.position, joueur.position, vitesseNormale * Time.deltaTime);
    }

    void DetectionJoueur()
    {
        transform.LookAt(joueur);
        transform.position = Vector3.MoveTowards(transform.position, joueur.position, vitesseAttaque * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col) {
        if (col.name.StartsWith("Zone"))
        {
            zones++;
        }
        Debug.Log("zones " + zones);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.name.StartsWith("Zone"))
        {
            zones--;
            if (zones == 0)
            {
                Destroy(gameObject);
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
