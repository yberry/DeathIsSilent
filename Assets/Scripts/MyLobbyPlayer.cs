using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyLobbyPlayer : NetworkLobbyPlayer {

    public Image readyImage;
    public Toggle readyToggle;

    public Sprite OFF;
    public Sprite ON;

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        MyLobbyPlayerList.instance.AddPlayer(this);

        readyToggle.GetComponent<Image>().sprite = OFF;
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
        readyToggle.interactable = true;
        readyToggle.onValueChanged.AddListener(OnToggleClicked);
    }

    void SetupOtherPlayer()
    {
        readyToggle.interactable = false;
    }

    void OnToggleClicked(bool ready)
    {
        if (ready)
        {
            SendReadyToBeginMessage();
        }
        else
        {
            SendNotReadyToBeginMessage();
        }
    }

    public override void OnClientReady(bool readyState)
    {
        readyToggle.isOn = readyState;
    }

    public void OnDestroy()
    {
        MyLobbyPlayerList.instance.RemovePlayer(this);
    }
}
