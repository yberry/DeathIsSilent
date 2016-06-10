using UnityEngine;
using UnityEngine.Networking;

public class EnnemiSpawner : NetworkBehaviour {

    [Tooltip("Préfab de l'ennemi")]
    public GameObject ennemiPrefab;
    [Tooltip("Zones délimitantes du bâtiment")]
    public Collider[] colliders;
    [Tooltip("Distance d'apparition de l'ennemi par rapport au joueur")]
    public float distanceApparition = 7f;
    [Tooltip("Position du monstre final")]
    public Transform finalPosition;

    public static EnnemiSpawner instance;

    private Transform joueur;
    private bool[] currentZones;
    private int currentZone = 0;
    private float temps = 0f;
    private float[] probaApparition;
    private Bounds[] bounds;
    private bool monstrePresent = false;
    private Vector3 extents;

    private GameObject ennemi;

    private const float y = 0f;
    private bool stop = false;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        probaApparition = new float[4]
        {
            1f / 40f,
            1f / 25f,
            1f / 20f,
            1f / 15f
        };
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
        currentZones = new bool[colliders.Length];
        bounds = new Bounds[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            bounds[i] = colliders[i].bounds;
        }
        extents = ennemiPrefab.GetComponent<Collider>().bounds.extents;
    }
	
	// Update is called once per frame
	void Update () {
        if (stop)
        {
            return;
        }

        if (monstrePresent)
        {
            bool interieur = false;
            Collider col = ennemi.GetComponent<Collider>();
            foreach (Bounds bound in bounds)
            {
                if (bound.Intersects(col.bounds) || bound.Contains(col.bounds.center))
                {
                    interieur = true;
                    return;
                }
            }

            if (!interieur)
            {
                Destroy(ennemi);
            }

            return;
        }

        if (temps < 1f)
        {
            temps += Time.deltaTime;
        }
        else
        {
            temps = 0f;
            if (Random.Range(0f, 1f) < probaApparition[currentZone])
            {
                Debug.Log("spawn");
                SpawnEnemy(GetSpawnPosition());
            }
        }
	}

    void SpawnEnemy(Vector3 position)
    {
        ennemi = Instantiate(ennemiPrefab, position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(ennemi);
        Vector3 cible = new Vector3(joueur.position.x, y, joueur.position.z);
        ennemi.transform.LookAt(cible);
        monstrePresent = true;
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 min = bounds[currentZone].min;
        Vector3 max = bounds[currentZone].max;

        float angle = Random.Range(0f, 2 * Mathf.PI);
        float x = joueur.position.x + distanceApparition * Mathf.Cos(angle);
        float z = joueur.position.z + distanceApparition * Mathf.Sin(angle);

        while (x < min.x + extents.x || x > max.x - extents.x || z < min.z + extents.z || z > max.z - extents.z)
        {
            angle = Random.Range(0f, 2 * Mathf.PI);
            x = joueur.position.x + distanceApparition * Mathf.Cos(angle);
            z = joueur.position.z + distanceApparition * Mathf.Sin(angle);
        }
        return new Vector3(x, y, z);
    }

    public void EnterZone(int zone)
    {
        currentZones[zone] = true;
        SetCurrentZone();
    }

    public void ExitZone(int zone)
    {
        currentZones[zone] = false;
        SetCurrentZone();
    }

    void SetCurrentZone()
    {
        for (int i = currentZones.Length - 1; i >= 0; i--)
        {
            if (currentZones[i])
            {
                currentZone = i;
                return;
            }
        }
    }

    public void MonstreAbsent()
    {
        monstrePresent = false;
    }

    public void End()
    {
        if (monstrePresent)
        {
            Destroy(ennemi);
        }
        SpawnEnemy(finalPosition.position);
        stop = true;
    }
}
