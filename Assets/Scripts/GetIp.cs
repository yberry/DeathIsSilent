using UnityEngine;
using UnityEngine.UI;
using System.Net;

[RequireComponent(typeof(Text))]
public class GetIp : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = "Votre IP : " + GetUnityIp();
	}

    string GetUnityIp()
    {
        return Network.player.ipAddress;
    }

    string GetDnsIp()
    {
        string host = Dns.GetHostName();
        IPHostEntry entry = Dns.GetHostEntry(host);
        IPAddress[] list = entry.AddressList;
        return list[list.Length - 1].ToString();
    }
}
