using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// ReSharper disable IdentifierTypo

public class RankingScreen : MonoBehaviour
{
	public HighscoreCell highscoreCellPrefab;
	public HighscoreCell rushCellPrefab;
	public GameObject emptyLeagueCellPrefab;
	public GameObject emptyRushCellPrefab;
	public GameObject rushListView;
	public GameObject monthListView;
	public GameObject weekListView;
	public GameObject dayListView;
	public TextMeshProUGUI dayText;
	public TextMeshProUGUI updatedText;
	public TextMeshProUGUI rushListTitleText;
	public TextMeshProUGUI rushParticipantsText;
	public TextMeshProUGUI totalPlayersText;
	public TextMeshProUGUI totalGamesText;
	public TextMeshProUGUI usersTodayText;
	public TextMeshProUGUI playersLastHourText;
	public TextMeshProUGUI statisticsDateText;
	public TextMeshProUGUI statisticsDetailsText;
	public GameObject rushActivityLight;
	public Animator statsView;
	public int updateFrequency = 15;

	private Enums.GameType selectedGameType = Enums.GameType.Boost22;
	private DateTime displayedDateTime;
	private float timer = 0.0f;


	void Start ()
	{
		displayedDateTime = DateTime.Today;
	}

	
	void Update () 
	{
		if (timer <= 0)
		{
            RefreshLists();
			timer = updateFrequency;
		}	

		timer -= Time.deltaTime;
	}


	public void RefreshLists ()
	{
		Debug.Log("Refresing lists...");

		UserApi.GetServerStats((serverStats, error) =>
		{
			if (serverStats != null)
			{
				totalPlayersText.text = serverStats.totalNumberOfPlayers + " registered players";
				totalGamesText.text = serverStats.totalNumberOfGamesPlayed + " games played";
				usersTodayText.text = serverStats.numberOfUsersRegisteredToday + (serverStats.numberOfUsersRegisteredToday == 1 ? " user " : " users ") + "registered today";
				playersLastHourText.text = serverStats.numberOfPlayersLastHour + " active " + (serverStats.numberOfPlayersLastHour == 1 ? "player " : "players ") + "the last hour";
				rushParticipantsText.text = serverStats.numberOfPlayersReadyForNextRushEvent + (serverStats.numberOfPlayersReadyForNextRushEvent == 1 ? " player is " : " players are ") + "ready for next RUSH22";
			}
		});

		ScoreApi.GetNextRushEvent((nextEvent, type) =>
		{
			ShowLatestEvent();
			ShowTheLeague();
		});
	}


	private bool isShowingStats = false;
	public void SwitchView ()
	{
		if (isShowingStats)
		{
			statsView.SetTrigger("close");
			RefreshLists();
			isShowingStats = false;	
		}
		else
		{
			statsView.SetTrigger("open");
			ShowStatisticsDetailsForDate();
			isShowingStats = true;
		}
	}


	public void ShowLatestEvent ()
	{
		selectedGameType = Enums.GameType.Boost22;

        ScoreApi.GetLatestRushEventResults(Enums.GameType.Boost22, 0, (eventScoreboard, type) =>
        {
            if (selectedGameType != Enums.GameType.Boost22)
            {
                return;
            }

            if (eventScoreboard == null || eventScoreboard.scores == null)
            {
                return;
            }
			ClearList(rushListView);
            SetupList(eventScoreboard.scores, rushListView);
			
            var gameEvent = eventScoreboard.gameEvent;

            if (gameEvent.TimeUntilEvent().TotalMinutes < 0 && gameEvent.TimeUntilEvent().TotalMinutes > -gameEvent.eventTimeInMinutes)
            {
                rushActivityLight.SetActive(true); // Icon to show that event is in progress?
            }
            else
            {
                rushActivityLight.SetActive(false);
            }

            rushListTitleText.text = gameEvent.LocalStartTime(false).ToString("dd/MM/yyyy HH:mm");
        });
	}


	public void ShowTheLeague ()
	{
		selectedGameType = Enums.GameType.Boost22;
		dayText.text = DateTime.Today.Date.Day.ToString();
		
		ScoreApi.GetDay(Enums.GameType.Boost22, (scores, type) =>
		{
			if (selectedGameType != Enums.GameType.Boost22)
			{
				return;
			}
			ClearList(dayListView);
			SetupList(scores, dayListView);
		});

		ScoreApi.GetWeek(Enums.GameType.Boost22, (scores, type) =>
		{
			if (selectedGameType != Enums.GameType.Boost22)
			{
				return;
			}
			ClearList(weekListView);
			SetupList(scores, weekListView);
		});

		ScoreApi.GetMonth(Enums.GameType.Boost22, (scores, type) =>
		{
			if (selectedGameType != Enums.GameType.Boost22)
			{
				return;
			}
			ClearList(monthListView);
			SetupList(scores, monthListView);

			DateTime now = DateTime.Now;			
			updatedText.text = "Last updated " + DayName(now) + " at " + now.Hour.ToString("00") + ":"  + now.Minute.ToString("00");
		});
	}


	public void ShowStatisticsDetailsForDate ()
	{
		statisticsDetailsText.text = string.Empty;
		statisticsDateText.text = displayedDateTime.DayOfWeek.ToString() + " " + displayedDateTime.ToShortDateString();

		UserApi.GetStatisticsDetails(displayedDateTime.ToString("yyyy-MM-dd"), (statisticsDetails, error) =>
		{
			if (!string.IsNullOrEmpty(statisticsDetails))
			{
				statisticsDetailsText.text = statisticsDetails;
			}
			else
			{
				Debug.Log(Api.ErrorMessage(error));
			}
		});
	}


	public void AdjustDateTime (int i) //   +/- (1: Day, 2: Week, 3: Month, 4: Year)
	{
		switch (i)
		{
			case -4:
				displayedDateTime = displayedDateTime.AddYears(-1);
				break;
			case -3:
				displayedDateTime = displayedDateTime.AddMonths(-1);
				break;
			case -2:
				displayedDateTime = displayedDateTime.AddDays(-7);
				break;
			case -1:
				displayedDateTime = displayedDateTime.AddDays(-1);
				break;
			case 1:
				displayedDateTime = displayedDateTime.AddDays(1);
				break;
			case 2:
				displayedDateTime = displayedDateTime.AddDays(7);
				break;
			case 3:
				displayedDateTime = displayedDateTime.AddMonths(1);
				break;
			case 4:
				displayedDateTime = displayedDateTime.AddYears(1);
				break;
		}

		ShowStatisticsDetailsForDate();
	}


	private void SetupList (IEnumerable<Score> scores, GameObject listView)
	{
		ClearList(listView);
		var index = 1;

		HighscoreCell cellPrefab;
		GameObject emptyCellPrefab;

		if (listView == rushListView)
		{
			cellPrefab = rushCellPrefab;
			emptyCellPrefab = emptyRushCellPrefab;
		}
		else
		{
			cellPrefab = highscoreCellPrefab;
			emptyCellPrefab = emptyLeagueCellPrefab;
		}

		var highscoresGroupedByScore = new List<List<Score>>();
		var score = -1;
		if (scores.Count() == 0)
		{
			if (emptyCellPrefab != null)
			{
				Instantiate(emptyCellPrefab, listView.transform);
			}
		}
		else
		{
			foreach (Score highscore in scores)
			{
				if ((int)highscore.score != score)
				{
					highscoresGroupedByScore.Add(new List<Score>());
					score = highscore.score;
				}
				highscoresGroupedByScore[highscoresGroupedByScore.Count - 1].Add(highscore);
			}

			var position = 1;
			foreach (List<Score> highscoreGroup in highscoresGroupedByScore)
			{
				int positionsToSkip = 0;
				foreach (Score highscore in highscoreGroup)
				{
					var highscoreCell = Instantiate(cellPrefab, listView.transform);
					highscoreCell.Setup(position, highscore);
					positionsToSkip++;
					index += 1;
					if (index > 15)
					{
						return;
					}
				}
				
				position += positionsToSkip;
			}
		}
	}

	private void ClearList (GameObject listView)
	{
		var listChildCount = listView.transform.childCount;
		for (var i = listChildCount - 1; i >= 0; i--)
		{
			Destroy(listView.transform.GetChild(i).gameObject);
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