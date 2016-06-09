using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class Item : NetworkBehaviour, IPointerDownHandler {

    [Tooltip("Objectif de l'exploration")]
    public bool objectif;

    private bool obtenu = false;
    private Sprite sprite;

    private static List<Item> items = new List<Item>();

    // Use this for initialization
    void Start () {
        items.Add(this);
        if (objectif)
        {
            return;
        }
        Texture2D texture = GetComponent<Renderer>().material.mainTexture as Texture2D;
        sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        obtenu = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (objectif && isServer)
        {
            //Fin du jeu
        }
        else
        {
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
}
