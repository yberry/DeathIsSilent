using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GetIp : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = "Votre IP : " + Network.player.ipAddress;
	}
}
