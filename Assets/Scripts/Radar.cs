using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
[AddComponentMenu("Image Effects/GlitchEffect")]

[RequireComponent(typeof(AudioSource))]
public class Radar : NetworkBehaviour {

	public GameObject[] trackedObjects;
	public GameObject radarPrefab;
	public float switchDistance;
	public Transform helpTransform;
    public GameObject balayagePrefab;
    public float tempsBalayageLimite;
    public int vitesse;
    public GameObject fleche;
    [SyncVar]
    public bool activate = true;
    public AudioClip sonEnnemi;

    public OVRInput.Button brouilleButton;
    public GameObject[] disparu;

    private List<GameObject> radarObjects;
    private float tempsBalayage;
    private GameObject balais;
    private GameObject player;
    private NoiseAndScratches script;
    private AudioSource source;

    [SyncVar]
    bool vueSubjective = true;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        ResetPosition();
		createRadarObjects();
        balais = Instantiate(balayagePrefab,transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
        balais.transform.parent = transform;
        tempsBalayage = 0f;
        source = GetComponent<AudioSource>();

        script = GameObject.Find("Radar Camera").GetComponent<NoiseAndScratches>();
        script.activate = false;
    }

    void Update() {
        if (!isLocalPlayer)
        {
            Camera cam = transform.Find("Radar Camera").GetComponent<Camera>();
            cam.rect = new Rect(2f / 3f, 0f, 1f / 3f, 1f / 3f);
            cam.GetComponent<AudioListener>().enabled = false;
            source.enabled = false;
        }
        else if (balais.GetComponent<Balayage>().detect)
        {
            source.PlayOneShot(sonEnnemi);
            balais.GetComponent<Balayage>().detect = false;
        }

        if (!activate)
        {
            return;
        }
        for (int i = 0; i < radarObjects.Count; i++) {
            if (Vector3.Distance(radarObjects[i].transform.position, transform.position) > switchDistance) {
                helpTransform.LookAt(radarObjects[i].transform);
                //borderObjects[i].transform.position = transform.position + switchDistance * helpTransform.forward;
            }
        }
        tempsBalayage += Time.deltaTime;
        if (tempsBalayage < tempsBalayageLimite)
        {
            balais.transform.localScale = balais.transform.localScale * (vitesse + 1) / vitesse;
        } else {
            Destroy(balais);
            tempsBalayage = 0f;
            balais = Instantiate(balayagePrefab,transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
            balais.transform.parent = transform;
        }

        if (OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.W))
        {
            vueSubjective = !vueSubjective;
        }

        if (isServer)
        {
            RpcSetPosition();
        }
	}

    [ClientRpc]
    public void RpcBrouille()
    {
        script.activate = !script.activate;
        activate = !activate;

        foreach (GameObject o in disparu)
        {
            if (o.GetComponent<MeshRenderer>() == null)
            {
                foreach (Transform child in o.transform)
                {
                    child.GetComponent<MeshRenderer>().enabled = activate;
                }
            }
            else {
                o.GetComponent<MeshRenderer>().enabled = activate;
            }
        }
    }

    [ClientRpc]
    void RpcSetPosition()
    {
        ResetPosition();

        if (vueSubjective)
        {
            transform.LookAt(transform.position + player.transform.forward);
            fleche.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            fleche.transform.LookAt(fleche.transform.position + player.transform.forward);
        }
    }

	void createRadarObjects (){
		radarObjects = new List <GameObject>();
		foreach(GameObject o in trackedObjects){
            Vector3 position = o.transform.position;
			GameObject k = Instantiate (radarPrefab, position, Quaternion.identity) as GameObject;
			radarObjects.Add(k);
		}
	}

    void ResetPosition()
    {
        Vector3 position = player.transform.position;
        position.y = transform.position.y;
        transform.position = position;
    }
}
