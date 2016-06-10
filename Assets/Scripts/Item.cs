using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class Item : NetworkBehaviour, IPointerDownHandler {

    [Tooltip("Objectif de l'exploration")]
    public bool objectif;
    public Sprite sprite;

    private bool obtenu = false;

    private static List<Item> items = new List<Item>();

    // Use this for initialization
    void Start () {
        items.Add(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        obtenu = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        Joueur joueur = FindObjectOfType<Joueur>();
        if (objectif)
        {
            EnnemiSpawner.instance.End();
            StartCoroutine(joueur.PlayEnd());
        }
        else
        {
            joueur.Affiche(sprite);
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
}
