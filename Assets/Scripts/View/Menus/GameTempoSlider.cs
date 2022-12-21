using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTempoSlider : MonoBehaviour 
{
    private Slider slider;


    private void Start ()
    {
        slider = GetComponent<Slider>();
        GetGameTempo();
    }


    public void GetGameTempo ()
    {
        if (PlayerPrefs.HasKey("GameTempo"))
        {
            slider.value = PlayerPrefs.GetFloat("GameTempo");
            Debug.Log("Game tempo is " + PlayerPrefs.GetFloat("GameTempo"));
        }
        else
        {
            Debug.Log("No tempo set; setting default");
            SetGameTempo();
        }
    }


    public void SetGameTempo ()
    {
        PlayerPrefs.SetFloat("GameTempo", slider.value);
        PlayerPrefs.Save();
    }
}