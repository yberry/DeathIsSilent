using UnityEngine;

public class Options : MonoBehaviour {

    public AudioSource voix;

    void Start()
    {
        PlayerPrefs.SetFloat("stick", 0.5f);
        PlayerPrefs.SetFloat("musique", 0.25f);
        PlayerPrefs.SetFloat("voix", 1f);
    }

	public void SetStick(float value)
    {
        PlayerPrefs.SetFloat("stick", value);
    }

    public void SetMusique(float value)
    {
        PlayerPrefs.SetFloat("musique", value);
    }

    public void SetVoix(float value)
    {
        voix.volume = value;
    }
}
