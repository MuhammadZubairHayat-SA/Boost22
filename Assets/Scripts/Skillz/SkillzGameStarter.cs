using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillzGameStarter : MonoBehaviour
{

    void Start ()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            PlayerPrefs.SetFloat("GameTempo", 2.5f);
            PlayerPrefs.Save();
            GameManager.shared.NewGame(Enums.GameType.Boost22);
        }

        // Set seed for random number generator here
        int seed = (int)(Random.value * 100000);
        print("Setting random seed to " + seed);
        Random.InitState(seed);
    }


    public void StartGame ()
    {
        StartCoroutine(LoadYourAsyncScene(2));
    }


    private IEnumerator LoadYourAsyncScene(int sceneIndex, float delay = 0.0f)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(Delays.revealLastCardsDelay + Delays.wonGameScoreboardDelay + delay);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    public void GoBackToStartScreen ()
    {
        StartCoroutine(LoadYourAsyncScene(0, delay: 7.0f));
    }
}
