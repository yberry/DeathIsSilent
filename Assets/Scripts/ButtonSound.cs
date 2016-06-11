using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, ISelectHandler {

    private Button bouton;

    private const string eventMove = "Play_Menu_Move";
    private const string eventClic = "Play_Menu_Clic";

	// Use this for initialization
	void Start () {
        bouton = GetComponent<Button>();
        bouton.onClick.AddListener(PlayClic);
	}
	
	public void OnSelect(BaseEventData eventData)
    {
        AkSoundEngine.PostEvent(eventMove, gameObject);
    }

    void PlayClic()
    {
        AkSoundEngine.PostEvent(eventClic, gameObject);
    }
}
