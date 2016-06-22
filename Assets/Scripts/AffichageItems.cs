using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AffichageItems : MonoBehaviour {

    public float tempsAffichage = 3f;
    public Image image;
    public Text text;

    public IEnumerator Affiche(Sprite sprite)
    {
        image.sprite = sprite;
        image.preserveAspect = false;
        image.preserveAspect = true;
        text.text = "découvert ! (" + Item.NbObtenus() + "/" + Item.MaxItems() + ")";
        gameObject.SetActive(true);
        yield return new WaitForSeconds(tempsAffichage);
        gameObject.SetActive(false);
    }
}
