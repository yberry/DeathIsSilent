using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Item : NetworkBehaviour, IPointerDownHandler {

    [Tooltip("Objectif de l'exploration")]
    public bool objectif;
    public Sprite sprite;

    private bool obtenu = false;

    private const string eventVu = "Play_Find";
    private static List<Item> items = new List<Item>();

    // Use this for initialization
    void Start () {
        items.Add(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        obtenu = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (objectif)
        {
            EnnemiSpawner.instance.End();
        }
        else
        {
            AkSoundEngine.PostEvent(eventVu, gameObject);
            FindObjectOfType<Joueur>().Affiche(sprite);
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

    public static int MaxItems()
    {
        return items.Count - 1;
    }

    public static void Vider()
    {
        items = new List<Item>();
    }
}
