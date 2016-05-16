using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyLobbyPlayer : NetworkLobbyPlayer {

    public Image readyImage;
    public Button readyButton;

    public Sprite OFF;
    public Sprite ON;

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        MyLobbyPlayerList.instance.AddPlayer(this);

        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupOtherPlayer();
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        SetupLocalPlayer();
    }

    void SetupLocalPlayer()
    {
        readyButton.GetComponent<Image>().sprite = OFF;
        readyButton.interactable = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClicked);
    }

    void SetupOtherPlayer()
    {
        readyButton.GetComponent<Image>().sprite = OFF;
        readyButton.interactable = false;
    }

    void OnReadyClicked()
    {
        SendReadyToBeginMessage();
    }

    public override void OnClientReady(bool readyState)
    {
        if (readyState)
        {
            readyButton.GetComponent<Image>().sprite = ON;
            readyButton.interactable = false;
        }
        else
        {
            readyButton.GetComponent<Image>().sprite = OFF;
            readyButton.interactable = isLocalPlayer;
        }
    }
}
