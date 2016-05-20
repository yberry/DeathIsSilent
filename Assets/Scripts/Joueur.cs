using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(OVRPlayerController))]
public class Joueur : NetworkBehaviour {

    [Header("Commandes Joueur")]
    public OVRInput.Button boutonPause;
    public OVRInput.Button boutonLampe;
    public MenuPause menuPause;
    public Lampe lampe;
    public AudioSource ambiance;

    [Header("Options")]
    public Slider stick;
    public Slider musique;
    public Slider voix;

    private bool pause = false;
    private Radar radar;
    private Chat chat;
    private OVRPlayerController controller;

    void Start()
    {
        if (!isLocalPlayer)
        {
            ambiance.enabled = false;
        }
        else
        {
            chat = FindObjectOfType<Chat>();
            stick.value = PlayerPrefs.GetFloat("stick");
            musique.value = PlayerPrefs.GetFloat("musique");
            voix.value = PlayerPrefs.GetFloat("voix");
        }
        controller = GetComponent<OVRPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (radar == null)
        {
            radar = FindObjectOfType<Radar>();
        }
        
        if (!isLocalPlayer)
        {
            return;
        }

        controller.RotationAmount = PlayerPrefs.GetFloat("stick");
        ambiance.volume = PlayerPrefs.GetFloat("musique");
        chat.SetVolume(PlayerPrefs.GetFloat("voix"));

        if (OVRInput.GetDown(boutonPause))
        {
            RpcPause();
        }

        if (OVRInput.GetDown(boutonLampe))
        {
            lampe.Switch();
        }
    }

    [ClientRpc]
    public void RpcPause()
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
            radar.CmdBrouille();
            //chat.SetTransmission(false);
            lampe.SetFreq(5f);
        }
    }
}
