using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] private GameObject noAudioIcon;
    [SerializeField] private Slider slider;


    private void Start ()
    {
        GetAudioVolume();
    }


    public void GetAudioVolume ()
    {
        if (PlayerPrefs.HasKey("AudioVolume"))
        {
            slider.value = PlayerPrefs.GetFloat("AudioVolume");
            AudioListener.volume = slider.value;
            Debug.Log("Audio volume is " + PlayerPrefs.GetFloat("AudioVolume"));
        }
        else
        {
            Debug.Log("No audio volume set; setting default");
            SetAudioVolume();
        }
    }


    public void SetAudioVolume ()
    {
        PlayerPrefs.SetFloat("AudioVolume", slider.value);
        PlayerPrefs.Save();
        
        AudioListener.volume = slider.value;

        if (slider.value > 0)
        {
            noAudioIcon.SetActive(false);
        }
        else
        {
            noAudioIcon.SetActive(true);
        }
    }
}
