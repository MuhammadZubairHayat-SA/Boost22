using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToSkillz : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Screen.orientation=ScreenOrientation.Portrait;
        SceneManager.LoadScene(2);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
