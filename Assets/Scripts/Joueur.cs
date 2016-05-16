using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class Joueur : NetworkBehaviour {

    public OVRInput.Button boutonPause;
    public Lampe lampe;

    private Radar radar;

    // Update is called once per frame
    void Update()
    {
        if (radar == null)
        {
            radar = FindObjectOfType<Radar>();
        }

        if (OVRInput.GetDown(boutonPause) && isLocalPlayer)
        {
            RpcPause();
        }
    }

    [ClientRpc]
    void RpcPause()
    {
        Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ennemi")
        {
            Debug.Log("attaque");
            Destroy(col.gameObject);
            //radar.RpcBrouille();
            lampe.SetFreq(10f);
        }
    }
}
