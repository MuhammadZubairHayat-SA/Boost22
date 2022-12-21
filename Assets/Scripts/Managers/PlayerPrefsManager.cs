using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager
{
    public static string GetDeviceIdentifier ()
	{
		if (PlayerPrefs.HasKey("deviceIdentifier"))
		{
			return PlayerPrefs.GetString("deviceIdentifier");
		}
		else
		{
			string deviceIdentifier = Guid.NewGuid().ToString();
			PlayerPrefs.SetString("deviceIdentifier", deviceIdentifier);
			PlayerPrefs.Save();
            return deviceIdentifier;
		}
	}


	public static void SaveRush22Ranking (int position, int participantCount, int score, int leadScore)
	{
		PlayerPrefs.SetInt("Rush22Ranking_position", position);
		PlayerPrefs.SetInt("Rush22Ranking_participantCount", participantCount);
		PlayerPrefs.SetInt("Rush22Ranking_score", score);
		PlayerPrefs.SetInt("Rush22Ranking_leadScore", leadScore);
		PlayerPrefs.Save();
	}


	public static void DeleteRush22Ranking ()
	{
		PlayerPrefs.DeleteKey("Rush22Ranking_position");
		PlayerPrefs.DeleteKey("Rush22Ranking_participantCount");
		PlayerPrefs.DeleteKey("Rush22Ranking_score");
		PlayerPrefs.DeleteKey("Rush22Ranking_leadScore");
		PlayerPrefs.Save();
	}

    public static void SavePlayerPrefsForKilledDuringGame (Event gameEvent = null)
    {
		var killedDuringGame = 1;
        if (PlayerPrefs.HasKey("KilledDuringGame"))
        {
			killedDuringGame += PlayerPrefs.GetInt("KilledDuringGame");
        }

        int eventId = gameEvent != null ? gameEvent.id : -1;
        PlayerPrefs.SetInt("KilledDuringGame", killedDuringGame);
		PlayerPrefs.SetInt("KilledDuringEventId", eventId);
		PlayerPrefs.SetString("KilledOnDate", DateTime.Today.ToString("dd/MM/yyyy"));
		PlayerPrefs.Save();
    }

    public static void ClearPlayerPrefsForKilledDuringGame()
    {
        PlayerPrefs.SetInt("KilledDuringGame", 0);
        PlayerPrefs.SetInt("KilledDuringEventId", -1);
		PlayerPrefs.SetString("KilledOnDate", string.Empty);
        PlayerPrefs.Save();
    }


	public static void SetComingFromGameScene (bool comingFromGameScene)
	{
		if (comingFromGameScene)
		{
			PlayerPrefs.SetInt("ComingFromGameScene", 1);
		}
		else
		{
			PlayerPrefs.SetInt("ComingFromGameScene", 0);
		}
		PlayerPrefs.Save();
	}


	public static void SaveIsReadyForRushEvent ()
	{
		PlayerPrefs.SetInt("IsReadyForRushEvent", 1);
		PlayerPrefs.Save();
	}


	public static void ClearIsReadyForRushEvent ()
	{
		PlayerPrefs.SetInt("IsReadyForRushEvent", 0);
		PlayerPrefs.Save();
	}


	public static void SaveDeviceLanguage ()
	{
		PlayerPrefs.SetString("DeviceLanguage", Application.systemLanguage.ToString());
		PlayerPrefs.Save();
	}
}