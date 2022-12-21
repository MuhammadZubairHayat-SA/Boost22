using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameViewController : MonoBehaviour 
{
	public AudioClip presentPlayersSound;
	public AudioClip buttonTappedSound;
	public AudioClip clearTableSound;
	public AudioClip wonCardSound;
	public AudioClip lostCardSound;
	public AudioClip wonHandSound;
	public AudioClip lostHandSound;
	public AudioClip wonGameSound;
	public AudioClip lostGameSound;
	public AudioClip excludePlayerSound;
	public AudioClip cleanSheetSound;
	public AudioClip fourOfAKindSound;
	public AudioClip knockoutSound;
	public Transform tableCenter;
	public InfoText infoText;
	public Sprite clubsSprite;
	public Sprite diamondsSprite;
	public Sprite heartsSprite;
	public Sprite spadesSprite;
	public Color blackSuitColor = Color.black;
	public Color redSuitColor = Color.red;
	public Transform[] starterPiecePositions;
	public RectTransform[] playerSeatPositions;
	public Animator[] playerAnimators;
	public Image[] playerAvatars;
	public CardView cardView;
	public RectTransform playedCardsParent;
	public ButtonView buttonView;
	public CardAnimator[] highAceCardAnimators;
	public StarterPiece starterPiece;
	public List<TimerRing> timerRings;
	public Animator[] avatarScoreCircles;
	public CardsLayoutGroup cardsLayoutGroup;
	public ScoreboardView scoreboardView;
	public CanvasGroup scoreboardButtonGroup;
	public CardAnimator cardAnimatorPrefab;
	public GameObject classicText;
	public GameObject turboText;
	public Animator jackpotBonusPopups;
	public Animator tableLogo;
	public GameObject lobbyButton;
	public ErrorPrompt errorPrompt;
	public ErrorPrompt eventErrorPrompt;
	public Animator lobbyPrompt;
	public Animator lobbyPlaygroundPrompt;
	public Animator noGamesLeftPrompt;
    public Animator useExtraGamePrompt;
    public Animator startGameRushWarningPrompt;
	public Animator startGameRushInProgressPrompt;
	public Animator rushStartingPrompt;
	public RushWaitScreen rushWaitScreen;
	public Animator eventEndScreen;
	public Image tableCloth;
	public Color classicClothColor = Color.green;
	public Color playgroundClothColor = Color.blue;
	public Color rushClothColor = Color.red;
	public TextMeshProUGUI rushTimeText;
	public AnimationCurve timerFlashCurve;
	public GameObject rankScoreView;
	public TextMeshProUGUI rankScore;
	public GameObject rushRankScoreView;
	public TextMeshProUGUI rushRankTitle;
	public TextMeshProUGUI rushRank;
	public TextMeshProUGUI rushScore;
	public TextMeshProUGUI rushLeadScore;
	public GameObject rushTimerView;
	public TextMeshProUGUI rushTimer;
	public Rush22ResultsScreen rush22ResultsScreenPrefab;
	public CanvasGroup rushLeaderboardButtonGroup;
	public CanvasGroup rushScoreboardButtonGroup;
	public Animator noEventPrompt;
    [SerializeField] private Animator sceneLoadingScreen;

    private List<CardAnimator> cardAnimatorsOnTable;
	private Coroutine buttonViewCoroutine;
	private int starterPiecePositionIndex = 0;
	private bool gameHasEnded = false;
	private bool showAvatars = true;
	private bool isRushGame = false;
	private bool isPlaygroundGame = false;
    private bool eventHasEnded = false;

	private Coroutine sceneCoroutine;

	void Start () 
	{
		GameManager.shared.gameViewController = this;
		cardAnimatorsOnTable = new List<CardAnimator>();

		foreach (CardAnimator highAceCardAnimator in highAceCardAnimators)
		{
			highAceCardAnimator.transform.eulerAngles = new Vector3(0, 90, 0);
		}

		lobbyButton = GameObject.Find("LobbyButton");
	}
	

	public void UpdateRank(float delay = 0.0f)
	{
		if (delay == 0)
		{
			SetRank();
			UserApi.Update((u, e) => { SetRank(); });
		}
		else
		{
			StartCoroutine(UpdateRankDelayed(delay));
		}
	}


	public IEnumerator UpdateRankDelayed (float delay)
	{
		yield return new WaitForSeconds(delay);
		SetRank();
		UserApi.Update((u, e) => { SetRank(); });
	}


	private void SetRank()
	{
        if (rankScoreView == null)
        {
            return;
        }
		if (isRushGame && GameManager.shared.currentEvent != null)
		{
			rankScoreView.SetActive(false);
			rushRankScoreView.SetActive(true);

			if (UserManager.LoggedInUser != null)//&& UserManager.LoggedInUser.rushPosition > 0)
			{
				if (PlayerPrefs.HasKey("Rush22Ranking_position"))
				{
					rushRankTitle.text = "RUSH22";
					rushRank.text = "#" + PlayerPrefs.GetInt("Rush22Ranking_position") + " of " + PlayerPrefs.GetInt("Rush22Ranking_participantCount");
					rushScore.text = "Your score: " + PlayerPrefs.GetInt("Rush22Ranking_score");
					rushLeadScore.text = "Leader score: " + PlayerPrefs.GetInt("Rush22Ranking_leadScore");
				}
			}
			else
			{
				rushRankTitle.text = string.Empty;
				rushRank.text = string.Empty;
				rushScore.text = string.Empty;
				rushLeadScore.text = string.Empty;
			}
		}
		else if (!isRushGame && !isPlaygroundGame)
		{
			if (UserManager.LoggedInUser != null && UserManager.LoggedInUser.dayPosition > 0)
			{
				rankScoreView.SetActive(true);
				rankScore.text = "#" + UserManager.LoggedInUser.dayPosition + " (" + UserManager.LoggedInUser.dayScore + (UserManager.LoggedInUser.dayScore == 1 ? " point)" : " points)");
			}
			else
			{
				rankScoreView.SetActive(true);
				rankScore.text = "No games yet today";
			}
		}
		else
		{
			rankScoreView.transform.Find("RankingText").GetComponent<TextMeshProUGUI>().text = string.Empty;
			rankScore.text = "Playground Mode";
			rankScoreView.SetActive(true);
		}
	}

	private bool hasPromptedForRush = false;
	private void LateUpdate() 
	{
        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (nextRushEvent != null)
		{
			TimeSpan timeToNextRushEvent = nextRushEvent.TimeUntilEvent();	
			if (timeToNextRushEvent.TotalMinutes <= 2 && timeToNextRushEvent.TotalMinutes > 0 && !hasPromptedForRush && !rushWaitScreen.isWaiting && !startGameRushWarningPrompt.transform.GetChild(0).gameObject.activeInHierarchy)
			{
				//rushStartingPrompt.SetTrigger("open");
				hasPromptedForRush = true;
			}
		}
	}


	public bool resultsSentToServer = false;
	float timer = 1.0f;
	float timerEventResults = 2.0f;
    int timerEventRankingUpdate = Constants.timeBetweenEventRankingUpdatesInSeconds;
	float timerNewRushGame = 0.0f;
	bool getEventResults = false;
    void Update () 
	{
		if (GameManager.shared.currentEvent != null)
		{
			if (getEventResults)
			{
				timerEventResults += Time.deltaTime;

				if (timerEventResults >= Constants.timeBetweenEventIsClosedChecksInSeconds)
				{
					timerEventResults = 0.0f;
					ScoreApi.GetEventIsClosed(GameManager.shared.currentEvent.id, (eventIsClosed, type) => 
					{
						if (eventIsClosed)
						{
							Instantiate(rush22ResultsScreenPrefab);
							getEventResults = false;
						}
					});
				}
			}
			else
			{
				TimeSpan timeToNextRushEvent = GameManager.shared.currentEvent.TimeUntilEvent();		
							
				if (timeToNextRushEvent.TotalMinutes < 0 && timeToNextRushEvent.TotalMinutes > -GameManager.shared.currentEvent.eventTimeInMinutes)
				{
					TimeSpan timeToEnd = TimeSpan.FromMinutes(GameManager.shared.currentEvent.eventTimeInMinutes) + timeToNextRushEvent;
					var hourText = (int)timeToEnd.TotalHours > 0 ? "<mspace=28>" + ((int)timeToEnd.TotalHours).ToString("00") + "</mspace>:" : "";
					rushTimeText.text = hourText + "<mspace=28>" + ((int)timeToEnd.Minutes).ToString("00") + "</mspace>:<mspace=28>" + ((int)timeToEnd.Seconds).ToString("00") + "</mspace>";

					if (timeToEnd.TotalMinutes < 2)
					{
						float alpha = timerFlashCurve.Evaluate(timer);
						rushTimeText.color = new Color(1, 1, 1, alpha);
					}
					else
					{
						rushTimeText.color = new Color(1, 1, 1, 0.25f);
					}
				}
				else if (!gameHasEnded && timeToNextRushEvent.TotalMinutes < -GameManager.shared.currentEvent.eventTimeInMinutes)
				{
					EventHasEnded();
					rushTimeText.text = string.Empty;

					foreach (TimerRing timerRing in timerRings)
					{
						timerRing.StopTimer();
					}
				}
				
				timer += Time.deltaTime;
				if (timer >= 1)
				{
					timer = 0.0f;
					timerEventRankingUpdate += 1;
					if (timerEventRankingUpdate >= Constants.timeBetweenEventRankingUpdatesInSeconds)
					{
						timerEventRankingUpdate = 0;
						ScoreApi.GetRunningEventResults(GameManager.shared.currentEvent.id, (results, type) =>
						{
							if (results != null)
							{
                                PlayerPrefsManager.SaveRush22Ranking(results.position, results.participantCount, results.score, results.leadScore);

                                rushRankTitle.text = "RUSH22";
								rushRank.text = "#" + results.position.ToString() + " of " + results.participantCount;
								rushScore.text = "Your score: " + results.score.ToString();
								rushLeadScore.text = "Leader score: " + results.leadScore.ToString();
                            }
                        });
					}
				}
			}

			if (gameHasEnded && isRushGame && !eventHasEnded && timerNewRushGame >= 0)
			{
				timerNewRushGame += Time.deltaTime;

				if (timerNewRushGame > Delays.revealLastCardsDelay + Delays.wonGameScoreboardDelay + 3 && resultsSentToServer)
				{
					Debug.Log("Auto-starting new game");
					timerNewRushGame = -1;
					PlayAgainClassic();
				}
			}
		}
		else if (!string.IsNullOrEmpty(rushTimeText.text))
		{
			rushTimeText.text = string.Empty;
		}
	}


	public void LobbyButtonPressed ()
	{
		if (gameHasEnded)
		{
			GoToLobby();
		}
		else
		{
            if (isPlaygroundGame)
            {
                lobbyPlaygroundPrompt.SetTrigger("open");
            }
            else
            {
			    lobbyPrompt.SetTrigger("open");
            }
		}
	}


	public void GoToLobby ()
	{
        PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();

        GameManager.shared.SendPoints(adjustRankingType: AdjustRankingType.UpdateRankingWhenAbortingGame);
		Destroy(GameObject.Find("GameStarter"));
		Destroy(GameObject.Find("LocalNotificationManager"));

		if (!isRushGame && !isPlaygroundGame && !gameHasEnded) 
		{
			UserManager.AdjustLeagueGamesCount(); // after playing a league game, decrease games count, so that it's updated, if dynamic data request fails
		}
        
		GameManager.shared.currentEvent = null;
		LoadYourScene(0, false);
	}


	public void GoToLobbyFree ()
	{
		Destroy(GameObject.Find("GameStarter"));
		Destroy(GameObject.Find("LocalNotificationManager"));

        PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();

        GameManager.shared.currentEvent = null;
		LoadYourScene(0, false);
	}


	public void PlayAgainImmediate ()
	{
		if (isPlaygroundGame)
		{
			LoadYourScene(3);
		}
		else if (isRushGame)
		{
			LoadYourScene(2);
		}
		else
		{
			LoadYourScene(1);
		}
	}

	public void PlayAgain(bool isTurbo) 
	{
		sceneLoadingScreen.SetTrigger("open");
		UserApi.GetDynamicUserData((dynamicUserData, error) => 
		{
			var gamesLeftToday = UserManager.LoggedInUser.gamesLeftToday;
			var extraGamesLeftToday = UserManager.LoggedInUser.extraGamesLeftToday;
			if (dynamicUserData != null)
			{
				extraGamesLeftToday = dynamicUserData.extraGamesLeftToday;
				UserManager.LoggedInUser.extraGamesLeftToday = extraGamesLeftToday;
				gamesLeftToday = dynamicUserData.gamesLeftToday;
				UserManager.LoggedInUser.gamesLeftToday = gamesLeftToday;
			}

			GameStarter gameStarter = FindObjectOfType(typeof(GameStarter)) as GameStarter;		

			if (!isRushGame && !isPlaygroundGame && (gamesLeftToday + extraGamesLeftToday) <= 0)
			{
				noGamesLeftPrompt.SetTrigger("open");
				sceneLoadingScreen.SetTrigger("close");
				return;
			} 
            else if (!isRushGame && !isPlaygroundGame && gamesLeftToday <= 0 && extraGamesLeftToday > 0)
            {
                useExtraGamePrompt.SetTrigger("open");
				sceneLoadingScreen.SetTrigger("close");
				return;
            }
			else if (!isRushGame && UserManager.LoggedInUser.GetNextRushEvent() != null && gameStarter.hasAcceptedRush22 == null)
			{
				double minutesUntilRush = UserManager.LoggedInUser.GetNextRushEvent().TimeUntilEvent().TotalMinutes;

				if (minutesUntilRush <= 10 && minutesUntilRush > 0)
				{
					startGameRushWarningPrompt.SetTrigger("open");
					sceneLoadingScreen.SetTrigger("close");
					return;
				}
				else if (minutesUntilRush <= 0 && minutesUntilRush >= -UserManager.LoggedInUser.GetNextRushEvent().eventTimeInMinutes)
				{
					startGameRushInProgressPrompt.SetTrigger("open");
					sceneLoadingScreen.SetTrigger("close");
					return;
				}
			}
			
			gameStarter.SetGameType(isTurbo : isTurbo);
			PlayAgainImmediate();
		});	
	}


	public void PlayAgainClassic ()
	{
		PlayAgain(isTurbo: false);
	}


	public void PlayAgainTurbo ()
	{
		PlayAgain(isTurbo: true);
	}


	public void ShowWaitScreen ()
	{
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			isRushGame = true;
		}	
		
		UpdateTableColor();

        Event currentEvent = GameManager.shared.currentEvent;
        if (currentEvent == null)
        {
            GameManager.shared.currentEvent = UserManager.LoggedInUser.GetNextRushEvent();
            currentEvent = GameManager.shared.currentEvent;
        }

		if (currentEvent != null)
		{
			if (PlayerPrefs.HasKey("Rush22Ranking_position"))
			{
				rushRankScoreView.SetActive(true);
				rushRankTitle.text = "RUSH22";
				rushRank.text = "#" + PlayerPrefs.GetInt("Rush22Ranking_position") + " of " + PlayerPrefs.GetInt("Rush22Ranking_participantCount");
				rushScore.text = "Your score: " + PlayerPrefs.GetInt("Rush22Ranking_score");
				rushLeadScore.text = "Leader score: " + PlayerPrefs.GetInt("Rush22Ranking_leadScore");
			}

			rushWaitScreen.gameObject.SetActive(true);
			
			if (!PlayerPrefs.HasKey("IsReadyForRushEvent") || PlayerPrefs.GetInt("IsReadyForRushEvent") == 0)
			{
				PlayerPrefsManager.SaveIsReadyForRushEvent();

				ScoreApi.ReadyForPlayingEvent(UserManager.LoggedInUser, currentEvent.id, (success, type) =>
				{
					if (!success)
					{				
						if (type == Enums.NetworkErrorType.EventCanOnlyBePlayedFromOneDevice || type == Enums.NetworkErrorType.EventIsClosed || type == Enums.NetworkErrorType.EventNotFound || type == Enums.NetworkErrorType.EventTypeIsNotSupported || type == Enums.NetworkErrorType.NoGamesLeftToday)
						{
							eventErrorPrompt.message.text = Api.ErrorMessage(type);
							eventErrorPrompt.Show();
						}
						/*else
						{
							errorPrompt.message.text = Api.ErrorMessage(type);
							errorPrompt.Show();
						}*/
					}
				});
			}
		}
		else
		{
			noEventPrompt.SetTrigger("open");
		}

	}


	public void WaitForRush22 ()
	{
		sceneLoadingScreen.SetTrigger("open");
		GameStarter gameStarter = FindObjectOfType(typeof(GameStarter)) as GameStarter;
		gameStarter.StartWaitingForRush22();
		PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();
        LoadYourScene(2);
    }


	public void Setup(Player[] players)
	{
		Delays.ResetDelays();
		Delays.ResetGameTempo();

		SetPlayerScore(0, players, 0);

		var index = 0;
		foreach (var player in players.Where(p => p.user.id <= 100))
		{
			index += 1;

			SetPlayerScore(index, players, 0);
			var playerText = (TextMeshProUGUI) playerSeatPositions[index].GetComponentInChildren(typeof(TextMeshProUGUI));
			playerText.text = player.user.username;
			playerAvatars[index].sprite = Resources.Load<Sprite>("Avatars/avatar_" + player.user.id);
			playerAvatars[index].color = Color.white;
		}

		scoreboardButtonGroup.interactable = true;
		scoreboardButtonGroup.alpha = 1;
		rushLeaderboardButtonGroup.interactable = true;
		rushLeaderboardButtonGroup.alpha = 1;
		rushScoreboardButtonGroup.interactable = true;

		buttonView.FadeIn();			
		avatarScoreCircles[0].transform.parent.GetComponent<Animator>().SetBool("fadeOut", false);

		if (isRushGame)
		{
			Event currentEvent = GameManager.shared.currentEvent != null ? GameManager.shared.currentEvent : UserManager.LoggedInUser.GetNextRushEvent();

			if (!PlayerPrefs.HasKey("IsReadyForRushEvent") || PlayerPrefs.GetInt("IsReadyForRushEvent") == 0)
			{
				PlayerPrefsManager.SaveIsReadyForRushEvent();
				
				ScoreApi.ReadyForPlayingEvent(UserManager.LoggedInUser, currentEvent != null ? currentEvent.id : -1, (success, type) =>
				{
					if (!success)
					{				
						if (type == Enums.NetworkErrorType.EventCanOnlyBePlayedFromOneDevice || type == Enums.NetworkErrorType.EventIsClosed || type == Enums.NetworkErrorType.EventNotFound || type == Enums.NetworkErrorType.EventTypeIsNotSupported || type == Enums.NetworkErrorType.NoGamesLeftToday)
						{
							eventErrorPrompt.message.text = Api.ErrorMessage(type);
							eventErrorPrompt.Show();
						}
						/*else
						{
							errorPrompt.message.text = Api.ErrorMessage(type);
							errorPrompt.Show();
						}*/
					}
				});
			}

			//rushLeaderboardView.SetActive(true);
			Delays.SetGameTempo(2.5f);
		}
		else if (GameObject.Find("GameStarter").GetComponent<GameStarter>().IsPlaygroundGame() && SceneManager.GetActiveScene().buildIndex == 3)
		{
			isPlaygroundGame = true;
		}

		//UpdateTableColor();

		UpdateRank();
		Delays.SetDelaysTempo();

		foreach (Animator avatarScoreCircle in avatarScoreCircles)
		{
			avatarScoreCircle.speed = Delays.GetGameTempo();

			if (avatarScoreCircle.gameObject.name == "UserScoreOverlay")
			{
				avatarScoreCircle.SetBool("isUser", true);
			}
		}

		lobbyButton.GetComponent<Button>().interactable = true;
	}


	public void SetGameTypeTextOnTable (Enums.GameType gameType)
	{
		if (gameType == Enums.GameType.Boost22)
		{
			classicText.SetActive(true);
			turboText.SetActive(false);
		}
		else
		{
			classicText.SetActive(false);
			turboText.SetActive(true);
		}
	}


	public void PresentPlayers ()
	{
		foreach (Animator playerAnimator in playerAnimators)
		{
			if (playerAnimator != null)
			{
				playerAnimator.SetTrigger("present");
			}
		}

		StartCoroutine(PlayClipDelayed(presentPlayersSound, 2.0f));
	}


	public void GameOver ()
	{

	}


	public void UpdatePlayerInTurn (Player player)
	{
		// Deactivate player cards
	}


	public void HighlightPlayer (int playerIndex, float delay = 0.0f)
	{
		StartCoroutine(HighlightPlayerDelayed(playerIndex, delay));
	}


	public void InstantiateCardsInCardView ()
	{
		foreach (CardAnimator cardAnimator in cardView.cardAnimators)
		{
			Destroy(cardAnimator.gameObject);
		}

		List<CardAnimator> newCardAnimators = new List<CardAnimator>();

		for (int i = 0; i < 7; i++)
		{
			CardAnimator newCardAnimator = Instantiate(cardAnimatorPrefab);
			newCardAnimator.transform.SetParent(cardView.transform);
			newCardAnimator.transform.localPosition = Vector3.zero;
			newCardAnimator.transform.localScale = Vector3.one;
			newCardAnimator.gameObject.SetActive(false);
			newCardAnimators.Add(newCardAnimator);
		}

		cardView.cardAnimators = new List<CardAnimator>();
		cardView.cardAnimators = newCardAnimators;
	}


	public void SelectPlayerCards (List<int> indexes, bool canDeselect = true, float delay = 0.0f)
	{
		for (int i = 0; i < indexes.Count; i++)
		{
			StartCoroutine(cardView.cardAnimators[indexes[i]].Select(canDeselect, delay));
		}
	}


	public void SelectLowestCards ()
	{
		StartCoroutine(SelectLowestCardsDelayed());
	}


	public IEnumerator SelectLowestCardsDelayed ()
	{
		yield return new WaitForSeconds(0.5f * (1 / Delays.GetGameTempo()));
		GameManager.shared.SelectLowestCards();
	}
	

	public void DeselectPlayerCards (int clickedCardIndex)
	{
		for (int i = 0; i < cardView.cardAnimators.Count; i++)
		{
			if (i != clickedCardIndex && cardView.cardAnimators[i].isSelected)
			{
				cardView.cardAnimators[i].Deselect();
			}
		}
	}


	public void UpdateCardsSelectionColor ()
	{
		foreach (CardAnimator cardAnimator in cardView.cardAnimators)
		{
			cardAnimator.UpdateSelectionColor();
		}
	}


	public void AnimatePlayerCards (List<Card> cards)
	{
		if (gameHasEnded)
		{
			return;
		}

		List<CardAnimator> selectedCardAnimators = cardView.GetSelectedCardAnimators();
		List<CardAnimator> clones = new List<CardAnimator>();

		if (selectedCardAnimators.Count == 0)
		{
			return;
		}

		for (int i = 0; i < selectedCardAnimators.Count; i++)
		{
			CardAnimator clone = Instantiate(selectedCardAnimators[i]);
			Sprite suitSprite = GetSuitFromCardType(cards[i].cardType);
			Color suitColor = GetColorFromCardType(cards[i].cardType);
			string value = GetValueFromCardType(cards[i].cardType);
			clone.SetSuitAndValue(suitSprite, suitColor, value);
			clone.playedByPlayerIndex = 0;
			clone.transform.SetParent(playedCardsParent);
			clone.GetComponent<RectTransform>().anchoredPosition = selectedCardAnimators[i].GetComponent<RectTransform>().anchoredPosition;
			clones.Add(clone);
			cardAnimatorsOnTable.Add(clone);
			clone.FadeIn();

			cardView.cardAnimators.Remove(selectedCardAnimators[i]);
			DestroyImmediate(selectedCardAnimators[i].gameObject);
		}

		cardsLayoutGroup.ResizeSmooth();
		
		float elementWidth = clones[0].GetComponent<RectTransform>().sizeDelta.x;
		List<Vector2> positionsOnTable = cardsLayoutGroup.CalculatePositions(clones.Count, elementWidth);

		for (int i = 0; i < clones.Count; i++)
		{
			StartCoroutine(clones[i].MoveToPositionOnTable(positionsOnTable[i], 157, 0.1f * i));
		}

		clones.Clear();
	}


	public void AnimateOpponentCards (int playerIndex, List<Card> cards, bool waitForNewRound)
	{
		if (gameHasEnded)
		{
			return;
		}

		StartCoroutine(AnimateOpponentCardsDelayed(playerIndex, cards, waitForNewRound));
	}


	private IEnumerator AnimateOpponentCardsDelayed (int playerIndex, List<Card> cards, bool waitForNewRound)
	{
		float delay = Delays.opponentCardsDelay;// + Random.value;
		if (waitForNewRound)
		{
			delay += Delays.clearTableDelay + Delays.newRoundDelay;
		}
		yield return new WaitForSeconds(delay);

		StopPlayerTimer(playerIndex);

		List<CardAnimator> clones = new List<CardAnimator>();

		foreach (Card card in cards)
		{
			CardAnimator clone = Instantiate(cardAnimatorPrefab);

			Sprite suitSprite = GetSuitFromCardType(card.cardType);
			Color suitColor = GetColorFromCardType(card.cardType);
			string value = GetValueFromCardType(card.cardType);
			clone.SetSuitAndValue(suitSprite, suitColor, value);
			clone.playedByPlayerIndex = playerIndex;
			clone.transform.SetParent(playerSeatPositions[playerIndex].Find("CardArea"));
			clone.GetComponent<RectTransform>().anchoredPosition = playerSeatPositions[playerIndex].anchoredPosition;
			clones.Add(clone);
			cardAnimatorsOnTable.Add(clone);
		}

		RectTransform cardArea = playerSeatPositions[playerIndex].Find("CardArea").GetComponent<RectTransform>();
		
		float elementWidth = clones[0].GetComponent<RectTransform>().sizeDelta.x;
		List<Vector2> positionsOnTable = new List<Vector2>();

		if (playerIndex == 1)
		{
			positionsOnTable = cardsLayoutGroup.CalculatePositionsLeft(clones.Count, elementWidth);
		}
		else if (playerIndex == 3)
		{
			positionsOnTable = cardsLayoutGroup.CalculatePositionsRight(clones.Count, elementWidth);
		}
		else
		{
			positionsOnTable = cardsLayoutGroup.CalculatePositions(clones.Count, elementWidth);
			
		}

		if (playerIndex == 3)
		{
			for (int i = 0; i < clones.Count; i++)
			{
				StartCoroutine(clones[i].MoveToPositionOnTable(positionsOnTable[clones.Count - 1 - i] + cardArea.anchoredPosition, 0, 0.1f * i));
			}
		}
		else
		{
			for (int i = 0; i < clones.Count; i++)
			{
				StartCoroutine(clones[i].MoveToPositionOnTable(positionsOnTable[i] + cardArea.anchoredPosition, 0, 0.1f * i));
			}
		}

		GameManager.shared.UpdatePlayerInTurn();
	}


	public void RevealLastCards (List<Card> cards, Enums.GameType gameType)
	{
		if (gameHasEnded)
		{
			return;
		}

		//DehighlightPlayersDelayed(1.0f);
		HighlightPlayer(-1, 1.0f);
		bool isTurboGame = false;
		if (gameType == Enums.GameType.Turbo)
		{
			isTurboGame = true;
		}

		// Player card
		CardAnimator playerOriginal = cardView.cardAnimators[0];
		CardAnimator playerClone = Instantiate(playerOriginal);
		playerClone.transform.SetParent(playedCardsParent);
		playerClone.playedByPlayerIndex = 0;

		RectTransform playerCloneRT = playerClone.GetComponent<RectTransform>();
		playerCloneRT.anchoredPosition = playerOriginal.GetComponent<RectTransform>().anchoredPosition;
		cardAnimatorsOnTable.Add(playerClone);

		cardView.cardAnimators.Remove(playerOriginal);
		DestroyImmediate(playerOriginal.gameObject);
		
		List<Vector2> playerPositionsOnTable = cardsLayoutGroup.CalculatePositions(1, playerCloneRT.sizeDelta.x);
		playerClone.transform.Find("Back").gameObject.SetActive(true);
		StartCoroutine(playerClone.Reveal(isTurboGame, Delays.revealLastCardsDelay + 0.5f));
		StartCoroutine(playerClone.MoveToPositionOnTable(playerPositionsOnTable[0], 157, Delays.revealLastCardsDelay));
		
		playerClone.gameObject.SetActive(false);

		// Opponent cards
		for (int i = 1; i < cards.Count; i++)
		{
			if (cards[i].cardType != Enums.CardType.noCard)
			{
				Card card = cards[i];

				CardAnimator opponentClone = Instantiate(cardAnimatorPrefab);

				Sprite suitSprite = GetSuitFromCardType(card.cardType);
				Color suitColor = GetColorFromCardType(card.cardType);
				string value = GetValueFromCardType(card.cardType);
				opponentClone.SetSuitAndValue(suitSprite, suitColor, value);
				opponentClone.playedByPlayerIndex = i;

				opponentClone.transform.SetParent(playerSeatPositions[i]);
				opponentClone.transform.localPosition = Vector3.zero;
				cardAnimatorsOnTable.Add(opponentClone);
			
				RectTransform cardArea = playerSeatPositions[i].Find("LastCardPosition").GetComponent<RectTransform>();
				opponentClone.transform.Find("Back").gameObject.SetActive(true);
				StartCoroutine(opponentClone.Reveal(isTurboGame, Delays.revealLastCardsDelay + 0.6f + 0.1f * i));
				StartCoroutine(opponentClone.MoveToPositionOnTable(cardArea.anchoredPosition, 0, Delays.revealLastCardsDelay + 0.1f + 0.1f * i));
				
				opponentClone.gameObject.SetActive(false);
			}
		}
	}


	public void ShowScoreInAvatar (int playerIndex, int playerScore, bool isLoser)
	{
		if (gameHasEnded)
		{
			return;
		}

		showAvatars = false;
		avatarScoreCircles[1].SetBool("showAvatar", false);
		avatarScoreCircles[2].SetBool("showAvatar", false);
		avatarScoreCircles[3].SetBool("showAvatar", false);
		avatarScoreCircles[playerIndex].SetBool("isLoser", isLoser);
		avatarScoreCircles[playerIndex].SetTrigger("show");
		if (playerIndex == 0)
		{
			avatarScoreCircles[playerIndex].SetBool("isUser", true);
		}
		StartCoroutine(UpdateScorecircleText(playerIndex, playerScore));
	}


	public IEnumerator UpdateScorecircleText (int playerIndex, int playerScore)
	{
		yield return new WaitForSeconds(Delays.clearTableDelay + Delays.revealLastCardsDelay);

		avatarScoreCircles[playerIndex].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = playerScore.ToString();
	}


	public void ShowHideAvatars ()
	{
		showAvatars = !showAvatars;
		avatarScoreCircles[1].SetBool("showAvatar", showAvatars);
		avatarScoreCircles[2].SetBool("showAvatar", showAvatars);
		avatarScoreCircles[3].SetBool("showAvatar", showAvatars);
	}


	public void HighlightWinningCards (int winningPlayerIndex)
	{
		if (gameHasEnded)
		{
			return;
		}

		foreach (CardAnimator cardAnimator in cardAnimatorsOnTable)
		{
			if (cardAnimator.playedByPlayerIndex == winningPlayerIndex)
			{
				cardAnimator.Highlight(2.0f);
			}
		}
		
		if (winningPlayerIndex == 0)
		{
			StartCoroutine(PlayClipDelayed(wonCardSound, 1.5f));
		}
		else
		{
			StartCoroutine(PlayClipDelayed(lostCardSound, 1.5f));
		}
	}


	public void HighlightLosingCard (int losingPlayerIndex)
	{
		if (gameHasEnded)
		{
			return;
		}

		foreach (CardAnimator cardAnimator in cardAnimatorsOnTable)
		{
			if (cardAnimator.playedByPlayerIndex == losingPlayerIndex)
			{
				cardAnimator.HighlightRed(Delays.revealLastCardsDelay + Delays.excludeCardsDelay);
			}
		}

		if (losingPlayerIndex == 0)
		{
			StartCoroutine(PlayClipDelayed(lostHandSound, Delays.revealLastCardsDelay + Delays.excludeCardsDelay + 1.0f));
		}
		else
		{
			StartCoroutine(PlayClipDelayed(wonHandSound, Delays.revealLastCardsDelay + Delays.excludeCardsDelay + 1.0f));
		}
	}


	public void ClearTable (float delay = 0.0f)
	{
		if (gameHasEnded)
		{
			return;
		}

		foreach (CardAnimator cardAnimator in cardAnimatorsOnTable)
		{
			StartCoroutine(cardAnimator.Remove(delay));
		}

		StartCoroutine(PlayClipDelayed(clearTableSound, delay));
		cardAnimatorsOnTable = new List<CardAnimator>();
	}


	public void SetPlayerScore (int playerIndex, Player[] players, float delay)
	{
		scoreboardView.SetPlayerScore(playerIndex, players, delay);
	}


	public void UserWon ()
	{
		GameManager.shared.GameOver(true);
		StartCoroutine(scoreboardView.WonGame(Delays.revealLastCardsDelay + Delays.wonGameScoreboardDelay));
		//infoText.ClearAnnouncements(0.0f);
		DeactivateButtons();
		StartCoroutine(PlayClipDelayed(wonGameSound, Delays.revealLastCardsDelay + Delays.wonGameScoreboardDelay));

		GameManager.shared.SendPoints();

		if (buttonViewCoroutine != null)
		{
			StopCoroutine(buttonViewCoroutine);
		}
		buttonViewCoroutine = StartCoroutine(buttonView.GameOver(Delays.revealLastCardsDelay + Delays.wonGameScoreboardDelay, isRushGame));

        if (isPlaygroundGame)
        {
            buttonView.DoneSaving();
        }
		
		gameHasEnded = true;
	}


	public void UserLost (int rank)
	{
		GameManager.shared.GameOver(true);
		StartCoroutine(scoreboardView.LostGame(rank, Delays.revealLastCardsDelay + Delays.lostGameScoreboardDelay));
		infoText.ClearAnnouncements(0.0f);
		DeactivateButtons();
		
		StartCoroutine(PlayClipDelayed(lostGameSound, Delays.revealLastCardsDelay + Delays.lostGameScoreboardDelay));

		GameManager.shared.SendPoints();
		if (buttonViewCoroutine != null)
		{
			StopCoroutine(buttonViewCoroutine);
		}
		buttonViewCoroutine = StartCoroutine(buttonView.GameOver(Delays.revealLastCardsDelay + Delays.lostGameScoreboardDelay, isRushGame));
        if (isPlaygroundGame)
        {
            buttonView.DoneSaving();
        }
        gameHasEnded = true;
	}


	public void EventHasEnded ()
	{
		GameManager.shared.GameOver(false);
		DeactivateButtons();

		buttonView.FadeOut();
		cardView.gameObject.SetActive(false);

		StartCoroutine(SendLastEventGameResultsDelayed());
		gameHasEnded = true;
        eventHasEnded = true;
		eventEndScreen.SetTrigger("open");
		StartCoroutine(DelayedGetEventResults());
		PlayerPrefsManager.DeleteRush22Ranking();
	}


	private IEnumerator SendLastEventGameResultsDelayed ()
	{
		float delay = UnityEngine.Random.value * 3;
		Debug.Log("Delay: " + delay + " seconds");
		yield return new WaitForSeconds(delay);
		GameManager.shared.SendPoints();
	}


	private IEnumerator DelayedGetEventResults ()
	{
		yield return new WaitForSeconds(Constants.timeBetweenEventRankingUpdatesInSeconds);
		getEventResults = true;
	}


	public void ExcludePlayer (int playerIndex, List<Player> players)
	{
		if (gameHasEnded)
		{
			return;
		}

		var playersByRank = GameManager.shared.GetPlayersByRank();
		if (playerIndex != 0)
		{
			StartCoroutine(PlayerLeaveTableDelayed(playerIndex));

			if (players[playerIndex].score != playersByRank[0].score)
			{
				StartCoroutine(scoreboardView.CrossOutPlayerDelayed(playerIndex, Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.excludePlayersDelay));
				StartCoroutine(PlayClipDelayed(excludePlayerSound, Delays.revealLastCardsDelay + Delays.excludePlayersDelay));
			}
		}
		else
		{
			if (players[0].score != playersByRank[0].score)
			{
                int userRank = playersByRank.FindIndex(p => p.user.id == UserManager.LoggedInUser.id);
                UserLost(userRank);
				
				StartCoroutine(PlayerLeaveTableDelayed(playerIndex));
				StartCoroutine(scoreboardView.CrossOutPlayerDelayed(playerIndex, Delays.revealLastCardsDelay + Delays.lostGameScoreboardDelay + Delays.playerCrossOutDelay));
			}
			else
			{
				UserWon();
			}
			
			if (GameManager.shared.currentEvent != null)
			{
				ScoreApi.GetRunningEventResults(GameManager.shared.currentEvent.id, (results, type) =>
				{
					if (results != null)
					{
						PlayerPrefsManager.SaveRush22Ranking(results.position, results.participantCount, results.score, results.leadScore);
					}
				});
			}
//			PlayerLost();
//			GameManager.shared.SendPoints(); // Game over
		}
	}


	private IEnumerator PlayerLeaveTableDelayed (int playerIndex)
	{
		yield return new WaitForSeconds(Delays.revealLastCardsDelay + Delays.excludePlayersDelay * (1 / Delays.GetGameTempo()) + playerIndex * 0.25f);
		playerAnimators[playerIndex].SetBool("leaveTable", true);
	}


	public void ExcludeCard (Card card, int playerIndex)
	{
		if (gameHasEnded)
		{
			return;
		}

		CardAnimator cardAnimator = Instantiate(cardAnimatorPrefab);
		
		Sprite suitSprite = GetSuitFromCardType(card.cardType);
		Color suitColor = GetColorFromCardType(card.cardType);
		string value = GetValueFromCardType(card.cardType);
		cardAnimator.SetSuitAndValue(suitSprite, suitColor, value);
		cardAnimator.playedByPlayerIndex = playerIndex;

		StartCoroutine(scoreboardView.DisplayCardAtPlayer(cardAnimator, Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.excludePlayersDelay));
	}


	public void SetHighAceCardFaces (List<Card> cards, Card winningCard)
	{	
		for (int i = 0; i < cards.Count; i++)
		{
			Sprite suitSprite = GetSuitFromCardType(cards[i].cardType);
			string valueString = GetValueFromCardType(cards[i].cardType);
			Color suitColor = GetColorFromCardType(cards[i].cardType);

			highAceCardAnimators[i].SetSuitAndValue(suitSprite, suitColor, valueString);
			highAceCardAnimators[i].GetComponent<Animator>().SetBool("highAce", true);
			StartCoroutine(highAceCardAnimators[i].DealHighAce(tableCenter));

			if (cards[i] == winningCard)
			{
				StartCoroutine(highAceCardAnimators[i].DelayedHighlight());
			}
		}
	}


	public void SetHandCardFaces (List<Card> cards, bool unfold, float delay)
	{
		if (gameHasEnded)
		{
			return;
		}

		Debug.Log("Setting card faces");

		for (int i = 0; i < cardView.cardAnimators.Count; i++)
		{
			if (i > cards.Count)
			{
				cardView.cardAnimators.RemoveAt(i);
				Destroy(cardView.cardAnimators[i].gameObject);
			}
			else
			{
				Sprite suitSprite = GetSuitFromCardType(cards[i].cardType);
				string valueString = GetValueFromCardType(cards[i].cardType);
				Color suitColor = GetColorFromCardType(cards[i].cardType);

				CardAnimator cardAnimator =  cardView.transform.GetChild(i).GetComponent<CardAnimator>();
				cardAnimator.SetSuitAndValue(suitSprite, suitColor, valueString); 
			}
		}

		if (unfold == true)
		{
			StartCoroutine(ShowHand(cards, delay));
		}
	}


	public void StartPlayerTimer (int playerIndex, float duration, float delay)
	{
		timerRings[playerIndex].StartTimer(duration, delay);
	}


	public void StopPlayerTimer (int playerIndex)
	{
		timerRings[playerIndex].StopTimer();
	}


	public void InsertSwitchedCards (List<Card> cardsOnHand, List<Card> newCards)
	{
		if (gameHasEnded)
		{
			return;
		}

		List<int> cardIndexes = new List<int>();
		
		for (int i = 0; i < newCards.Count; i++)
		{
			cardIndexes.Add(cardsOnHand.IndexOf(newCards[i]));
		}
		
		cardView.SwitchedCards(cardIndexes);
	}


	public void SetButtonState (int buttonStateIndex, float delay = 0.0f)
	{
		buttonViewCoroutine = StartCoroutine(buttonView.ChangeButtons(buttonStateIndex, delay));
	}


	public void PlayerButtonTapped ()
	{
		StartCoroutine(PlayClipDelayed(buttonTappedSound, 0.0f));
	}


	public Sprite GetSuitFromCardType (Enums.CardType cardType)
	{
		Sprite suitSprite = null;
		Color suitColor;
		string cardTypeString = cardType.ToString();

		if (cardTypeString.Contains("clubs"))
		{
			suitSprite = clubsSprite;
			suitColor = blackSuitColor;
		}
		else if (cardTypeString.Contains("diamonds"))
		{
			suitSprite = diamondsSprite;
			suitColor = redSuitColor;
		}
		else if (cardTypeString.Contains("hearts"))
		{
			suitSprite = heartsSprite;
			suitColor = redSuitColor;
		}
		else if (cardTypeString.Contains("spades"))
		{
			suitSprite = spadesSprite;
			suitColor = blackSuitColor;
		}

		return suitSprite;
	}


	public Color GetColorFromCardType (Enums.CardType cardType)
	{
		Color suitColor = Color.clear;
		string cardTypeString = cardType.ToString();

		if (cardTypeString.Contains("clubs"))
		{
			suitColor = blackSuitColor;
		}
		else if (cardTypeString.Contains("diamonds"))
		{
			suitColor = redSuitColor;
		}
		else if (cardTypeString.Contains("hearts"))
		{
			suitColor = redSuitColor;
		}
		else if (cardTypeString.Contains("spades"))
		{
			suitColor = blackSuitColor;
		}

		return suitColor;
	}


	public string GetValueFromCardType (Enums.CardType cardType)
	{
		int value = Mathf.FloorToInt((float)cardType / 4);
		string valueString = value.ToString();

		switch (value)
		{
			case 10:
				valueString = "=";
				break;
			case 11:
				valueString = "J";
				break;
			case 12:
				valueString = "Q";
				break;
			case 13:
				valueString = "K";
				break;
			case 14:
				valueString = "A";
				break;
		}

		return valueString;
	}


	public void ShowHideStarterPiece (bool show, float delay)
	{
		if (gameHasEnded)
		{
			return;
		}

		if (show)
		{
			StartCoroutine(starterPiece.Show(delay));
		}
		else
		{
			StartCoroutine(starterPiece.Hide(delay));
		}
	}


	public void MoveStarterPiece (int toPlayerIndex, float delay = 0.0f)
	{
		if (gameHasEnded)
		{
			return;
		}

		toPlayerIndex++;
		StartCoroutine(starterPiece.Move(starterPiecePositions[starterPiecePositionIndex].position, starterPiecePositions[toPlayerIndex].position, delay));
		starterPiecePositionIndex = toPlayerIndex;
	}


	public void ActivateButtons (float delay)
	{
		if (gameHasEnded)
		{
			return;
		}

		buttonViewCoroutine = StartCoroutine(buttonView.SetButtonsActive(true, delay));
	}


	public void DeactivateButtons ()
	{
		if (gameHasEnded)
		{
			return;
		}

		buttonViewCoroutine = StartCoroutine(buttonView.SetButtonsActive(false, 0.0f));
	}


	public void SetCardsInteractable (bool interactable, float delay = 0.0f)
	{
		if (gameHasEnded)
		{
			return;
		}

		StartCoroutine(cardView.SetCanvasGroupInteractable(interactable, delay));
	}


	public void PlayNotificationSound (float delay = 0.0f)
	{
		if (gameHasEnded)
		{
			return;
		}

		StartCoroutine(buttonView.PlayNotificationSound(delay));
	}


	public void DisplayCleanSheet (int playerIndex, float delay)
	{
		if (gameHasEnded)
		{
			return;
		}

		StartCoroutine(scoreboardView.DisplayCleanSheet(playerIndex, delay));
	}


	public void DisplayBonus (int bonusId)
	{
		if (gameHasEnded)
		{
			return;
		}

		// Boost22 = 1, Turbo = 2, CleanSheet = 3, FourOfAKind = 4, KnockOut2 = 5, KnockOut3 = 6
		Debug.Log("Display bonus (" + bonusId + ")");
		StartCoroutine(DisplayBonusDelayed(bonusId));
	}


	public void DisplayBonusImmediate (int bonusId)
	{
		// For testing
		switch (bonusId)
		{
			case 3:
				tableLogo.SetTrigger("fadeOutIn");
				jackpotBonusPopups.SetTrigger("cleanSheet");
				AudioSource.PlayClipAtPoint(cleanSheetSound, Vector3.zero);
				break;
			case 4:
				tableLogo.SetTrigger("fadeOutIn");
				jackpotBonusPopups.SetTrigger("fourOfAKind");
				AudioSource.PlayClipAtPoint(fourOfAKindSound, Vector3.zero);
				break;
			case 5:
				tableLogo.SetTrigger("fadeOutIn");
				jackpotBonusPopups.SetTrigger("doubleKO");
				AudioSource.PlayClipAtPoint(knockoutSound, Vector3.zero);
				break;
			case 6:
				tableLogo.SetTrigger("fadeOutIn");
				jackpotBonusPopups.SetTrigger("tripleKO");
				AudioSource.PlayClipAtPoint(knockoutSound, Vector3.zero);
				break;
		}
	}


	private IEnumerator DisplayBonusDelayed (int bonusId)
	{
		switch (bonusId)
		{
			//case 3:
				//yield return new WaitForSeconds(Delays.clearTableDelay + Delays.revealLastCardsDelay + Delays.excludeCardsDelay);
				//tableLogo.SetTrigger("fadeOutIn");
				//jackpotBonusPopups.SetTrigger("cleanSheet");
				//AudioSource.PlayClipAtPoint(cleanSheetSound, Vector3.zero);
				//break;
			case 4:
				yield return new WaitForSeconds(1.0f * (1 / Delays.GetGameTempo()));
				tableLogo.SetTrigger("fadeOutIn");
				jackpotBonusPopups.SetTrigger("fourOfAKind");
				AudioSource.PlayClipAtPoint(fourOfAKindSound, Vector3.zero);
				break;
			case 5:
				yield return new WaitForSeconds(Delays.clearTableDelay + Delays.revealLastCardsDelay + 0.5f);
				tableLogo.SetTrigger("fadeOutIn");
				jackpotBonusPopups.SetTrigger("doubleKO");
				AudioSource.PlayClipAtPoint(knockoutSound, Vector3.zero);
				break;
			case 6:
				yield return new WaitForSeconds(Delays.clearTableDelay + Delays.revealLastCardsDelay + 0.5f);
				tableLogo.SetTrigger("fadeOutIn");
				jackpotBonusPopups.SetTrigger("tripleKO");
				AudioSource.PlayClipAtPoint(knockoutSound, Vector3.zero);
				break;
		}
	}


	Color tableColor;
	private void UpdateTableColor ()
	{
		if (isRushGame)
		{
			tableColor = rushClothColor;
		}
		else if (isPlaygroundGame)
		{
			tableColor = playgroundClothColor;
		}

		Transform fourOfAKindLogo = jackpotBonusPopups.transform.Find("4OfAKind").Find("Logo");
		foreach (Transform child in fourOfAKindLogo)
		{
			child.GetChild(0).GetComponent<TextMeshProUGUI>().color = tableColor;
		}
	}


    /*private float tableColorTimer = 0.0f;
    private void Update()
    {
        if (tableCloth.color != tableColor)
        {
            tableCloth.color = Color.Lerp(tableCloth.color, tableColor, tableColorTimer);
            tableColorTimer += Time.deltaTime * 3;
        }
    }*/


    private IEnumerator PlayClipDelayed (AudioClip clip, float delay)
	{
		yield return new WaitForSeconds(delay);
		AudioSource.PlayClipAtPoint(clip, Vector3.zero);
	}


	private IEnumerator ShowHand (List<Card> cards, float delay)
	{
		yield return new WaitForSeconds(delay);

		cardView.ShowHand();

		ActivateButtons(Delays.showHandInteractionDelay);
		SetCardsInteractable(true, Delays.showHandInteractionDelay);
	}


	private IEnumerator HighlightPlayerDelayed (int playerIndex, float delay)
	{
		yield return new WaitForSeconds(delay);

		for (int i = 0; i < playerAnimators.Length; i++)
		{
			if (i == playerIndex)
			{
				playerAnimators[i].SetBool("highlight", true);
			}
			else
			{
				playerAnimators[i].SetBool("highlight", false);
			}
		}
	}


	private IEnumerator DehighlightPlayersDelayed (float delay)
	{
		yield return new WaitForSeconds(delay);

		foreach (Animator playerAnimator in playerAnimators)
		{
			if (playerAnimator != null)
			{
				playerAnimator.SetBool("highlight", false);
			}
		}
	}

    public void LoadYourScene(int sceneIndex, bool loadAsync = true)
    {
        if (sceneCoroutine != null)
        {
			StopCoroutine(sceneCoroutine);
			sceneCoroutine = null;
        }

		if (loadAsync)
		{
        	sceneCoroutine = StartCoroutine(LoadYourAsyncScene(sceneIndex));
		}
		else
		{
			SceneManager.LoadScene(sceneIndex);
		}
    }

    private IEnumerator LoadYourAsyncScene(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
