using UnityEngine;
using UnityEngine.UI;

public class GetIp : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (GetComponent<Text>() != null)
        {
            GetComponent<Text>().text = Network.player.ipAddress;
        }
        else if (GetComponent<InputField>() != null)
        {
            GetComponent<InputField>().text = Network.player.ipAddress;
        }
	}
}
