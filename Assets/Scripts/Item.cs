using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Item : NetworkBehaviour, IPointerDownHandler {

    [Tooltip("Objectif de l'exploration")]
    public bool objectif;

    private bool obtenu = false;

    private static List<Item> items = new List<Item>();

    // Use this for initialization
    void Start () {
        items.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        obtenu = true;
        if (objectif && isServer)
        {
            
        }
    }

    public static int NbObtenus()
    {
        int nb = 0;
        foreach (Item item in items)
        {
            if (item.obtenu)
            {
                nb++;
            }
        }
        return nb;
    }

    public static bool TousObtenus()
    {
        return NbObtenus() == items.Count;
    }
}
