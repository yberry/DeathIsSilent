using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(OVRPlayerController))]
public class Joueur : NetworkBehaviour {

    [Header("Commandes Joueur")]
    [Tooltip("Bouton de pause")]
    public OVRInput.RawButton boutonPause;
    public KeyCode boutonPauseClavier;
    [Tooltip("Boutons pour allumer/éteindre la lampe")]
    public OVRInput.RawButton[] boutonsLampe;
    public KeyCode[] boutonsLampeClavier;
    [Tooltip("Bouton pour enlever le toit")]
    public OVRInput.RawButton boutonToit;
    [Tooltip("Possiblilité d'enlever le toit (pour le debug)")]
    public bool switchToit = true;

    [Header("Variables Joueur")]
    [Tooltip("Temps que met le joueur pour mourir")]
    public float tempsMort = 2f;
    [Tooltip("Temps que met le joueur pour ressuciter")]
    public float tempsVie = 1f;
    [Tooltip("Event sonore de la musique")]
    public string eventAmbiance;
    [Tooltip("Event sonore de la mort")]
    public string eventDeath;
    [Tooltip("Event sonore pour la fin du jeu")]
    public string eventEnd;
    [Tooltip("Fréquence de vibration de la manette")]
    [Range(0f, 1f)]
    public float frequenceVibration = 1f;
    [Tooltip("Amplitude de vibration de la manette")]
    [Range(0f, 1f)]
    public float amplitudeVibration = 1f;
    [Tooltip("Temps d'attente avant la vidéo")]
    public float tempsAvantVideo = 11f;
    [Tooltip("Temps d'attente pendant le générique")]
    public float tempsGenerique = 5f;

    [Header("Interface")]
    [Tooltip("Menu de pause")]
    public MenuPause menuPause;
    [Tooltip("Interface d'affichage des items récupérés")]
    public AffichageItems affichageItems;
    [Tooltip("Lampe du joueur")]
    public Lampe lampe;
    [Tooltip("Image de fondu")]
    public RawImage fondu;
    [Tooltip("Vidéo de fin")]
    public MovieTexture video;
    [Tooltip("Credits")]
    public Texture credits;

    [Header("Options")]
    public Slider stick;
    public Slider musique;
    public Slider voix;

    [SyncVar]
    private bool pause = false;
    private Radar radar;
    private Chat chat;
    private OVRPlayerController controller;
    private int nbAttaques = 0;
    private GameObject[] lumieresToits;
    private AudioSource source;
    private bool end = false;

    public static bool isDying = false;

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

            menuPause.Active();
            menuPause.Desactive();
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

        if (OVRInput.GetDown(boutonPause) || Input.GetKeyDown(boutonPauseClavier))
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
        foreach (KeyCode bouton in boutonsLampeClavier)
        {
            if (Input.GetKeyDown(bouton))
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
        StartCoroutine(affichageItems.Affiche(sprite));
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Ennemi")
        {
            return;
        }
        Debug.Log("attaque");
        Destroy(col.gameObject);
        nbAttaques++;
        OVRInput.SetControllerVibration(frequenceVibration, amplitudeVibration);
        isDying = true;
        CmdSwitch(false);

        StartCoroutine(end ? PlayEnd() : Mort());
    }

    IEnumerator Mort()
    {
        AkSoundEngine.PostEvent(eventDeath, gameObject);
        Porte.FermetureGenerale();

        float tempsBrouille = 3 * Mathf.Log(nbAttaques + 1);
        radar.CmdBrouille(tempsMort + tempsVie + tempsBrouille);

        while (fondu.color.a < 1f)
        {
            Color color = fondu.color;
            color.a += Time.deltaTime / tempsMort;
            fondu.color = color;
            radar.CmdColor(color.a, end);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        transform.position = CheckPoint.GetCheckPointPosition();
        transform.rotation = CheckPoint.GetCheckPointRotation();

        isDying = false;

        CmdSwitch(true);
        
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
        AkSoundEngine.PostEvent(eventEnd, gameObject);
        radar.CmdBrouille(tempsAvantVideo);
        radar.CmdPlaySound(eventEnd);
        if (isLocalPlayer)
        {
            yield return new WaitForSeconds(tempsAvantVideo);
            fondu.texture = video;
            fondu.color = Color.white;
            radar.CmdColor(1f, end);
            video.Play();
            video.loop = true;
            source.Play();
            yield return new WaitForSeconds(video.duration);
            fondu.texture = credits;
            video.Stop();
            yield return new WaitForSeconds(tempsGenerique);
            isDying = false;
            FindObjectOfType<MyLobbyManager>().StopHost();
        }
    }
}
