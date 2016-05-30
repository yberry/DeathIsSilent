using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(OVRPlayerController))]
public class Joueur : NetworkBehaviour {

    [Header("Commandes Joueur")]
    public OVRInput.RawButton boutonPause;
    public OVRInput.RawButton[] boutonsLampe;
    public OVRInput.RawButton boutonToit;
    public MenuPause menuPause;
    public Lampe lampe;
    public string eventAmbiance;
    [Range(0f, 1f)]
    public float frequenceVibration = 1f;
    [Range(0f, 1f)]
    public float amplitudeVibration = 1f;
    public bool switchToit = true;

    [Header("Options")]
    public Slider stick;
    public Slider musique;
    public Slider voix;

    private bool pause = false;
    private Radar radar;
    private Chat chat;
    private OVRPlayerController controller;
    private int nbAttaques = 0;
    private GameObject[] lumieresToits;

    void Start()
    {
        if (isLocalPlayer)
        {
            AkSoundEngine.PostEvent(eventAmbiance, gameObject);
            chat = FindObjectOfType<Chat>();
            stick.value = PlayerPrefs.GetFloat("stick");
            musique.value = PlayerPrefs.GetFloat("musique");
            voix.value = PlayerPrefs.GetFloat("voix");
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
        
        //ambiance.volume = PlayerPrefs.GetFloat("musique");
        chat.SetVolume(PlayerPrefs.GetFloat("voix"));

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
                lampe.Switch();
            }
        }
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

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ennemi")
        {
            Debug.Log("attaque");
            Destroy(col.gameObject);
            nbAttaques++;
            OVRInput.SetControllerVibration(frequenceVibration, amplitudeVibration);
            float tempsBrouille = 3 * Mathf.Log(nbAttaques + 1);
            radar.CmdBrouille(tempsBrouille);
            lampe.SetFreq(tempsBrouille);
        }
    }
}
