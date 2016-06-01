using UnityEngine;
using UnityEngine.Networking;

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
    [Tooltip("Lampes représentant le champ de vision de l'ennemi")]
    public Oeil[] yeux;
    [Tooltip("Event sonore pour le monstre")]
    public string eventMonstre;
    [Tooltip("Event sonore pour la destruction du monstre")]
    public string eventDestroy;
    [Tooltip("Event sonore pour l'attaque du monstre")]
    public string eventAttack;
    [Tooltip("Event sonore après l'attaque du monstre")]
    public string eventSafe;
    [Tooltip("Animateur du monstre")]
    public Animator animator;

    private bool deplacementRandom = false;
    [SyncVar]
    private Vector3 cible;
    [SyncVar]
    private Vector3 cibleTemp;
    private float tempsDeplacementRandom;

    private bool detecteLumiere = false;
    private bool detecteJoueur = false;
    private Transform joueur;
    private Collider[] lumieres;
    private float rangeLight;
    private RaycastHit hit1;
    private RaycastHit hit2;
    private int rand = 0;

    private Collider col;
    private bool attaque = false;

	// Use this for initialization
	void Start () {
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
        Lampe lampe = FindObjectOfType<Lampe>();
        lumieres = lampe.cones;
        rangeLight = lampe.GetComponent<Light>().range;
        if (isServer)
        {
            AkSoundEngine.PostEvent(eventMonstre, gameObject);
            AkSoundEngine.SetState("Monster_States", "idle");
        }
        col = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("marche_cycle"))
        {
            AkSoundEngine.SetState("Monster_States", "move");
        }

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

        foreach (Oeil oeil in yeux)
        {
            Vector3 origin, dir;
            if (rand % 3 == 0)
            {
                origin = oeil.transform.position;
                dir = oeil.GetRandomDir();
            }
            else if (rand % 3 == 1)
            {
                origin = oeil.transform.position;
                dir = oeil.transform.forward;
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
                if (hit1.collider == lumieres[0] || hit1.collider == lumieres[1] || col.bounds.Intersects(lumieres[0].bounds) || col.bounds.Intersects(lumieres[1].bounds))
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
                    if (!detecteJoueur)
                    {
                        detecteJoueur = true;
                        AkSoundEngine.SetState("Monster_States", "attack");
                        AkSoundEngine.PostEvent(eventAttack, gameObject);
                    }
                    Debug.Log("détection joueur");
                }
            }
        }

        float dist = Vector3.Distance(transform.position, joueur.position);
        Debug.Log("rtpc : " + dist * 5f);
        AkSoundEngine.SetRTPCValue("Monster_Distance_Reverb", dist * 5f);

        if (attaque && dist > rangeLight)
        {
            Destroy(gameObject);
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
        cible = new Vector3(joueur.position.x, transform.position.y, joueur.position.z);
        cibleTemp = Vector3.MoveTowards(cibleTemp, cible, vitesseNormale * Time.deltaTime);
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
        cible = new Vector3(joueur.position.x, transform.position.y, joueur.position.z);
        cibleTemp = Vector3.MoveTowards(cibleTemp, cible, vitesseAttaque * Time.deltaTime);
        transform.LookAt(cibleTemp);
        transform.position = Vector3.MoveTowards(transform.position, cibleTemp, vitesseAttaque * Time.deltaTime);
        if (Vector3.Distance(transform.position, joueur.position) < 5f) {
            AkSoundEngine.SetState("Monster_States", "attacking");
            animator.SetTrigger("attaque");
            attaque = true;
        }
    }

    void OnDestroy()
    {
        Debug.Log("destruction");
        AkSoundEngine.SetState("Monster_States", "None");
        //AkSoundEngine.PostEvent(eventDestroy, gameObject);
        AkSoundEngine.PostEvent(eventSafe, gameObject);
        if (isServer)
        {
            EnnemiSpawner.instance.MonstreAbsent();
        }
    }
}
