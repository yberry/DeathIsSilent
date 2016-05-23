﻿using UnityEngine;
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
    private bool[] currentZones;
    private int currentZone = 0;
    private float temps = 0f;
    private float[] probaApparition;
    private Bounds[] bounds;
    private bool monstrePresent = false;
    private Vector3 extents;

    private GameObject ennemi;

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
        if (monstrePresent)
        {
            bool interieur = false;
            Collider col = ennemi.GetComponent<Collider>();
            foreach (Bounds bound in bounds)
            {
                if (bound.Intersects(col.bounds) || bound.Contains(col.bounds.center))
                {
                    interieur = true;
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
                SpawnEnemy();
            }
        }
	}

    void SpawnEnemy()
    {
        ennemi = Instantiate(ennemiPrefab, GetSpawnPosition(), Quaternion.identity) as GameObject;
        NetworkServer.Spawn(ennemi);
        monstrePresent = true;
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 min = bounds[currentZone].min;
        Vector3 max = bounds[currentZone].max;

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
}
