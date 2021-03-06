﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class Porte : NetworkBehaviour, IPointerDownHandler {

    public bool sensHoraireOuverture;
    [Range(0f, 90f)]
    public float angleOuverture = 60f;
    public float vitesseOuverture = 50f;
    public string eventOuverture;
    public string eventFermeture;

    public const float distanceOuverture = 2f;

    [SyncVar]
    private float angle = 0f;
    [SyncVar]
    private bool ouverte = false;
    private Transform pos;
    private PorteRadar porteRadar;
    private bool tourne = false;
    private Light lumiere;

    // Use this for initialization
    void Start () {
        pos = Camera.main.transform;
        string numPorte = transform.parent.name;
        string piece = transform.parent.parent.name;
        porteRadar = GameObject.FindGameObjectWithTag("BatimentRadar").transform.Find(piece).Find(numPorte).Find(transform.name).GetComponent<PorteRadar>();
        lumiere = pos.GetComponentInChildren<Light>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distanceOuverture);
    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer || !tourne)
        {
            return;
        }
        if (ouverte && angle != angleOuverture)
        {
            if (vitesseOuverture == 0f)
            {
                angle = angleOuverture;
                tourne = false;
            }
            else
            {
                if (angle < angleOuverture)
                {
                    angle += vitesseOuverture * Time.deltaTime;
                    if (angle > angleOuverture)
                    {
                        angle = angleOuverture;
                        tourne = false;
                    }
                }
            }
            Turn();
            
        }
        else if (!ouverte && angle != 0f)
        {
            if (vitesseOuverture == 0f)
            {
                angle = 0f;
                tourne = false;
            }
            else
            {
                if (angle > 0f)
                {
                    angle -= vitesseOuverture * Time.deltaTime;
                    if (angle < 0f)
                    {
                        angle = 0f;
                        tourne = false;
                    }
                }
            }
            Turn();
        }
        else
        {
            tourne = false;
        }
    }

    void Turn()
    {
        float x = transform.localRotation.eulerAngles.x;
        float new_angle = sensHoraireOuverture ? angle : -angle;
        RpcSetLocalRotation(Quaternion.Euler(x, new_angle, 0f));
        if (porteRadar != null)
        {
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
        if (Vector3.Distance(pos.position, transform.position) < distanceOuverture && !tourne && lumiere.enabled)
        {
            tourne = true;
            ouverte = !ouverte;
            AkSoundEngine.PostEvent(ouverte ? eventOuverture : eventFermeture, gameObject);
        }
    }

    public static void FermetureGenerale()
    {
        foreach (Porte porte in FindObjectsOfType<Porte>())
        {
            if (porte.ouverte)
            {
                porte.ouverte = false;
                porte.tourne = true;
            }
        }
    }
}
