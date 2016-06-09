using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AffichageItems : MonoBehaviour {

    public float tempsAffichage = 3f;
    public Image image;
    public Text text;

    public void Affiche(Sprite sprite)
    {
        image.sprite = sprite;
        image.preserveAspect = false;
        image.preserveAspect = true;
        text.text = "obtenu ! (" + Item.NbObtenus() + "/" + Item.MaxItems() + ")";
        gameObject.SetActive(true);
        StartCoroutine(Affichage());
    }

	IEnumerator Affichage()
    {
        yield return new WaitForSeconds(tempsAffichage);
        gameObject.SetActive(false);
    }
}
