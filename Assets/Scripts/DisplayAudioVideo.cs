using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DisplayAudioVideo : MonoBehaviour {

    public OVRInput.Button oculusTalkButton;
    public OVRInput.Button radarTalkButton;
    

    private string microOculus;
    private string microRadar;

    private int minFreqOculus;
    private int maxFreqOculus;
    private int minFreqRadar;
    private int maxFreqRadar;

    private AudioSource source1;
    private AudioSource source2;

    void Start () {
        Debug.Log("Ecrans connectés : " + Display.displays.Length);
        Debug.Log("Micros connectés : " + Microphone.devices.Length);
        for (int i = 1; i <= 3; i++)
        {
            if (Display.displays.Length > i)
            {
                //Display.displays[i].Activate();
            }
        }

        //microOculus = PlayerPrefs.GetString("microOculus", Microphone.devices[0]);
        //microRadar = PlayerPrefs.GetString("microRadar", Microphone.devices[1]);
        microOculus = Microphone.devices[0];
        microRadar = Microphone.devices[1];

        Microphone.GetDeviceCaps(microOculus, out minFreqOculus, out maxFreqOculus);
        if (minFreqOculus == 0 && maxFreqOculus == 0)
        {
            maxFreqOculus = 44100;
        }
        Microphone.GetDeviceCaps(microRadar, out minFreqRadar, out maxFreqRadar);
        if (minFreqRadar == 0 && maxFreqRadar == 0)
        {
            maxFreqRadar = 44100;
        }

        AudioSource[] sources = GetComponents<AudioSource>();
        source1 = sources[0];
        source2 = sources[1];

        //StartMicrophone();
    }

    void StartMicrophone()
    {
        source1.clip = Microphone.Start(microOculus, true, 10, maxFreqOculus);
        source1.loop = true;
        while (Microphone.GetPosition(microOculus) <= 0) { }
        source1.Play();

        source2.clip = Microphone.Start(microRadar, true, 10, maxFreqRadar);
        source2.loop = true;
        while (Microphone.GetPosition(microRadar) <= 0) { }
        source2.Play();
    }
}
