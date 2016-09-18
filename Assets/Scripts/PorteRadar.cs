using UnityEngine;
using UnityEngine.Networking;

public class PorteRadar : NetworkBehaviour {

	[ClientRpc]
    public void RpcSetLocalRotation(Quaternion localRotation)
    {
        transform.localRotation = localRotation;
    }
}
