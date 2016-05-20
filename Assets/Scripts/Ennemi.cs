using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class Ennemi : NetworkBehaviour {

    public Camera vision;
    [Tooltip("Temps de déplacement vers le joueur après avoir détecté la lumière")]
    public float tempsDeplacementLumiere = 3f;
    [Tooltip("Vitesse normale de l'ennemi")]
    public float vitesseNormale = 1f;
    [Tooltip("Vitesse d'attaque de l'ennemi")]
    public float vitesseAttaque = 2f;
    [Tooltip("Layers visibles par l'ennemi")]
    public LayerMask mask;

    private AudioSource source;

    private bool deplacementRandom = false;
    private Vector3 cible;
    private Vector3 cibleTemp;
    private float tempsDeplacementRandom;
    public MeshFilter[] yeux;

    private bool detecteLumiere = false;
    private bool detecteJoueur = false;
    private Transform joueur;
    private Collider[] lumieres;
    private RaycastHit hit1;
    private RaycastHit hit2;
    private int rand = 0;

	// Use this for initialization
	void Start () {
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
        lumieres = FindObjectOfType<Lampe>().cones;
        source = GetComponent<AudioSource>();
        if (!isServer)
        {
            source.enabled = false;
        }
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

        foreach (MeshFilter oeil in yeux)
        {
            Vector3 origin, dir;
            if (rand % 3 == 0)
            {
                origin = oeil.transform.parent.position;
                dir = transform.TransformPoint(oeil.mesh.vertices[Random.Range(0, oeil.mesh.vertexCount)]) - origin;
            }
            else if (rand % 3 == 1)
            {
                origin = oeil.transform.parent.position;
                dir = oeil.transform.parent.forward;
            }
            else
            {
                origin = transform.position;
                dir = transform.forward + Vector3.up / 2;
            }

            rand++;
            Debug.DrawRay(origin, 5 * dir, Color.green);
            if (Physics.Raycast(origin, dir, out hit1, Mathf.Infinity, mask))
            {
                //Debug.Log(hit1.collider);
                if (hit1.collider == lumieres[0] || hit1.collider == lumieres[1])
                {
                    if (Physics.Raycast(hit1.point, joueur.position - hit1.point, out hit2, Mathf.Infinity, mask))
                    {
                        if (hit2.transform == joueur)
                        {
                            detecteLumiere = true;
                            tempsDeplacementLumiere = 3f;
                            deplacementRandom = false;
                            Debug.Log("détection lumière");
                        }
                    }
                }

                if (detecteLumiere && hit1.transform == joueur)
                {
                    detecteJoueur = true;
                    Debug.Log("détection joueur");
                }
            }
        }
	}

    void MouvementRandom()
    {
        if (!deplacementRandom)
        {
            cibleTemp = transform.position + 5 * transform.forward;
            float angle = Mathf.Deg2Rad * Random.Range(-120f, 120f);
            cible = cibleTemp + 5 * new Vector3(Mathf.Cos(angle) * transform.forward.x - Mathf.Sin(angle) * transform.forward.z, 0f, Mathf.Sin(angle) * transform.forward.x + Mathf.Cos(angle) * transform.forward.z);
            tempsDeplacementRandom = Random.Range(2f, 6f);
            deplacementRandom = true;
        }
        else
        {
            cibleTemp = Vector3.MoveTowards(cibleTemp, cible, vitesseNormale * Time.deltaTime);
            transform.LookAt(cibleTemp);
            transform.position = Vector3.MoveTowards(transform.position, cibleTemp, vitesseNormale * Time.deltaTime);
            tempsDeplacementRandom -= Time.deltaTime;
            if (tempsDeplacementRandom <= 0f)
            {
                deplacementRandom = false;
            }
        }
    }

    void DetectionLumiere()
    {
        cibleTemp = Vector3.MoveTowards(cibleTemp, joueur.position, vitesseNormale * Time.deltaTime);
        transform.LookAt(cibleTemp);
        transform.position = Vector3.MoveTowards(transform.position, cibleTemp, vitesseNormale * Time.deltaTime);
        tempsDeplacementLumiere -= Time.deltaTime;
        if (tempsDeplacementLumiere <= 0f)
        {
            detecteLumiere = false;
            tempsDeplacementLumiere = 3f;
        }
    }

    void DetectionJoueur()
    {
        transform.LookAt(joueur);
        transform.position = Vector3.MoveTowards(transform.position, joueur.position, vitesseAttaque * Time.deltaTime);
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
