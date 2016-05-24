using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(OVRPlayerController))]
public class Joueur : NetworkBehaviour {

    [Header("Commandes Joueur")]
    public OVRInput.Button boutonPause;
    public OVRInput.Button[] boutonsLampe;
    public MenuPause menuPause;
    public Lampe lampe;
    public string eventAmbiance = "musique_ambiance";

    [Header("Options")]
    public Slider stick;
    public Slider musique;
    public Slider voix;

    private bool pause = false;
    private Radar radar;
    private Chat chat;
    private OVRPlayerController controller;
    private int nbAttaques = 0;

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

        if (OVRInput.GetDown(boutonPause))
        {
            CmdPause();
        }

        foreach (OVRInput.Button bouton in boutonsLampe)
        {
            if (OVRInput.GetDown(bouton))
            {
                lampe.Switch();
            }
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
            float tempsBrouille = 3 * Mathf.Log(nbAttaques + 1);
            radar.CmdBrouille(tempsBrouille);
            lampe.SetFreq(tempsBrouille);
        }
    }
}
