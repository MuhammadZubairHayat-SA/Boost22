using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// ReSharper disable IdentifierTypo

public class HighscoreView : MonoBehaviour
{
	public HighscoreCell highscoreCellPrefab;
	public GameObject emptyCellPrefab;
	public GameObject listView;
	public GameObject spinner;
	public GameObject playMenu;
	public CanvasGroup refreshGraphics;
	public TextMeshProUGUI dayText;
	public Image playTab;
	public Image dailyTab;
	public Image weeklyTab;
	public Image monthlyTab;
	public Image rushTab;
	public Color selectedColor = Color.green;
	public Color unselectedColor = Color.grey;
    public bool canRefresh = true;

	private Enums.GameType selectedGameType = Enums.GameType.Boost22;
	private int selectedTab = 0;
	private bool isRefreshing = false;


	void Start ()
	{
		dayText.text = DateTime.Today.Date.Day.ToString();
	}


	public void TappedPlay ()
	{
		ClearList();
		SelectTab(0);
		spinner.SetActive(false);
	}
	
	
	public void TappedShowDaily ()
	{
		isRefreshing = true;
		dayText.text = DateTime.Today.Date.Day.ToString();
		spinner.SetActive(true);
		selectedGameType = Enums.GameType.Boost22;
		ClearList();

		ScoreApi.GetDay(Enums.GameType.Boost22, (scores, type) =>
		{
			if (selectedGameType != Enums.GameType.Boost22)
			{
				return;
			}
			SetupList(scores, 3);
		});

		SelectTab(1);
	}


	public void TappedShowWeekly ()
	{
		isRefreshing = true;
		spinner.SetActive(true);
		selectedGameType = Enums.GameType.Boost22;
		ClearList();

		ScoreApi.GetWeek(Enums.GameType.Boost22, (scores, type) =>
		{
			if (selectedGameType != Enums.GameType.Boost22)
			{
				return;
			}
			SetupList(scores, 3);
		});

		SelectTab(2);
	}


	public void TappedShowMonthly ()
	{
		isRefreshing = true;
		spinner.SetActive(true);
		selectedGameType = Enums.GameType.Turbo;
		ClearList();

		ScoreApi.GetMonth(Enums.GameType.Turbo, (scores, type) =>
		{
			if (selectedGameType != Enums.GameType.Turbo)
			{
				return;
			}
			SetupList(scores, 3);
		});

		SelectTab(3);
	}


	public void UpdateLatestEventList ()
	{
		isRefreshing = true;
		dayText.text = DateTime.Today.Date.Day.ToString();
		spinner.SetActive(true);
		selectedGameType = Enums.GameType.Boost22;
		ClearList();

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

			int numberOfPrizeWinners = -1;
			if (eventScoreboard.gameEvent.prizeDistribution != null)
			{
				numberOfPrizeWinners = eventScoreboard.gameEvent.prizeDistribution.Split(',').Length;
			}

            SetupList(eventScoreboard.scores, numberOfPrizeWinners);

            /*var gameEvent = eventScoreboard.gameEvent;
            dayText.text = "TOP RANK - " + gameEvent.LocalStartTime(false).ToString("dd/MM/yyyy HH:mm");

            if (gameEvent.TimeUntilEvent().TotalMinutes < 0 && gameEvent.TimeUntilEvent().TotalMinutes > -gameEvent.eventTimeInMinutes)
            {
                dayText.text = "CURRENT RANK - ";
            }
            else
            {
                dayText.text = "TOP RANK - ";
            }

            dayText.text += gameEvent.LocalStartTime(false).ToString("dd/MM/yyyy HH:mm");*/
        });
        
		SelectTab(4);
	}

	private void SetupList(IEnumerable<Score> scores, int numberOfPrizeWinners)
	{
		ClearList();

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
					var highscoreCell = Instantiate(highscoreCellPrefab, listView.transform);
					bool isPrizeWinner = false;

					if (position <= numberOfPrizeWinners)
					{
						isPrizeWinner = true;
					}
					
					highscoreCell.Setup(position, highscore, isPrizeWinner);
					positionsToSkip++;
				}
				
				position += positionsToSkip;			
			}

			if (emptyCellPrefab != null)
			{
				GameObject invisibleCell = Instantiate(emptyCellPrefab, listView.transform);
				foreach (Transform child in invisibleCell.transform)
				{
					child.gameObject.SetActive(false);
				}
			}
		}

		/*
		foreach (var score in scores)
		{
			var highscoreCell = Instantiate(highscoreCellPrefab, listView.transform);
			highscoreCell.Setup(index, score);
			index += 1;
		}*/
		
		spinner.SetActive(false);
		isRefreshing = false;
	}


	private void ClearList()
	{
		var listChildCount = listView.transform.childCount;
		for (var i = listChildCount - 1; i >= 0; i--)
		{
			Destroy(listView.transform.GetChild(i).gameObject);
		}
	}


	private RectTransform listViewTransform;
	private void Update ()
	{
        if (!canRefresh)
        {
            return;
        }

		if (listViewTransform == null)
		{
			listViewTransform = listView.GetComponent<RectTransform>();
		}

		if (Input.GetMouseButton(0) && listViewTransform.anchoredPosition.y < -25)
		{
			refreshGraphics.alpha = Mathf.Clamp01(-1 * (listViewTransform.anchoredPosition.y + 25) / 100.0f);

			if (isRefreshing)
			{
				refreshGraphics.transform.eulerAngles = Vector3.zero;
			}
			else
			{
				refreshGraphics.transform.eulerAngles = new Vector3(0, 0, 180 * (1 - refreshGraphics.alpha));
			}
		}
		else if (refreshGraphics.alpha != 0)
		{
			refreshGraphics.alpha = 0;
			refreshGraphics.transform.eulerAngles = Vector3.zero;
		}

		if (Input.GetMouseButtonUp(0) && listViewTransform.anchoredPosition.y < -100)
		{
			if (selectedTab == 3)
			{
				TappedShowMonthly();
			}
			else if (selectedTab == 2)
			{
				TappedShowWeekly();
			}
			else if (selectedTab == 1)
			{
				TappedShowDaily();
			}
			else if (selectedTab == 0)
			{
				TappedPlay();
			}
			else
			{
				UpdateLatestEventList();
			}
		}
	}


	private void SelectTab (int tab)
	{
		selectedTab = tab;

		if (rushTab != null)
		{
			if (tab == 4)
			{
				rushTab.color = selectedColor;
			}
			else
			{
				rushTab.color = unselectedColor;
			}
		}

		if (monthlyTab != null)
		{
			if (tab == 3)
			{
				monthlyTab.color = selectedColor;
			}
			else
			{
				monthlyTab.color = unselectedColor;
			}
		}

		if (weeklyTab != null)
		{
			if (tab == 2)
			{
				weeklyTab.color = selectedColor;
			}
			else
			{
				weeklyTab.color = unselectedColor;
			}
		}

		if (dailyTab != null)
		{
			if (tab == 1)
			{
				dailyTab.color = selectedColor;
			}
			else
			{
				dailyTab.color = unselectedColor;
			}
		}

		if (playTab != null)
		{
			if (tab == 0)
			{
				playTab.color = selectedColor;
				playMenu.SetActive(true);
			}
			else
			{
				playTab.color = unselectedColor;
				playMenu.SetActive(false);
			}
		}
	}
}