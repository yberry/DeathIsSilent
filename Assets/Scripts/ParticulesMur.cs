using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ParticulesMur : MonoBehaviour {

    private bool particules = false;
    private ParticleSystem system;
    private int collisions = 0;
    private float duration;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
        duration = system.duration;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            if (!particules)
            {
                system.Play();
                particules = true;
            }
            collisions++;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Default")) {
            duration -= Time.deltaTime;
            if (duration <= 0f)
            {
                system.loop = true;
                duration = system.duration;
            }
        }
        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            collisions--;
            if (collisions == 0)
            {
                system.loop = false;
                particules = false;
            }
        }
    }
}
