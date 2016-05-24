using UnityEngine;

public class Chat : MonoBehaviour {

    public AkMultiPosEvent ev;

    private PhotonVoiceRecorder rec;
    private AudioSource source;

	// Use this for initialization
	void Start () {
        PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
        PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsEnabled = true;
        PhotonVoiceNetwork.Client.DebugEchoMode = false;

        DontDestroyOnLoad(gameObject);
	}

    void Update()
    {
        if (rec != null && source != null)
        {
            return;
        }
        foreach (PhotonVoiceRecorder r in FindObjectsOfType<PhotonVoiceRecorder>())
        {
            if (r.photonView.isMine)
            {
                rec = r;
                rec.Transmit = true;
            }
            else
            {
                source = r.GetComponent<AudioSource>();
            }
        }
    }
	
	public void SetTransmission(bool tr)
    {
        rec.Transmit = tr;
    }

    public void SetVolume(float vol)
    {
        if (source != null)
        {
            source.volume = vol;
        }
    }
}
