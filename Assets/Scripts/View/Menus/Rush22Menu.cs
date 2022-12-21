using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Managers.API;

public class Rush22Menu : MonoBehaviour 
{
	[SerializeField] private HighscoreView highscoreView;
	[SerializeField] private TextMeshProUGUI nextRushEventText;
	[SerializeField] private TextMeshProUGUI highscoreListTitleText;
	[SerializeField] private RectTransform playNowButtonFrame;


    private void OnEnable ()
    {
        UpdateNextEventText();
    }

    void Start () 
	{
		//highscoreView.UpdateLatestEventList();
		float aspectRatio = Screen.width / (float)Screen.height;
		playNowButtonFrame.localScale *= 1.25f / ((aspectRatio / 1.778f) * 1.33f);
	}
	
	
	float timer = 1.0f;
	void FixedUpdate () 
	{
		timer += Time.fixedDeltaTime;
		if (timer >= 1)
		{
			UpdateNextEventText();
			timer = 0.0f;
		}
	}	


	public void UpdateNextEventText ()
	{
        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (UserManager.LoggedInUser != null && nextRushEvent != null)
		{
            DateTime nextRushEventDateTime = nextRushEvent.LocalStartTime();

			if (nextRushEvent.TimeUntilEvent().TotalMinutes < 0)
			{
				if (nextRushEvent.TimeUntilEvent().TotalMinutes > -nextRushEvent.eventTimeInMinutes)
				{
					DateTime endTime = nextRushEvent.LocalEndTime();
					nextRushEventText.text = "Current RUSH22 event ends at " + endTime.Hour.ToString("00") + ":"  + endTime.Minute.ToString("00") + " " + DayName(endTime) + ".";	
				}
				else
				{
					nextRushEventText.text = "Latest RUSH22 event has ended.";
				}

				return;
			}			

			nextRushEventText.text = "Next RUSH22 event starts at " + nextRushEventDateTime.Hour.ToString("00") + ":"  + nextRushEventDateTime.Minute.ToString("00") + " " + DayName(nextRushEventDateTime) + ".";
		}
		else
		{
			nextRushEventText.text = "No upcoming RUSH22 events.";
		}
	}


	private string DayName (DateTime dateTime)
	{
		string dayString = string.Empty;

		if (dateTime.DayOfYear == DateTime.Now.DayOfYear && dateTime.Year == DateTime.Now.Year)
		{
			dayString = "today";
		}
		else if (dateTime.DayOfYear == DateTime.Now.DayOfYear - 1 && dateTime.Year == DateTime.Now.Year)
		{
			dayString = "yesterday";
		}
		else if (dateTime.DayOfYear == DateTime.Now.DayOfYear + 1 && dateTime.Year == DateTime.Now.Year)
		{
			dayString = "tomorrow";
		}
		else if (dateTime.DayOfYear < DateTime.Now.DayOfYear + 7 && dateTime.Year == DateTime.Now.Year)
		{
			dayString = "on " + dateTime.DayOfWeek.ToString();
		}
		else
		{
			dayString = "on " + MonthName(dateTime.Month) + " " + dateTime.Day;
			
			if (dateTime.Year != DateTime.Now.Year)
			{
				dayString += " " + dateTime.Year;
			}
		}

		return dayString;
	}


	private string MonthName (int monthNumber)
	{
		string monthName = string.Empty;

		switch (monthNumber)
		{
			case 1:
				monthName = "January";
				break;
			case 2:
				monthName = "February";
				break;
			case 3:
				monthName = "March";
				break;
			case 4:
				monthName = "April";
				break;
			case 5:
				monthName = "May";
				break;
			case 6:
				monthName = "June";
				break;
			case 7:
				monthName = "July";
				break;
			case 8:
				monthName = "August";
				break;
			case 9:
				monthName = "September";
				break;
			case 10:
				monthName = "October";
				break;
			case 11:
				monthName = "November";
				break;
			case 12:
				monthName = "December";
				break;
		}

		return monthName;
	}
}
