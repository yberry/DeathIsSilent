using UnityEngine;
using UnityStandardAssets.ImageEffects;
[AddComponentMenu("Image Effects/GlitchEffect")]

public class Brouilleur : MonoBehaviour
{
    public OVRInput.Button brouilleButton;
    public GameObject[] disparu;

    private NoiseAndScratches script;

    void Start()
    {
        script = GameObject.Find("Radar Camera").GetComponent<NoiseAndScratches>();
        script.activate = false;
    }

    void Update()
    {
        if (OVRInput.GetDown(brouilleButton))
        {
            Debug.Log("brouille");
            script.activate = !script.activate;
            //GetComponent<Radar>().activate = !GetComponent<Radar>().activate;

            foreach (GameObject o in disparu)
            {
                if (o.GetComponent<MeshRenderer>() == null)
                {
                    foreach (Transform child in o.transform)
                    {
                        child.GetComponent<MeshRenderer>().enabled = !child.GetComponent<MeshRenderer>().enabled;
                    }
                }
                else {
                    o.GetComponent<MeshRenderer>().enabled = !o.GetComponent<MeshRenderer>().enabled;
                }
            }
        }
    }
}

