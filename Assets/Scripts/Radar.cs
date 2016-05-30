using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
[AddComponentMenu("Image Effects/GlitchEffect")]

public class Radar : NetworkBehaviour {

    [Tooltip("Préfab de l'onde de balayage")]
    public GameObject balayagePrefab;
    public float limitScale = 800f;
    public int vitesse;
    [Tooltip("Flèche représentant le joueur sur le radar")]
    public GameObject fleche;
    public string eventEmissionStart;
    public string eventEmissionStop;
    public string eventDetection;
    [Tooltip("Texte d'information sur la coupure de communication")]
    public Text text;
    [Tooltip("Touches permettant de passer en vue subjective")]
    public KeyCode[] boutonsVueSub;
    [Tooltip("Activer le mini-radar sur l'écran du joueur à l'oculus")]
    public bool miniCamera = true;

    private bool brouille = false;
    private float tempsBrouilleMax = 10f;
    private float tempsBrouille;
    private float tempsBalayage;
    private GameObject balais;
    private GameObject player;
    private NoiseAndScratches script;
    private Chat chat;

    [SyncVar]
    private bool vueSubjective = true;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        ResetPosition();
        StartWave();
        tempsBalayage = 0f;
        if (isLocalPlayer)
        {
            chat = FindObjectOfType<Chat>();
        }
        else
        {
            Camera cam = transform.Find("Radar Camera").GetComponent<Camera>();
            if (miniCamera)
            {
                cam.rect = new Rect(2f / 3f, 0f, 1f / 3f, 1f / 3f);
                cam.GetComponent<AudioListener>().enabled = false;
            }
            else
            {
                cam.gameObject.SetActive(false);
            }
        }

        script = transform.Find("Radar Camera").GetComponent<NoiseAndScratches>();
    }

    void Update() {

        if (isLocalPlayer)
        {
            if (Balayage.detect)
            {
                // Son + ou - aigu en fonction de tempsBalayage
                AkSoundEngine.PostEvent(eventDetection, Balayage.detected);
                Balayage.detect = false;
            }
            foreach (KeyCode key in boutonsVueSub)
            {
                Debug.Log(key);
                if (Input.anyKeyDown)
                {
                    Debug.Log("espace");
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
            if (balais.transform.localScale.z < limitScale)
            {
                balais.transform.localScale = balais.transform.localScale * (vitesse + 1) / vitesse;
            }
            else
            {
                Destroy(balais);
                tempsBalayage = 0f;
                StartWave();
            }
        }

        if (tempsBrouille < 0f)
        {
            tempsBrouille = 0f;
            SetTransmission(true);
            SetNoise(0f);

            StartWave();
        }

        if (isServer)
        {
            RpcSetPosition();
        }
	}

    void StartWave()
    {
        balais = Instantiate(balayagePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
        balais.transform.parent = transform;
        if (isLocalPlayer)
        {
            AkSoundEngine.PostEvent(eventEmissionStart, gameObject);
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
