using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PorteRadar : NetworkBehaviour {

	[ClientRpc]
    public void RpcSetLocalRotation(Quaternion localRotation)
    {
        transform.localRotation = localRotation;
    }
}
