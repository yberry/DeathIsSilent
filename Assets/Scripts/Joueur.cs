using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(OVRPlayerController))]
public class Joueur : NetworkBehaviour {

    [Header("Commandes Joueur")]
    public OVRInput.RawButton boutonPause;
    public OVRInput.RawButton[] boutonsLampe;
    public OVRInput.RawButton boutonToit;
    public MenuPause menuPause;
    public AffichageItems affichageItems;
    public Lampe lampe;
    public RawImage fondu;
    public MovieTexture video;
    public float tempsMort = 2f;
    public float tempsVie = 1f;
    public string eventAmbiance;
    public string eventDeath;
    [Range(0f, 1f)]
    public float frequenceVibration = 1f;
    [Range(0f, 1f)]
    public float amplitudeVibration = 1f;
    public bool switchToit = true;

    [Header("Options")]
    public Slider stick;
    public Slider musique;
    public Slider voix;

    [SyncVar]
    private bool pause = false;
    private bool isDying = false;
    private Radar radar;
    private Chat chat;
    private OVRPlayerController controller;
    private int nbAttaques = 0;
    private GameObject[] lumieresToits;
    private AudioSource source;
    private bool end = false;

    void Start()
    {
        if (isLocalPlayer)
        {
            AkSoundEngine.PostEvent(eventAmbiance, gameObject);
            chat = FindObjectOfType<Chat>();
            stick.value = PlayerPrefs.GetFloat("stick");
            musique.value = PlayerPrefs.GetFloat("musique");
            voix.value = PlayerPrefs.GetFloat("voix");
            source = fondu.GetComponent<AudioSource>();
        }
        controller = GetComponent<OVRPlayerController>();
        lumieresToits = GameObject.FindGameObjectsWithTag("Toit");
        foreach (GameObject obj in lumieresToits)
        {
            if (obj.GetComponent<Light>() != null)
            {
                obj.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (radar == null)
        {
            radar = FindObjectOfType<Radar>();
        }

        controller.RotationAmount = PlayerPrefs.GetFloat("stick");

        if (!isLocalPlayer)
        {
            return;
        }

        chat.SetVolume(PlayerPrefs.GetFloat("voix"));

        if (isDying)
        {
            return;
        }

        if (switchToit && OVRInput.GetDown(boutonToit))
        {
            CmdToit();
        }

        if (OVRInput.GetDown(boutonPause))
        {
            CmdPause();
        }

        foreach (OVRInput.RawButton bouton in boutonsLampe)
        {
            if (OVRInput.GetDown(bouton))
            {
                CmdSwitch(!lampe.lampe.enabled);
            }
        }
    }

    [Command]
    void CmdSwitch(bool en)
    {
        RpcSwitch(en);
    }

    [ClientRpc]
    void RpcSwitch(bool en)
    {
        lampe.Switch(en);
    }

    [Command]
    void CmdToit()
    {
        RpcToit();
    }

    [ClientRpc]
    void RpcToit()
    {
        foreach (GameObject obj in lumieresToits)
        {
            obj.SetActive(!obj.activeInHierarchy);
        }
    }

    [Command]
    public void CmdPause()
    {
        RpcPause();
    }

    [ClientRpc]
    void RpcPause()
    {
        if (pause)
        {
            pause = false;
            Time.timeScale = 1f;
            menuPause.Desactive();
        }
        else
        {
            pause = true;
            Time.timeScale = 0f;
            menuPause.Active();
        }
    }

    public void Affiche(Sprite sprite)
    {
        affichageItems.Affiche(sprite);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Ennemi")
        {
            return;
        }
        if (!end)
        {
            Debug.Log("attaque");
            Destroy(col.gameObject);
            nbAttaques++;
            OVRInput.SetControllerVibration(frequenceVibration, amplitudeVibration);
            isDying = true;
            CmdSwitch(false);
            StartCoroutine(Mort());
        }
        else
        {
            StartCoroutine(PlayEnd());
        }
    }

    IEnumerator Mort()
    {
        Porte.FermetureGenerale();
        AkSoundEngine.PostEvent(eventDeath, gameObject);

        while (fondu.color.a < 1f)
        {
            Color color = fondu.color;
            color.a += Time.deltaTime / tempsMort;
            fondu.color = color;
            radar.CmdColor(color.a, end);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Transform checkpoint = CheckPoint.GetCheckPoint();
        transform.position = checkpoint.position;
        transform.rotation = checkpoint.rotation;

        isDying = false;

        CmdSwitch(true);
        float tempsBrouille = 3 * Mathf.Log(nbAttaques + 1);
        radar.CmdBrouille(tempsBrouille);
        lampe.SetFreq(tempsBrouille);
        while (fondu.color.a > 0f)
        {
            Color color = fondu.color;
            color.a -= Time.deltaTime / tempsVie;
            fondu.color = color;
            radar.CmdColor(color.a, end);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void End()
    {
        end = true;
    }

    IEnumerator PlayEnd()
    {
        if (isLocalPlayer)
        {
            fondu.texture = video;
            fondu.color = Color.white;
            radar.CmdColor(1f, end);
            video.Play();
            source.Play();
            yield return new WaitForSeconds(video.duration);
            Debug.Log("quit");
            menuPause.Quit();
        }
    }
}
