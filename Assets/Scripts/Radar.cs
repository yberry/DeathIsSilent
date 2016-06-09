using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;
[AddComponentMenu("Image Effects/GlitchEffect")]

public class Radar : NetworkBehaviour {

    [Tooltip("Préfab de l'onde de balayage")]
    public GameObject balayagePrefab;
    [Tooltip("Limite de taille de l'onde")]
    public float limitScale = 800f;
    [Tooltip("Vitesse de l'onde")]
    public int vitesse;
    [Tooltip("Flèche représentant le joueur sur le radar")]
    public Transform fleche;
    [Tooltip("Fondu enchaîné de la mort")]
    public Image fondu;
    [Tooltip("Lumière représentant la lampe")]
    public Light lumiere;
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
            AkSoundEngine.PostEvent(eventEmissionStart, gameObject);
        }
    }

    [Command]
    public void CmdSwitch()
    {
        RpcSwitch();
    }

    [ClientRpc]
    void RpcSwitch()
    {
        lumiere.enabled = !lumiere.enabled;
    }

    [Command]
    public void CmdColor(Color color)
    {
        RpcColor(color);
    }

    [ClientRpc]
    void RpcColor(Color color)
    {
        fondu.color = color;
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
