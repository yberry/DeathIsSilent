using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnnemiRadar : NetworkBehaviour {

    public bool dejaEuCollision = false;
    public float timeLimited;
    public float tempsDisparition = 5f;

    private float timeElapsedAppeared = 0f;
    private float alpha = 1f;

    void Update()
    {
        MeshRenderer r = GetComponent<MeshRenderer>();
        foreach (Material m in r.materials)
        {
            Color c = m.color;
            c.a = alpha;
            m.color = c;
        }
        if (dejaEuCollision)
        {
            tempsDisparition = 5f;
            timeElapsedAppeared += Time.deltaTime;
            if (alpha > 0.0f)
            {
                alpha -= Time.deltaTime / timeLimited;
            }
        }
        else
        {
            tempsDisparition -= Time.deltaTime;
        }
        if (timeElapsedAppeared >= timeLimited)
        {
            GetComponent<Renderer>().enabled = false;
            dejaEuCollision = false;
            timeElapsedAppeared = 0f;
            alpha = 1f;
        }
        if (tempsDisparition <= 0f)
        {
            //Destroy(transform.parent.gameObject);
        }
    }
}
    