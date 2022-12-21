using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicHighscoresMenu : MonoBehaviour
{
    [SerializeField] private RectTransform playNowButtonFrame;


    private void Start() 
    {
        float aspectRatio = Screen.width / (float)Screen.height;
		playNowButtonFrame.localScale *= 1.25f / ((aspectRatio / 1.778f) * 1.33f);    
    }


    private void OnEnable() 
    {       
        this.GetComponent<HighscoreView>().TappedPlay();
    }
}
