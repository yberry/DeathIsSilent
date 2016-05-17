using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class OptionsSlideMusic : MonoBehaviour
{

    public Slider mainSliderMusic;
    public GameObject TheMusic;
    // Use this for initialization
    void Start()
    {
        TheMusic.GetComponent<AudioSource>().volume = mainSliderMusic.value;
        mainSliderMusic.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }


    public void ValueChangeCheck(){
        Debug.Log("music "+mainSliderMusic.value);
        TheMusic.GetComponent<AudioSource>().volume = mainSliderMusic.value;
    }

}
