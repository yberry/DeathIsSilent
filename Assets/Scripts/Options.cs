using UnityEngine;

public class Options : MonoBehaviour {

    public bool start;

    void Start()
    {
        if (!PlayerPrefs.HasKey("stick") || start)
        {
            PlayerPrefs.SetFloat("stick", 1.5f);
        }
        if (!PlayerPrefs.HasKey("musique") || start)
        {
            PlayerPrefs.SetFloat("musique", 0.25f);
        }
        if (!PlayerPrefs.HasKey("voix") || start)
        {
            PlayerPrefs.SetFloat("voix", 1f);
        }
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
        PlayerPrefs.SetFloat("voix", value);
    }
}
