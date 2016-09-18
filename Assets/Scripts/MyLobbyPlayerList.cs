using UnityEngine;
using System.Collections.Generic;

public class MyLobbyPlayerList : MonoBehaviour {

    public Sprite manette;
    public Sprite radar;

    public static MyLobbyPlayerList instance;

    private List<MyLobbyPlayer> players;

	// Use this for initialization
	void Start () {
        instance = this;
        players = new List<MyLobbyPlayer>();
	}
	
	public void AddPlayer(MyLobbyPlayer player)
    {
        players.Add(player);
        player.transform.SetParent(transform);
        player.readyImage.sprite = players.Count == 1 ? manette : radar;
        player.transform.localScale = Vector3.one;
    }

    public void RemovePlayer(MyLobbyPlayer player)
    {
        players.Remove(player);
    }
}
