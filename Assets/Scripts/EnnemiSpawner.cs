using UnityEngine;
using UnityEngine.Networking;

public class EnnemiSpawner : NetworkBehaviour {

    [Tooltip("Préfab de l'ennemi")]
    public GameObject ennemiPrefab;
    [Tooltip("Zones délimitantes du bâtiment")]
    public Collider[] colliders;
    [Tooltip("Distance minimum d'apparition de l'ennemi par rapport au joueur")]
    public float distanceApparition = 5f;

    public static EnnemiSpawner instance;

    private Transform joueur;
    private int currentZone = 0;
    private float temps = 0f;
    private float[] probaApparition;
    private Vector3[] mins;
    private Vector3[] maxs;
    private bool monstrePresent = false;
    private Vector3 extents;

    private const float y = 1f;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        probaApparition = new float[4]
        {
            1f / 50f,
            1f / 25f,
            1f / 20f,
            1f / 15f
        };
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
        mins = new Vector3[colliders.Length];
        maxs = new Vector3[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            mins[i] = colliders[i].bounds.min;
            maxs[i] = colliders[i].bounds.max;
        }
        extents = ennemiPrefab.GetComponent<Collider>().bounds.extents;
    }
	
	// Update is called once per frame
	void Update () {
        if (monstrePresent)
        {
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
                SpawnEnemy();
            }
        }
	}

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(ennemiPrefab, GetSpawnPosition(), Quaternion.identity) as GameObject;
        NetworkServer.Spawn(enemy);
        monstrePresent = true;
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 min = mins[currentZone];
        Vector3 max = maxs[currentZone];

        float x = Random.Range(min.x + extents.x, max.x - extents.x);
        float z = Random.Range(min.z + extents.z, max.z - extents.z);

        Vector3 pos = new Vector3(x, y, z);

        while (Vector3.Distance(joueur.position, pos) < distanceApparition)
        {
            pos.x = Random.Range(min.x + extents.x, max.x - extents.x);
            pos.z = Random.Range(min.z + extents.z, max.z - extents.z);
        }
        return pos;
    }

    public void SetCurrentZone(int zone)
    {
        currentZone = zone;
    }

    public void MonstreAbsent()
    {
        monstrePresent = false;
    }
}
