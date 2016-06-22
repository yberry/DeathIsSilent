using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPause : MonoBehaviour {

    public RectTransform menuPause;
    public RectTransform menuOptions;

    private RectTransform currentMenu;
    private MyLobbyManager lobbyManager;

    void Start()
    {
        lobbyManager = FindObjectOfType<MyLobbyManager>();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
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
    }

    public void GoBackButton()
    {
        ChangeTo(menuPause);
    }

    public void Quit()
    {
        lobbyManager.StopHost();
    }
}
