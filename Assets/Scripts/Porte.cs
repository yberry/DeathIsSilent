using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class Porte : NetworkBehaviour, IPointerDownHandler {

    public bool sensHoraireOuverture;
    [Range(0f, 90f)]
    public float angleOuverture = 60f;
    public float vitesseOuverture = 50f;
    public static float distanceOuverture = 2f;
    public AudioClip sonOuverture;
    public AudioClip sonFermeture;

    [SyncVar]
    private bool ouverte = false;
    [SyncVar]
    private float angle = 0f;
    private Transform pos;
    private PorteRadar porteRadar;
    private AudioSource source;
    
	// Use this for initialization
	void Start () {
        pos = Camera.main.transform;
        string numPorte = transform.parent.name;
        string piece = transform.parent.parent.name;
        porteRadar = GameObject.FindGameObjectWithTag("BatimentRadar").transform.Find(piece).Find(numPorte).Find(transform.name).GetComponent<PorteRadar>();
        source = GetComponent<AudioSource>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distanceOuverture);
    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
        {
            return;
        }
        if (ouverte && angle != angleOuverture)
        {
            if (vitesseOuverture == 0f)
            {
                angle = angleOuverture;
            }
            else
            {
                if (angle < angleOuverture)
                {
                    angle += vitesseOuverture * Time.deltaTime;
                    if (angle > angleOuverture)
                    {
                        angle = angleOuverture;
                    }
                }
            }
            float x = transform.localRotation.eulerAngles.x;
            float new_angle = sensHoraireOuverture ? angle : -angle;
            RpcSetLocalRotation(Quaternion.Euler(x, new_angle, 0f));
            porteRadar.RpcSetLocalRotation(Quaternion.Euler(x, new_angle, 0f));
        }
        if (!ouverte && angle != 0f)
        {
            if (vitesseOuverture == 0f)
            {
                angle = 0f;
            }
            else
            {
                if (angle > 0f)
                {
                    angle -= vitesseOuverture * Time.deltaTime;
                    if (angle < 0f)
                    {
                        angle = 0f;
                    }
                }
            }
            float x = transform.localRotation.eulerAngles.x;
            float new_angle = sensHoraireOuverture ? angle : -angle;
            RpcSetLocalRotation(Quaternion.Euler(x, new_angle, 0f));
            porteRadar.RpcSetLocalRotation(Quaternion.Euler(x, new_angle, 0f));
        }
    }

    [ClientRpc]
    public void RpcSetLocalRotation(Quaternion localRotation)
    {
        transform.localRotation = localRotation;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Vector3.Distance(pos.position, transform.position) < distanceOuverture)
        {
            ouverte = !ouverte;
            source.PlayOneShot(ouverte ? sonOuverture : sonFermeture);
        }
    }
}
