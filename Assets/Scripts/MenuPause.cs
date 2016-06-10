using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPause : MonoBehaviour {

    public RectTransform menuPause;
    public RectTransform menuOptions;

    public string eventQuit;

    private RectTransform currentMenu;
    private MyLobbyManager lobbyManager;
    private bool isChanging = false;

    void Start()
    {
        lobbyManager = FindObjectOfType<MyLobbyManager>();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && !isChanging)
        {
            SetSelection();
        }
    }

    public void Active()
    {
        ChangeTo(menuPause);
        gameObject.SetActive(true);
    }

    public void Desactive()
    {
        ChangeTo(menuPause);
        gameObject.SetActive(false);
    }

    void SetSelection()
    {
        Selectable[] selectables = currentMenu.GetComponentsInChildren<Selectable>();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
    }

    public void ChangeTo(RectTransform newMenu)
    {
        isChanging = true;
        if (currentMenu != null)
        {
            currentMenu.gameObject.SetActive(false);
        }

        if (newMenu != null)
        {
            newMenu.gameObject.SetActive(true);
            currentMenu = newMenu;
            SetSelection();
        }
        isChanging = false;
    }

    public void GoBackButton()
    {
        ChangeTo(menuPause);
    }

    public void Quit()
    {
        lobbyManager.StopHost();
        AkSoundEngine.PostEvent(eventQuit, gameObject);
    }
}
