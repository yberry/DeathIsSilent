using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
[AddComponentMenu("Image Effects/GlitchEffect")]

public class Radar : NetworkBehaviour {

	public GameObject radarPrefab;
    public GameObject balayagePrefab;
    public float tempsBalayageLimite;
    public int vitesse;
    public GameObject fleche;
    public string eventEmissionStart = "radar_emit_start";
    public string eventEmissionStop = "radar_emit_stop";
    public string eventDetection = "radar_detect";
    public Text text;
    public KeyCode[] boutonsVueSub;

    private bool brouille = false;
    private float tempsBrouilleMax = 10f;
    private float tempsBrouille;
    private float tempsBalayage;
    private GameObject balais;
    private GameObject player;
    private NoiseAndScratches script;
    private Chat chat;

    [SyncVar]
    bool vueSubjective = true;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        ResetPosition();
        balais = Instantiate(balayagePrefab,transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
        balais.transform.parent = transform;
        tempsBalayage = 0f;
        if (isLocalPlayer)
        {
            AkSoundEngine.PostEvent(eventEmissionStart, gameObject);
            chat = FindObjectOfType<Chat>();
        }
        else
        {
            Camera cam = transform.Find("Radar Camera").GetComponent<Camera>();
            cam.rect = new Rect(2f / 3f, 0f, 1f / 3f, 1f / 3f);
            cam.GetComponent<AudioListener>().enabled = false;
        }

        script = transform.Find("Radar Camera").GetComponent<NoiseAndScratches>();
    }

    void Update() {

        if (isLocalPlayer)
        {
            if (Balayage.detect)
            {
                AkSoundEngine.PostEvent(eventDetection, Balayage.detected);
                Balayage.detect = false;
            }
            foreach (KeyCode key in boutonsVueSub)
            {
                if (Input.GetKeyDown(key))
                {
                    vueSubjective = !vueSubjective;
                }
            }
        }

        if (brouille)
        {
            tempsBrouille -= Time.deltaTime;
            SetNoise(5f * tempsBrouille / tempsBrouilleMax);
        }
        else
        {
            tempsBalayage += Time.deltaTime;
            if (tempsBalayage < tempsBalayageLimite)
            {
                balais.transform.localScale = balais.transform.localScale * (vitesse + 1) / vitesse;
            }
            else {
                Destroy(balais);
                tempsBalayage = 0f;
                balais = Instantiate(balayagePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                balais.transform.parent = transform;
            }
        }

        if (tempsBrouille < 0f)
        {
            tempsBrouille = 0f;
            SetTransmission(true);
            SetNoise(0f);

            balais = Instantiate(balayagePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
            balais.transform.parent = transform;
        }

        if (isServer)
        {
            RpcSetPosition();
        }
	}

    [Command]
    public void CmdBrouille(float time)
    {
        RpcBrouille(time);
    }

    [ClientRpc]
    void RpcBrouille(float time)
    {
        tempsBrouilleMax = time;
        tempsBrouille = time;
        SetTransmission(false);
        Destroy(balais);
        tempsBalayage = 0f;
        SetNoise(5f);
    }

    void SetTransmission(bool tr)
    {
        brouille = !tr;
        text.gameObject.SetActive(!tr);
        if (isLocalPlayer)
        {
            chat.SetTransmission(tr);
            AkSoundEngine.PostEvent(tr ? eventEmissionStart : eventEmissionStop, gameObject);
        }
    }

    void SetNoise(float val)
    {
        script.grainIntensityMin = val;
        script.grainIntensityMax = val;
        script.scratchIntensityMin = val;
        script.scratchIntensityMax = val;
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

    void ResetPosition()
    {
        Vector3 position = player.transform.position;
        position.y = transform.position.y;
        transform.position = position;
    }
}
