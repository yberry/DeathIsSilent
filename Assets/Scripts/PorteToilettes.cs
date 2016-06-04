using UnityEngine;
using UnityEngine.EventSystems;

public class PorteToilettes : MonoBehaviour, IPointerDownHandler
{
    public bool sensHoraireOuverture;
    [Range(-90f, 90f)]
    public float angleOuverture = 60f;
    public float vitesseOuverture = 50f;
    public static float distanceOuverture = 2f;
    public string eventOuverture;
    public string eventFermeture;
    public float angle = 0f;
    public bool ouverte = false;

    private Transform pos;
    private bool tourne = false;
    private Light lumiere;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distanceOuverture);
    }

    // Update is called once per frame
    void Update()
    {
        if (pos == null)
        {
            pos = Camera.main.transform;
            lumiere = pos.GetComponentInChildren<Light>();
        }

        if (!tourne)
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
        if (!ouverte && angle != angleOuverture - 90f)
        {
            if (vitesseOuverture == 0f)
            {
                angle = angleOuverture - 90f;
                tourne = false;
            }
            else
            {
                if (angle > angleOuverture - 90f)
                {
                    angle -= vitesseOuverture * Time.deltaTime;
                    if (angle < angleOuverture - 90f)
                    {
                        angle = angleOuverture - 90f;
                        tourne = false;
                    }
                }
            }
            Turn();
        }
    }

    void Turn()
    {
        float x = transform.parent.localRotation.eulerAngles.x;
        float new_angle = sensHoraireOuverture ? angle : -angle;
        transform.parent.localRotation = Quaternion.Euler(x, new_angle, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Vector3.Distance(pos.position, transform.position) < distanceOuverture && lumiere.enabled)
        {
            tourne = true;
            ouverte = !ouverte;
            AkSoundEngine.PostEvent(ouverte ? eventOuverture : eventFermeture, gameObject);
        }
    }
}
