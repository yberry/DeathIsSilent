using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoixOptions : MonoBehaviour {

    public Dropdown microOculus;
    public Dropdown microRadar;

    void Start()
    {
        microOculus.ClearOptions();
        microRadar.ClearOptions();
        foreach (string device in Microphone.devices)
        {
            Debug.Log(device);
            Dropdown.OptionData option = new Dropdown.OptionData(device);
            microOculus.options.Add(option);
            microRadar.options.Add(option);
        }
        microOculus.value = -1;
        microRadar.value = -1;
    }

	public void Lancer(bool oculus)
    {
        /*if (microOculus.value == microRadar.value)
        {
            Debug.LogWarning("Les deux joueurs ne peuvent pas avoir le même micro");
            return;
        }*/
        PlayerPrefs.SetString("microOculus", microOculus.options[microOculus.value].text);
        PlayerPrefs.SetString("microRadar", microRadar.options[microRadar.value].text);
        SceneManager.LoadScene(oculus ? "VRbatiment" : "batiment");
    }
}
