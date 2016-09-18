using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class ButtonSound : MonoBehaviour, ISelectHandler {

    private const string eventMove = "Play_Menu_Move";
    private const string eventClic = "Play_Menu_Clic";

    void Start()
    {
        Selectable selectable = GetComponent<Selectable>();
        if (selectable is Button)
        {
            Button bouton = selectable as Button;
            bouton.onClick.AddListener(Click);
        }
        else if (selectable is Toggle)
        {
            Toggle toggle = selectable as Toggle;
            toggle.onValueChanged.AddListener(Click);
        }
    }
	
	public void OnSelect(BaseEventData eventData)
    {
        AkSoundEngine.PostEvent(eventMove, gameObject);
    }

    void Click()
    {
        AkSoundEngine.PostEvent(eventClic, gameObject);
    }

    void Click(bool click)
    {
        AkSoundEngine.PostEvent(eventClic, gameObject);
    }
}
