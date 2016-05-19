using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joueur : NetworkBehaviour {

    public OVRInput.Button boutonPause;
    public MenuPause menuPause;
    public Lampe lampe;
    public AudioSource ambiance;

    private bool pause = false;
    private Radar radar;
    private Chat chat;

    void Start()
    {
        if (!isLocalPlayer)
        {
            ambiance.enabled = false;
        }
        else
        {
            chat = FindObjectOfType<Chat>();
        }
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

        if (OVRInput.GetDown(boutonPause))
        {
            RpcPause();
        }

        if (OVRInput.GetDown(lampe.switcher))
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
            //radar.CmdBrouille();
            //chat.SetTransmission(false);
            lampe.SetFreq(5f);
        }
    }
}
