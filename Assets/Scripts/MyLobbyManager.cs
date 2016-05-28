using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MyLobbyManager : NetworkLobbyManager {

    [Header("Eléments Interface")]
    [Tooltip("Addresse IP de connexion")]
    public InputField address;
    [Tooltip("Bouton pour rejoindre une partie")]
    public Button rejoindre;
    [Tooltip("Texte d'état de connexion")]
    public Text connexion;

    public RectTransform menuPrincipal;
    public RectTransform menuReseau;
    public RectTransform menuOptions;
    public RectTransform menuAttente;

    private RectTransform currentMenu;

    void Start()
    {
        currentMenu = menuPrincipal;
        SetSelection();
    }

    void Update()
    {
        SetConnexion(PhotonVoiceNetwork.instance.client.IsConnectedAndReady);
    }

    void SetSelection()
    {
        Selectable[] selectables = currentMenu.GetComponentsInChildren<Selectable>();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        return Instantiate(spawnPrefabs[conn.connectionId]);
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
            backDelegate = DelegateMenuPrincipal;
        }
    }

    public void ButtonHost()
    {
        StartHost();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        ChangeTo(menuAttente);
        backDelegate = DelegateStopHost;
    }

    public void CheckIp(string ip)
    {
        address.text = address.text.ToLower();
        rejoindre.interactable = IsValidIp(ip);
    }

    bool IsValidIp(string ip)
    {
        if (ip.ToLower() == "localhost")
        {
            return true;
        }

        string[] nums = ip.Split('.');

        if (nums.Length != 4)
        {
            return false;
        }

        foreach (string num in nums)
        {
            int n;
            if (!int.TryParse(num, out n) || !n.ToString().Length.Equals(num.Length) || n < 0 || n > 255)
            {
                return false;
            }
        }

        return true;
    }

    public void ButtonClient()
    {
        networkAddress = address.text;
        StartClient();
    }

    public override void OnStartClient(NetworkClient lobbyClient)
    {
        base.OnStartClient(lobbyClient);
        SetConnexion(false);
        PhotonNetwork.ConnectUsingSettings(string.Format("1.{0}", SceneManager.GetActiveScene().name));
    }

    void SetConnexion(bool conn)
    {
        if (conn)
        {
            connexion.text = "Connecté au chat";
            connexion.color = new Color(145f / 255f, 1f, 1f / 255f);
        }
        else
        {
            connexion.text = "Connexion au chat en cours...";
            connexion.color = new Color(1f, 248f / 255f, 1f / 255f);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        if (!NetworkServer.active)
        {
            ChangeTo(menuAttente);
            backDelegate = DelegateStopClient;
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ChangeTo(menuPrincipal);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        ChangeTo(menuPrincipal);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        menuAttente.gameObject.SetActive(false);
    }

    public delegate void BackButtonDelegate();
    public BackButtonDelegate backDelegate;
    public void GoBackButton()
    {
        backDelegate();
    }

    public void DelegateMenuPrincipal()
    {
        ChangeTo(menuPrincipal);
    }

    public void DelegateStopHost()
    {
        StopHost();
    }

    public void DelegateStopClient()
    {
        StopClient();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        ChangeTo(menuReseau);
        PhotonNetwork.Disconnect();
        Destroy(FindObjectOfType<Chat>().gameObject);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        ChangeTo(menuReseau);
        PhotonNetwork.Disconnect();
        Destroy(FindObjectOfType<Chat>().gameObject);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}
