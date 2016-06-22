using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;
[AddComponentMenu("Image Effects/GlitchEffect")]

public class Radar : NetworkBehaviour {

    [Header("Commandes Radar")]
    [Tooltip("Touches permettant de passer en vue subjective")]
    public KeyCode[] boutonsVueSub;

    [Header("Variables Radar")]
    [Tooltip("Limite de taille de l'onde")]
    public float limitScale = 800f;
    [Tooltip("Vitesse de l'onde")]
    public int vitesse = 45;
    [Tooltip("Event sonore d'émission de l'onde")]
    public string eventEmission;
    [Tooltip("Event sonore de détection de l'ennemi")]
    public string eventDetection;
    [Tooltip("Event sonore pour couper la musique du menu")]
    public string eventStopMenu;

    [Header("Interface")]
    [Tooltip("Préfab de l'onde de balayage")]
    public GameObject balayagePrefab;
    [Tooltip("Flèche représentant le joueur sur le radar")]
    public Transform fleche;
    [Tooltip("Fondu enchaîné de la mort")]
    public RawImage fondu;
    [Tooltip("Vidéo de fin")]
    public MovieTexture video;
    [Tooltip("Credits")]
    public Texture credits;
    [Tooltip("Lumière représentant la lampe")]
    public Light lumiere;
    [Tooltip("Texte d'information sur la coupure de communication")]
    public Text text;
    [Tooltip("Activer le mini-radar sur l'écran du joueur à l'oculus")]
    public bool miniCamera = true;

    private bool brouille = false;
    private float tempsBrouilleMax = 10f;
    private float tempsBrouille;
    private GameObject balais;
    private GameObject player;
    private NoiseAndScratches script;
    private Chat chat;
    private AudioSource source;

    [SyncVar]
    private bool vueSubjective = false;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        ResetPosition();
        StartWave();

        Camera cam = transform.Find("Radar Camera").GetComponent<Camera>();
        if (cam.GetComponent<AkAudioListener>() == null)
        {
            AkAudioListener listener = cam.gameObject.AddComponent<AkAudioListener>();
            listener.listenerId = 1;
        }

        if (isLocalPlayer)
        {
            AkSoundEngine.PostEvent(eventStopMenu, gameObject);
            chat = FindObjectOfType<Chat>();
        }
        else
        {
            if (PlayerPrefs.GetInt("oculus") == 0)
            {
                miniCamera = false;
            }
            cam.rect = new Rect(miniCamera ? 2f / 3f : 1f, 0f, 1f / 3f, 1f / 3f);
            cam.GetComponent<AudioListener>().enabled = false;
            cam.GetComponent<AkAudioListener>().enabled = false;
        }

        script = cam.GetComponent<NoiseAndScratches>();

        source = fondu.GetComponent<AudioSource>();
    }

    void Update() {

        if (isLocalPlayer)
        {
            if (Balayage.detect)
            {
                AkSoundEngine.SetRTPCValue("Radar_Distance", balais.transform.localScale.z * 100f / limitScale);
                AkSoundEngine.PostEvent(eventDetection, Balayage.detected);
                Balayage.detect = false;
            }
            foreach (KeyCode key in boutonsVueSub)
            {
                if (Input.GetKeyDown(key))
                {
                    CmdVue();
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
            if (balais.transform.localScale.z < limitScale)
            {
                balais.transform.localScale = balais.transform.localScale * (vitesse + 1) / vitesse;
            }
            else
            {
                Destroy(balais);
                StartWave();
            }
        }

        if (tempsBrouille < 0f)
        {
            tempsBrouille = 0f;
            brouille = false;
            SetTransmission(true);
            SetNoise(0f);

            StartWave();
        }

        if (isServer)
        {
            RpcSetPosition();
        }
	}

    [Command]
    void CmdVue()
    {
        vueSubjective = !vueSubjective;
    }

    void StartWave()
    {
        balais = Instantiate(balayagePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
        balais.transform.parent = transform;
        if (isLocalPlayer)
        {
            AkSoundEngine.PostEvent(eventEmission, gameObject);
        }
    }

    [Command]
    public void CmdColor(float alpha, bool end)
    {
        RpcColor(alpha, end);
    }

    [ClientRpc]
    void RpcColor(float alpha, bool end)
    {
        Color color = end ? Color.white : Color.black;
        color.a = alpha;
        fondu.color = color;
        if (end)
        {
            StartCoroutine(PlayEnd());
        }
    }

    IEnumerator PlayEnd()
    {
        fondu.texture = video;
        video.Play();
        video.loop = true;
        if (isLocalPlayer)
        {
            source.Play();
        }
        yield return new WaitForSeconds(video.duration);
        video.Stop();
        fondu.texture = credits;
    }

    [Command]
    public void CmdPlaySound(string ev)
    {
        RpcPlaySound(ev);
    }

    [ClientRpc]
    void RpcPlaySound(string ev)
    {
        fleche.GetComponent<Renderer>().enabled = false;
        if (isLocalPlayer)
        {
            AkSoundEngine.PostEvent(ev, gameObject);
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
        brouille = true;
        SetTransmission(false);
        Destroy(balais);
        SetNoise(5f);
    }

    [Command]
    public void CmdCoupe(float temps)
    {
        RpcCoupe(temps);
    }

    [ClientRpc]
    void RpcCoupe(float temps)
    {
        StartCoroutine(Coupe(temps));
    }

    IEnumerator Coupe(float temps)
    {
        SetTransmission(false);
        yield return new WaitForSeconds(temps);
        SetTransmission(true);
    }

    void SetTransmission(bool tr)
    {
        text.gameObject.SetActive(!tr);
        if (isLocalPlayer)
        {
            chat.SetTransmission(tr);
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

        Quaternion zero = Quaternion.Euler(0f, 0f, 0f);

        if (vueSubjective)
        {
            transform.LookAt(transform.position + player.transform.forward);
            fleche.localRotation = zero;
        }
        else
        {
            transform.rotation = zero;
            fleche.LookAt(fleche.transform.position + player.transform.forward);
        }
    }

    void ResetPosition()
    {
        Vector3 position = player.transform.position;
        position.y = transform.position.y;
        transform.position = position;
    }
}
