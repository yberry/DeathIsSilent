using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerDownHandler {

    [Tooltip("Objectif de l'exploration")]
    public bool objectif;

    private bool obtenu = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        obtenu = true;
        if (objectif)
        {
            EnnemiSpawner.instance.ActiveFin();
        }
    }
}
