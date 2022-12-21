
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SkillzSDK;

public class MatchController : MonoBehaviour
{
    public static MatchController instance;


    List<string> keys = new List<string>()
    {
        "games_played"
    };

    public static int LeagueScore=0;
    public static int PlayGroundScore=0;
    public static int RushScore=0;

    Dictionary<string, object> updateDict = new Dictionary<string, object>()
    {
        { "MyGameLeagueScore",LeagueScore},
        { "MyGamePlayGroundScore", PlayGroundScore },
        { "MyGameRushScore", RushScore }
    };

    public int _retrySeconds;





    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(this);
    }


    public void TryToSubmitScore()
    {
        string score = GetScore();
        SkillzCrossPlatform.SubmitScore(score, OnSuccess, OnFailure);
    }

    void OnSuccess()
    {
        Debug.Log("Success");
        MatchComplete();
    }

    string GetScore()
    {
        int score = Random.Range(1, 101);
        return score.ToString();
    }

    void OnFailure(string reason)
    {
        Debug.LogWarning("Fail: " + reason);
        StartCoroutine(RetrySubmit());
    }

    IEnumerator RetrySubmit()
    {
        yield return new WaitForSeconds(_retrySeconds);
        TryToSubmitScore();
    }


    void MatchComplete()
    {
        SkillzCrossPlatform.ReturnToSkillz();
    }


    public void AbortMatch()
    {
        SkillzCrossPlatform.AbortMatch();
    }



    // skills default data want to show in your game press GetData Function


    void OnReceivedData(Dictionary<string, SkillzSDK.ProgressionValue> data)
    {
        Debug.LogWarning("Success");
        // Do something with the data, such as populate a custom Progression scene

        // Example manipulation of the returned data
        // Printing each key/value to console
        foreach (var kvp in data)
            Debug.LogWarning("Progression default player data key: " + kvp.Key + ", value: " + kvp.Value.Value);
    }


    void OnReceivedDataFail(string reason)
    {
        Debug.LogWarning("Fail: " + reason);
        // Continue without Progression data
    }

    public void GetData()
    {
        SkillzCrossPlatform.GetProgressionUserData(ProgressionNamespace.DEFAULT_PLAYER_DATA, keys, OnReceivedData, OnReceivedDataFail);
    }


    // End Default data



    // Skills Custom player Data

    // Handle success response
    void OnSentDataSuccess()
    {
        Debug.LogWarning("Successfully updated!");
    }

    // Handle failure response
    void OnSentDataFail(string reason)
    {
        Debug.LogWarning("Fail: " + reason);
    }

    public void UpdateData()
    {
        SkillzCrossPlatform.UpdateProgressionUserData(ProgressionNamespace.PLAYER_DATA,updateDict, OnSentDataSuccess, OnSentDataFail);
    }


}