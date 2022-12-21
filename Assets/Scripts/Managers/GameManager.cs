using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers.API;
using UnityEngine;
using Facebook.Unity;

public enum AdjustRankingType {
	UpdateRankingForClassicPlayers, UpdateRankingForAllPlayers, UpdateRankingWhenAbortingGame
};
public class GameManager 
{
	public static GameManager shared = new GameManager();
	public GameViewController gameViewController;
	public Event currentEvent;
	private Game game;
    bool isSendingPoints = false;
	

    public void NewGame (Enums.GameType gameType)
	{
		Debug.Log("Started new game");
        isSendingPoints = false;
        game = Game.Create(gameType);
		gameViewController.Setup(game.players.ToArray());
		gameViewController.SetGameTypeTextOnTable(gameType);
		FillCardPool();
		StartHighAceMode();

		GameStarter gameStarter = GameObject.FindObjectOfType(typeof(GameStarter)) as GameStarter;

		if (!gameStarter.IsPlaygroundGame())
		{
			PlayerPrefsManager.SavePlayerPrefsForKilledDuringGame(currentEvent);
		}
        if (gameStarter.IsLeagueGame())
        {
			UserManager.AdjustLeagueGamesCount();
		}
	}


	public void NewRiggedGame (Enums.GameType gameType, List<List<Enums.CardType>> playerCardTypes, int gameStartingPlayerIndex)
	{
		Debug.Log("Started new RIGGED game");
		game = Game.Create(gameType);
		game.gameIsRigged = true;
		gameViewController.Setup(game.players.ToArray());
		gameViewController.SetGameTypeTextOnTable(gameType);

		for (int i = 0; i < game.players.Count; i++)
		{
			if (playerCardTypes[i].Count > 0)
			{
				List<Card> cards = new List<Card>();
				foreach (Enums.CardType playerCardType in playerCardTypes[i])
				{
					Card card = new Card(playerCardType);
					cards.Add(card);
				}
				game.players[i].hand.AddCards(cards);
			}
			else
			{
				game.players[i].isExcluded = true;
				game.players[i].roundScore = 0;
				gameViewController.ExcludePlayer(i, game.players);
			}
		}

		game.handStartingPlayer = game.players[gameStartingPlayerIndex];
		game.roundStartingPlayer = game.players[gameStartingPlayerIndex];
		gameViewController.HighlightPlayer(gameStartingPlayerIndex, 3.0f);

		gameViewController.infoText.AnnounceHandStartingPlayer(game.handStartingPlayer.user.username, Delays.highAceDealDelay + Delays.highAceHighlightDelay);// "Spiller " + game.players.IndexOf(game.handStartingPlayer), Delays.highAceDealDelay + Delays.highAceHighlightDelay);
		gameViewController.ShowHideStarterPiece(true, 0.0f);
		gameViewController.MoveStarterPiece(game.players.IndexOf(game.handStartingPlayer), Delays.starterPieceDelay);

		game.gameStateType = Enums.GameStateType.SwitchCardsMode;
		
		FillCardPool();

		gameViewController.SetHandCardFaces(game.players[0].hand.cards, true, Delays.switchCardsModeDelay);
		int maxNumberOfCards = GetMaxNumberOfCardsToSwitch();

		if (maxNumberOfCards == 1)
		{
			gameViewController.infoText.AnnounceSwitchOneCard(Delays.switchCardsModeDelay);
		}
		else
		{
			gameViewController.infoText.AnnounceSwitchNumberOfCards(maxNumberOfCards, Delays.switchCardsModeDelay);
		}

		gameViewController.PlayNotificationSound(Delays.switchCardsModeDelay);
	}


	public void GameOver(bool gameCompleted)
	{
		if (gameCompleted) {
			foreach (Player player in game.players)
			{
				if (player.score == 0)
				{
					player.pointTypes.Add(Enums.PointType.CleanSheet);
					gameViewController.DisplayCleanSheet(player.tablePosition, Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.excludePlayersDelay);
					//if (player.tablePosition == 0)
					//{
					//	gameViewController.DisplayBonus(3);
					//}
				}
			}
		}
	}


	public bool CanSelectCard (Card card, bool announceReason = false)
	{
		switch (game.gameStateType)
		{
			case Enums.GameStateType.HighAceMode:
				break;
			case Enums.GameStateType.SwitchCardsMode:
				if (game.players[0].hand.GetSelectedCards().Count < GetMaxNumberOfCardsToSwitch())
				{
					if (announceReason)
					{
						gameViewController.infoText.ClearAnnouncements(0.0f);
					}
					return true;
				}
				else
				{
					if (announceReason)
					{
						gameViewController.infoText.AnnounceCannotSelect(0);
					}	
					return false;
				}
			case Enums.GameStateType.PlayMode:
				Player player  = game.players[0];
				List<Card> selectedCards = player.hand.GetSelectedCards();
				
				if (player.hand.cards.Count - selectedCards.Count == 1)
				{
					if (announceReason)
					{
						gameViewController.infoText.AnnounceCannotSelect(1);
					}
					return false;
				}

				bool isSameValueAsAlreadySelectedCards = false;

				if (selectedCards.Count == 0)
				{
					isSameValueAsAlreadySelectedCards = true;
				}
				else
				{
					int cardValue = Enums.CardTypeToInt(card.cardType);
					int selectedCardsValue = Enums.CardTypeToInt(selectedCards[0].cardType);
					
					if (cardValue == selectedCardsValue)
					{
						isSameValueAsAlreadySelectedCards = true;
					}
					else
					{
						if (announceReason)
						{
							gameViewController.infoText.AnnounceCannotSelect(2);
						}
						return false;
					}
				}

				if (player != game.roundStartingPlayer)
				{	
					int playedCardTypeInt = Enums.CardTypeToInt(game.highestCardsPlayed[0].cardType);
					int playedNumberOfCards = game.highestCardsPlayed.Count;
					bool isLowestUnselected = true;

					foreach (Card cardOnHand in player.hand.cards)
					{
						if (selectedCards.Contains(cardOnHand) == false && Enums.CardTypeToInt(cardOnHand.cardType) < Enums.CardTypeToInt(card.cardType))
						{
							isLowestUnselected = false;
							break;
						}
					}

					if (selectedCards.Count < playedNumberOfCards && isLowestUnselected)
					{
						return true;
					}					
					else if (selectedCards.Count < playedNumberOfCards && Enums.CardTypeToInt(card.cardType) >= playedCardTypeInt)
					{
						List<Card> cardsOfCorrectType = new List<Card>();

						foreach (Card cardOnHand in player.hand.cards)
						{
							if (Enums.CardTypeToInt(cardOnHand.cardType) == Enums.CardTypeToInt(card.cardType))
							{
								cardsOfCorrectType.Add(cardOnHand);
							}
						}

						if (cardsOfCorrectType.Count >= playedNumberOfCards && isSameValueAsAlreadySelectedCards)
						{
							if (announceReason)
							{
								gameViewController.infoText.ClearAnnouncements(0.0f);
							}
							return true;
						}
						else
						{
							if (announceReason)
							{
								gameViewController.infoText.AnnounceCannotSelect(3);
							}
							return false;
						}
					}
					else if (selectedCards.Count < playedNumberOfCards && isSameValueAsAlreadySelectedCards)
					{
						if (announceReason)
						{
							gameViewController.infoText.AnnounceCannotSelect(4);
						}
						return false;
					}
					else
					{
						if (announceReason)
						{
							gameViewController.infoText.AnnounceCannotSelect(5);
						}	
						return false;
					}
				}
				else if (isSameValueAsAlreadySelectedCards)
				{
					if (announceReason)
					{
						gameViewController.infoText.ClearAnnouncements(0.0f);
					}
					return true;
				}
				break;
		}
		if (announceReason)
		{
			gameViewController.infoText.AnnounceCannotSelect(-1);
		}	
		return false;
	}


	public void SelectOtherCardsOfType (Card card)
	{
		if (game.gameStateType == Enums.GameStateType.PlayMode && game.players[0] != game.roundStartingPlayer)
		{
			int numberOfCardsToSelect = game.highestCardsPlayed.Count - 1;
			List<int> indexes = new List<int>();

			foreach (Card cardOnHand in game.players[0].hand.cards)
			{
				if (cardOnHand != card && Enums.CardTypeToInt(cardOnHand.cardType) == Enums.CardTypeToInt(card.cardType) && numberOfCardsToSelect > 0)
				{
					int index = game.players[0].hand.cards.IndexOf(cardOnHand);
					indexes.Add(index);
					numberOfCardsToSelect--;
				}
			}

			SelectPlayerCards(indexes);
			gameViewController.SelectPlayerCards(indexes);
			UpdateButtonState();
		}
	}


	public void SelectOtherBluffCards (Card card)
	{
		if (game.gameStateType == Enums.GameStateType.PlayMode && game.players[0] != game.roundStartingPlayer)
		{
			Player player = game.players[0];
			if (player.hand.cards.IndexOf(card) != 0)
			{
				return;
			}

			int numberOfCardsToSelect = game.highestCardsPlayed.Count - 1;
			List<int> indexes = new List<int>();

			foreach (Card cardOnHand in game.players[0].hand.cards)
			{
				if (cardOnHand != card && numberOfCardsToSelect > 0)
				{
					int index = game.players[0].hand.cards.IndexOf(cardOnHand);
					indexes.Add(index);
					numberOfCardsToSelect--;
				}
			}

			SelectPlayerCards(indexes);
			gameViewController.SelectPlayerCards(indexes);
		}
	}


	public void SelectPlayerCards (List<int> indexes)
	{
		for (int i = 0; i < indexes.Count; i++)
		{
			game.players[0].hand.cards[indexes[i]].isSelected = true;
		}
	}


	public void DeselectOtherCards (Card card)
	{
		if (game.gameStateType == Enums.GameStateType.PlayMode)
		{
			int index = game.players[0].hand.cards.IndexOf(card);
			gameViewController.DeselectPlayerCards(index);
		}
	}


	public void PlayerButtonTapped (int buttonIndex)
	{
		switch (game.gameStateType)
		{
			case Enums.GameStateType.HighAceMode:
				Debug.Log("High Ace");
				break;
			case Enums.GameStateType.SwitchCardsMode:
				if (buttonIndex == 1)
				{
					List<Card> selectedCards = game.players[0].hand.GetSelectedCards();
					List<Card> newCards = SwitchCardsAndUpdatePlayerInTurn(game.players[0], selectedCards);
					gameViewController.SetHandCardFaces(game.players[0].hand.cards, false, 0.0f);
					gameViewController.InsertSwitchedCards(game.players[0].hand.cards, newCards);
					if (CurrentPlayer() != game.players[0])
					{
						gameViewController.SetCardsInteractable(false);
					}

					if (newCards.Count == 1)
					{
						gameViewController.infoText.AnnounceReceivedOneCard(0.0f);
					}
					else
					{
						gameViewController.infoText.AnnounceReceivedNumberOfCards(newCards.Count, 0.0f);
					}

					// Stop timer
				}
				else
				{
					UpdatePlayerInTurn();
					if (CurrentPlayer() == game.players[0])
					{
						gameViewController.infoText.AnnounceYourTurn(0.0f);
						gameViewController.PlayNotificationSound();
					}
					else
					{
						gameViewController.infoText.ClearAnnouncements(0.0f);
					}

					if (CurrentPlayer() != game.players[0])
					{
						gameViewController.SetCardsInteractable(false);
					}
					// Stop timer
				}
				StartPlayMode();
				break;
			case Enums.GameStateType.PlayMode:
				PlayCards(game.players[0].hand.GetSelectedCards());
				break;
		}

		gameViewController.PlayerButtonTapped();
	}


	public void UpdateButtonState ()
	{
		switch (game.gameStateType)
		{
			case Enums.GameStateType.HighAceMode:
				break;
			case Enums.GameStateType.SwitchCardsMode:
				if (game.players[0].hand.GetSelectedCards().Count > 0)
				{
					gameViewController.SetButtonState(1);
				}
				else
				{
					gameViewController.SetButtonState(0);
				}
				break;
			case Enums.GameStateType.PlayMode:
				gameViewController.SetButtonState(2);

				if ((game.highestCardsPlayed.Count == 0 && game.players[0].hand.GetSelectedCards().Count > 0) || (game.highestCardsPlayed.Count > 0 && game.players[0].hand.GetSelectedCards().Count == game.highestCardsPlayed.Count))
				{
					gameViewController.ActivateButtons(0.0f);
				}
				else
				{
					gameViewController.DeactivateButtons();
				}

				break;
		}
	}


	public Card GetCardFromIndex (Player player, int index)
	{
		if (player == null)
		{
			return game.players[0].hand.cards[index];
		}
		
		return player.hand.cards[index];
	}


	private void NewHand ()
	{
		Debug.Log("New hand");
        game.NewHand();
        gameViewController.InstantiateCardsInCardView();	
		gameViewController.DeactivateButtons();
		gameViewController.SetButtonState(0, Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.newRoundDelay);
		StartSwitchCardsMode(Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.switchCardsModeDelay);
	}


    public List<Card> SwitchCards(Player player, List<Card> cards)
    {
        player.hand.RemoveCards(cards);
        List<Card> newCards = GetNewCards(cards.Count);
        player.hand.AddCards(newCards);
        return newCards;
    }


    private List<Card> SwitchCardsAndUpdatePlayerInTurn (Player player, List<Card> cards)
	{
        var newCards = SwitchCards(player, cards);

        UpdatePlayerInTurn();

		return newCards;
	}


	private void PlayCards (List<Card> cards)
	{
		Player currentPlayer = CurrentPlayer();
		currentPlayer.PlayedCards(cards);

		bool cardsAreSameType = true;
		foreach (Card card in cards)
		{
			if (Enums.CardTypeToInt(card.cardType) != Enums.CardTypeToInt(cards[0].cardType))
			{
				cardsAreSameType = false;
				break;
			}
			else if (game.highestCardsPlayed.Count > 0 && Enums.CardTypeToInt(card.cardType) < Enums.CardTypeToInt(game.highestCardsPlayed[0].cardType))
			{
				cardsAreSameType = false;
				break;
			}
		}

		if (cards.Count == 4 && cardsAreSameType)
		{
			currentPlayer.pointTypes.Add(Enums.PointType.FourOfAKind);
			if (currentPlayer.tablePosition == 0)
			{
				gameViewController.DisplayBonus(4);
			}
		}

        if (game.highestCardsPlayed.Count == 0 || (cardsAreSameType && Enums.CardTypeToInt(cards[0].cardType) >= Enums.CardTypeToInt(game.highestCardsPlayed[0].cardType)))
		{
            game.highestCardsPlayed = cards;
        }
		
		if (currentPlayer == game.players[0])
		{
			gameViewController.StopPlayerTimer(0);
			gameViewController.AnimatePlayerCards(cards);
			gameViewController.SetCardsInteractable(false);
			UpdatePlayerInTurn();
		}
		else
		{
			bool waitForNewRound = false;

			if (game.firstRoundInHand == false && currentPlayer == game.roundStartingPlayer && currentPlayer.playedHandInRound != null)
			{
				waitForNewRound = true;
			}
			else
			{
				gameViewController.infoText.ClearAnnouncements(0);
			}

            gameViewController.AnimateOpponentCards(game.players.IndexOf(currentPlayer), cards, waitForNewRound);
		}
		
		gameViewController.DeactivateButtons();
		game.firstRoundInHand = false;
	}


	public List<Player> GetPlayersByRank() 
	{
		var playersByRank = new List<Player>();
		var excludedPlayers = game.players.Where(p => p.isExcluded).OrderBy(p => p.rankingScore);
        var nonExcludedPlayers = game.players.Where(p => !p.isExcluded).OrderBy(p => p.score - (p.user.id == UserManager.LoggedInUser.id ? 0.5 : 0));
        playersByRank.AddRange(nonExcludedPlayers);
		playersByRank.AddRange(excludedPlayers);
		return playersByRank;
	}


	public void UpdatePlayerInTurn ()
	{
		if (IsGameOver())
		{
			gameViewController.GameOver();
		}
		
		if (CurrentPlayer() == game.roundStartingPlayer && game.roundStartingPlayer.playedHandInRound != null)
		{
			Player roundWinner = RoundWinningPlayer();
			game.roundStartingPlayer = roundWinner;
			gameViewController.HighlightPlayer(game.players.IndexOf(roundWinner));
			gameViewController.infoText.AnnouncePlayerWinsRound(roundWinner.user.username, 0.0f);//"Spiller " + game.players.IndexOf(roundWinner), 0.0f);

            game.NewRound();

			gameViewController.ClearTable(Delays.clearTableDelay);
		}

		if (CurrentPlayer().hand.cards.Count <= 1)
		{
			Debug.Log("Last card");
			List<Card> lastCards = new List<Card>();

			foreach (Player player in game.players)
			{
				if (player.isExcluded == false)
				{
					lastCards.Add(player.hand.cards[0]);
				}
				else
				{
					lastCards.Add(new Card(Enums.CardType.noCard));
				}
			}
			
			gameViewController.SetCardsInteractable(true);
			gameViewController.RevealLastCards(lastCards, game.gameType);
			gameViewController.infoText.AnnounceLastCardsReveal(Delays.revealLastCardsDelay);
			gameViewController.DeactivateButtons();

			EvaluateLastCards(lastCards);

			if (game.gameType == Enums.GameType.Turbo)
			{
				var playersByRank = GetPlayersByRank();
				for (int i = 0; i < playersByRank.Count; i++) {
					if (playersByRank[i].tablePosition == 0) {
						if (i == 0)
						{
							gameViewController.UserWon();
						}
						else
						{
							gameViewController.UserLost(i);
						}
						break;
					}
				}
				return;
			}

			if (game.players[0].isExcluded)
			{
				return;
			}
			else if (ActivePlayers().Count <= 1)
			{
				// End game
				gameViewController.UserWon();
				return;
			}

			NextHandStartingPlayer();
			NewHand();
		}
		else
		{
			gameViewController.UpdatePlayerInTurn(CurrentPlayer());

			if (CurrentPlayer() == game.roundStartingPlayer && game.roundStartingPlayer.playedHandInRound == null && game.firstHand == false)
			{
				gameViewController.StartPlayerTimer(game.players.IndexOf(CurrentPlayer()), 10, Delays.clearTableDelay);
			}
			else
			{
				gameViewController.StartPlayerTimer(game.players.IndexOf(CurrentPlayer()), 10, 0);
			}

			if (CurrentPlayer().playerType == Enums.PlayerType.Robot)
			{
				//int numberOfPlayedCards = 0;
				//if (game.roundStartingPlayer.playedHandInRound != null)
				//{
				//	numberOfPlayedCards = game.highestCardsPlayed.Count;
				//}

				List<Card> cards = RobotManager.ChooseCardsToPlay(game, (RobotPlayer)CurrentPlayer());
				PlayCards(cards);
			}
			else if (CurrentPlayer() == game.players[0])
			{
				gameViewController.UpdateCardsSelectionColor();
				if (game.roundStartingPlayer == game.players[0] && game.firstRoundInHand == false)
				{
					gameViewController.SetCardsInteractable(true, Delays.clearTableDelay + Delays.yourTurnInteractionDelay);
					gameViewController.PlayNotificationSound(Delays.clearTableDelay);
					gameViewController.SetButtonState(3, Delays.clearTableDelay);
				}
				else
				{
					gameViewController.SetCardsInteractable(true, Delays.yourTurnInteractionDelay);
					gameViewController.PlayNotificationSound();
					gameViewController.SetButtonState(3);
				}

				if (CanPlay() == false)
				{
					gameViewController.SelectLowestCards();
				}
				else
				{
					gameViewController.infoText.AnnounceYourTurn(Delays.yourTurnInteractionDelay);				
				}

			}
		}

		game.firstHand = false;
	}


    public void SendPoints(AdjustRankingType adjustRankingType = AdjustRankingType.UpdateRankingForAllPlayers)
	{
        if (isSendingPoints || GameObject.Find("GameStarter").GetComponent<GameStarter>().IsPlaygroundGame())
        {
            return;
        }

        isSendingPoints = true;

        Debug.Log("SEND POINTS");
		game.endTime = Time.time;
		Debug.Log("Game duration: " + (game.endTime - game.startTime));

		if (PlayerPrefs.HasKey("KilledDuringGame"))
		{
			if (PlayerPrefs.GetInt("KilledDuringGame") > 1 && PlayerPrefs.GetString("KilledOnDate") == System.DateTime.Today.ToString("dd/MM/yyyy"))
			{				
				ScoreApi.KilledAppPunishment(UserManager.LoggedInUser, PlayerPrefs.GetInt("KilledDuringEventId"), (success, type) =>
				{
					if (success)
					{
						Debug.Log("App was killed; punishment successful");
                        PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();
					}
					else
					{
						Debug.Log("App was killed; punishment not successful: " + Api.ErrorMessage(type));
					}
				});				
			} else
            {
				PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();
			}
		}

		int currentEventId = currentEvent != null ? currentEvent.id : -1;
		if (!game.resultsSentToServer) {
			AdjustRankingScoreForPlayersToBeExcluded(adjustRankingType: adjustRankingType);

			
			// TODO: SHOW SPINNER ON BUTTON?
			ScoreApi.Add(game.players.Where(p => p.rankingScore > 0).ToList(), game.gameIdentifier, currentEventId, game.gameType, (int)(game.endTime - game.startTime), (success, type) =>
			{
                isSendingPoints = false;

                if (!success)
				{
					if (type == Enums.NetworkErrorType.EventCanOnlyBePlayedFromOneDevice || type == Enums.NetworkErrorType.EventIsClosed || type == Enums.NetworkErrorType.EventNotFound || type == Enums.NetworkErrorType.EventTypeIsNotSupported || type == Enums.NetworkErrorType.NoGamesLeftToday)
					{
						gameViewController.eventErrorPrompt.message.text = Api.ErrorMessage(type);
						gameViewController.eventErrorPrompt.Show();
					}
					else
					{
						gameViewController.errorPrompt.message.text = Api.ErrorMessage(type);
						gameViewController.errorPrompt.Show();
					}
				}
				else
				{
                    gameViewController.errorPrompt.Hide();
					gameViewController.buttonView.DoneSaving();
					gameViewController.resultsSentToServer = true;
					game.resultsSentToServer = true;
                    gameViewController.UpdateRank(Delays.revealLastCardsDelay + Delays.lostGameScoreboardDelay);
					List<Player> humanPlayers = game.players.Where(p => !p.user.IsAi()).ToList();
					LogAchieveLevelEvent(humanPlayers[0].user);
                }
			});

		}
	}


	public void SelectLowestCards ()
	{
		List<int> indexes = new List<int>();
		int numberOfCardsToPlay = game.highestCardsPlayed.Count;

		for (int i = 0; i < numberOfCardsToPlay; i++)
		{
			game.players[0].hand.cards[i].isSelected = true;
			indexes.Add(i);
		}

		SelectPlayerCards(indexes);
		gameViewController.SelectPlayerCards(indexes, false, Delays.selectLowestCardsDelay);
		
		if (numberOfCardsToPlay == 1)
		{
			gameViewController.infoText.AnnounceCannotMatchCard(0.5f);
		}
		else
		{
			gameViewController.infoText.AnnounceCannotMatchCards(numberOfCardsToPlay, 0.5f);
		}

		gameViewController.PlayNotificationSound();
	}


	private bool CanPlay ()
	{
		Player player = CurrentPlayer();
		
		if (player == game.roundStartingPlayer)
		{
			return true;
		}

		int playedCardTypeInt = Enums.CardTypeToInt(game.highestCardsPlayed[0].cardType);
		int playedNumberOfCards = game.highestCardsPlayed.Count;
		
		foreach (Card card in player.hand.cards)
		{
			if (Enums.CardTypeToInt(card.cardType) >= playedCardTypeInt)
			{
				List<Card> cardsOfCorrectType = new List<Card>();

				foreach (Card cardOnHand in player.hand.cards)
				{
					if (Enums.CardTypeToInt(cardOnHand.cardType) == Enums.CardTypeToInt(card.cardType))
					{
						cardsOfCorrectType.Add(cardOnHand);
					}
				}

				if (cardsOfCorrectType.Count >= playedNumberOfCards)
				{
					return true;
				}
			}
		}

		return false;
	}


	private Player RoundWinningPlayer ()
	{
		for (int i = 0; i < game.players.Count; i++)
		{
			if (game.players[i].isExcluded == false)
			{
				int score = 0;
				Card compareCard = null;
				bool cardsMatch = true;

				for (int j = 0; j < game.players[i].playedHandInRound.cards.Count; j++)
				{
					if (j > 0 && Enums.CardTypeToInt(game.players[i].playedHandInRound.cards[j].cardType) != Enums.CardTypeToInt(compareCard.cardType))
					{
						cardsMatch = false;
					}

					if (cardsMatch)
					{
						score += Enums.CardTypeToInt(game.players[i].playedHandInRound.cards[j].cardType);
					}
					else
					{
						score = 0;
					}

					compareCard = game.players[i].playedHandInRound.cards[j];
				}

				game.players[i].roundScore = score;
			}
		}

		Player roundWinner = game.players[0];

		int l = game.players.IndexOf(game.roundStartingPlayer);

		for (int k = 0; k < game.players.Count; k++)
		{
			if (l >= game.players.Count)
			{
				l = 0;
			}

			if (game.players[l].isExcluded == false)
			{
				if (game.players[l].roundScore >= roundWinner.roundScore)
				{
					roundWinner = game.players[l];
				}
			}
		
			l++;
		}

		gameViewController.HighlightWinningCards(game.players.IndexOf(roundWinner));

		return roundWinner;
	}


    private void AdjustRankingScoreForPlayersToBeExcluded(AdjustRankingType adjustRankingType)
    {
		if (adjustRankingType == AdjustRankingType.UpdateRankingWhenAbortingGame) {
            var excludedPlayersCount = game.players.Where(p => p.isExcluded).Count();
            var nextGamePoint = game.GetNextGamePosition(excludedPlayersCount);
            game.players[0].rankingScore = (int)nextGamePoint;
            game.players[0].pointTypes.Add(nextGamePoint);
        } else {
			var excludedPlayersCount = game.players.Where(p => p.isExcluded).Count();
			var playersToBeExcluded = game.players.Where(p => p.score > (adjustRankingType == AdjustRankingType.UpdateRankingForAllPlayers ? -1 : Helpers.Constants.maxScoreClassic) && !p.isExcluded).OrderByDescending(p => p.score);

			var playersGroupedByScore = new List<List<Player>>();
			var score = -1;
			foreach (Player player in playersToBeExcluded)
			{
				if ((int)player.score != score)
				{
					playersGroupedByScore.Add(new List<Player>());
					score = player.score;
				}
				playersGroupedByScore[playersGroupedByScore.Count - 1].Add(player);
			}

			foreach (List<Player> playerGroup in playersGroupedByScore)
			{
                //var exclusionPointFactor = excludedPlayersCount * (excludedPlayersCount + 1) / 2; // formula: sum = N*(N+1)/2
                var gamePositionForEachPlayer = game.GetNextGamePosition(excludedPlayersCount + playerGroup.Count - 1); //((playerGroup.Count + excludedPlayersCount) * (playerGroup.Count + excludedPlayersCount + 1) / 2 - exclusionPointFactor) * 100 / playerGroup.Count;

                foreach (Player player in playerGroup)
				{
					player.rankingScore = (int)gamePositionForEachPlayer;
                    player.pointTypes.Add(gamePositionForEachPlayer);
				}
				excludedPlayersCount += playerGroup.Count;
			}
		}
    }


    private void EvaluateLastCards (List<Card> cards)
	{
		List<Card> sortedCards = cards.OrderBy(w => (int)w.cardType).ToList();
		int numberOfLosers = 0;

		if (game.gameType == Enums.GameType.Boost22)
		{
			Enums.CardType excludedCardType = sortedCards[sortedCards.Count - 1].cardType;
			
			for (int i = 0; i < sortedCards.Count; i++)
			{
				int playerIndex = cards.IndexOf(sortedCards[i]);

				if (Enums.CardTypeToInt(sortedCards[i].cardType) == Enums.CardTypeToInt(excludedCardType))
				{
					game.excludedCardTypes.Add(sortedCards[i].cardType);

					gameViewController.ExcludeCard(sortedCards[i], playerIndex);
					gameViewController.HighlightLosingCard(playerIndex);

					game.players[playerIndex].AddPointToScore(Enums.CardTypeToInt(sortedCards[i].cardType));
					gameViewController.SetPlayerScore(playerIndex, game.players.ToArray(), Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.excludePlayersDelay);
					gameViewController.ShowScoreInAvatar(playerIndex, game.players[playerIndex].score, true);
					numberOfLosers++;
				}
				else
				{
					gameViewController.ShowScoreInAvatar(playerIndex, game.players[playerIndex].score, false);
				}
			}

            AdjustRankingScoreForPlayersToBeExcluded(adjustRankingType: AdjustRankingType.UpdateRankingForClassicPlayers);

            List<string> excludedNames = new List<string>();

			for (int i = game.players.Count - 1; i >= 0; i--)
			{
				Player player = game.players[i];

				if (player.score > Helpers.Constants.maxScoreClassic && player.isExcluded == false)
				{
					excludedNames.Add(player.user.username);
					player.isExcluded = true;
					player.roundScore = 0;
					gameViewController.ExcludePlayer(i, game.players);
				}
			}

			if (excludedNames.Count == 1)
			{
				gameViewController.infoText.AnnouncePlayerIsExcluded(excludedNames[0], Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.excludePlayersDelay);
			}
			else if (excludedNames.Count > 0)
			{
				gameViewController.infoText.AnnouncePlayersAreExcluded(excludedNames, Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.excludePlayersDelay);
			}
		}
		else
		{
			for (int i = 0; i < sortedCards.Count; i++)
			{
				int playerIndex = cards.IndexOf(sortedCards[i]);
				//game.excludedCardTypes.Add(sortedCards[i].cardType);
				game.players[playerIndex].AddPointToScore(Enums.CardTypeToInt(sortedCards[i].cardType));
				gameViewController.SetPlayerScore(playerIndex, game.players.ToArray(), Delays.revealLastCardsDelay + Delays.excludePlayersDelay);
				//gameViewController.ShowScoreInAvatar(playerIndex, game.players[playerIndex].score, false);
			}
		}

		int winnerIndex = cards.IndexOf(sortedCards[0]);
		if (numberOfLosers == 3)
		{
			game.players[winnerIndex].pointTypes.Add(Enums.PointType.KnockOut3);
			if (game.players[winnerIndex].tablePosition == 0)
			{
				gameViewController.DisplayBonus(6);
			}
		}
		else if (numberOfLosers == 2)
		{
			game.players[winnerIndex].pointTypes.Add(Enums.PointType.KnockOut2);
			
			int otherWinnerIndex = cards.IndexOf(sortedCards[1]);
			game.players[otherWinnerIndex].pointTypes.Add(Enums.PointType.KnockOut2);
			
			if (game.players[winnerIndex].tablePosition == 0 || (otherWinnerIndex != -1 && game.players[otherWinnerIndex].tablePosition == 0))
			{
				gameViewController.DisplayBonus(5);
			}
		}
	}


	private void NextHandStartingPlayer ()
	{
		int handStartingIndex = game.players.IndexOf(game.handStartingPlayer);
		handStartingIndex++;

		if (handStartingIndex > game.players.Count - 1)
		{
			handStartingIndex = 0;
		}

		game.handStartingPlayer = game.players[handStartingIndex];

		if (game.handStartingPlayer.isExcluded == true)
		{
			NextHandStartingPlayer();
			return;
		}

		game.roundStartingPlayer = game.handStartingPlayer;
		gameViewController.ShowHideStarterPiece(true, Delays.revealLastCardsDelay + Delays.starterPieceDelay);
		gameViewController.MoveStarterPiece(handStartingIndex, Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.starterPieceDelay);
		gameViewController.HighlightPlayer(handStartingIndex, Delays.revealLastCardsDelay + Delays.newHandDelay + Delays.starterPieceDelay + 1.0f);
	}


	private bool IsGameOver ()
	{
		// Check if game is over
		return false;
	}


	private void StartHighAceMode ()
	{
		Debug.Log("High Ace Mode");
		game.gameStateType = Enums.GameStateType.HighAceMode;

		List<Card> highAceCards = new List<Card>();

		for (int i = 0; i < game.players.Count; i++)
		{
			game.players[i].hand.ClearCards();
			
			Card newHighAceCard = GetNewCards(1)[0];
			
			if (i > 0)
			{
				foreach (Card highAceCard in highAceCards)
				{
					while (Enums.CardTypeToInt(newHighAceCard.cardType) == Enums.CardTypeToInt(highAceCard.cardType))
					{
						newHighAceCard = GetNewCards(1)[0];
					}
				}
			}
			
			highAceCards.Add(newHighAceCard);
			game.players[i].hand.AddCard(newHighAceCard);
		}

		List<Card> sortedHighAceCards = highAceCards.OrderBy(w => (int)w.cardType).ToList();
		//gameViewController.SetHighAceCardFaces(highAceCards, sortedHighAceCards[sortedHighAceCards.Count - 1]);
		//gameViewController.infoText.AnnounceHighAce(Delays.highAceDealDelay);

		foreach (Player player in game.players)
		{
			if (player.hand.cards[0] == sortedHighAceCards[sortedHighAceCards.Count - 1])
			{
				game.handStartingPlayer = player;
				game.roundStartingPlayer = player;
				gameViewController.HighlightPlayer(game.players.IndexOf(player), 3.0f);
				break;
			}
		}

		gameViewController.infoText.AnnounceHandStartingPlayer(game.handStartingPlayer.user.username, Delays.highAceDealDelay + Delays.highAceHighlightDelay);// "Spiller " + game.players.IndexOf(game.handStartingPlayer), Delays.highAceDealDelay + Delays.highAceHighlightDelay);
		gameViewController.ShowHideStarterPiece(true, 0.0f);
		gameViewController.MoveStarterPiece(game.players.IndexOf(game.handStartingPlayer), Delays.starterPieceDelay);

		StartSwitchCardsMode(Delays.switchCardsModeDelay);
	}


	private void StartSwitchCardsMode (float delay)
	{
		Debug.Log("Switch Cards Mode");
		game.gameStateType = Enums.GameStateType.SwitchCardsMode;
		
		//gameViewController.StartTimer(timerDuration, 9.0f);

		FillCardPool();
	
		foreach (Player player in game.players)
		{
			if (player.isExcluded == false)
			{
				player.hand.ClearCards();
				List<Card> newCards = GetNewCards(7);
				player.hand.AddCards(newCards);
			}
		}
		
		gameViewController.SetHandCardFaces(game.players[0].hand.cards, true, delay);
		int maxNumberOfCards = GetMaxNumberOfCardsToSwitch();

		if (maxNumberOfCards == 1)
		{
			gameViewController.infoText.AnnounceSwitchOneCard(delay);
		}
		else
		{
			gameViewController.infoText.AnnounceSwitchNumberOfCards(maxNumberOfCards, delay);
		}

		gameViewController.PlayNotificationSound(delay);
	}


	private void StartPlayMode ()
	{
		game.gameStateType = Enums.GameStateType.PlayMode;
		gameViewController.DeactivateButtons();
		gameViewController.SetButtonState(2);
		gameViewController.ShowHideStarterPiece(false, 1.0f);
	}


	private List<Card> GetNewCards (int numberOfCards)
	{
		List<Card> newCards = new List<Card>();

		for (int i = 0; i < numberOfCards; i++)
		{
			newCards.Add(game.cardPool[i]);
		}

		foreach (Card newCard in newCards)
		{
			game.cardPool.Remove(newCard);
		}

		return newCards;
	}


	private void FillCardPool ()
	{
		Debug.Log("Filling card pool");
		game.cardPool = new List<Card>();

		foreach (Enums.CardType cardType in System.Enum.GetValues(typeof(Enums.CardType)))
		{
			if (cardType != Enums.CardType.noCard && !game.excludedCardTypes.Contains(cardType))
			{
				game.cardPool.Add(new Card(cardType));
			}
		}

		ShuffleCardPool();
	}


	private void ShuffleCardPool ()
	{
		for (int i = 0; i < game.cardPool.Count; i++)
		{
			Card temp = game.cardPool[i];
            int randomIndex = Random.Range(i, game.cardPool.Count);
            game.cardPool[i] = game.cardPool[randomIndex];
            game.cardPool[randomIndex] = temp;
		}
	}


	private int GetMaxNumberOfCardsToSwitch ()
	{
		int maxNumberOfCards = Mathf.FloorToInt(game.cardPool.Count / (float)ActivePlayers().Count);
		return maxNumberOfCards > 6 ? 6 : maxNumberOfCards;
	}


	private Player CurrentPlayer ()
	{
		int i = game.players.IndexOf(game.roundStartingPlayer);

		for (int j = 0; j < game.players.Count; j++)
		{
			if (i >= game.players.Count)
			{
				i = 0;
			}

			if (game.players[i].playedHandInRound == null && game.players[i].isExcluded == false)
			{
				return game.players[i];
			}
		
			i++;
		}

		return game.roundStartingPlayer;		
	}


	private List<Player> ActivePlayers ()
	{
		List<Player> activePlayers = new List<Player>();

		foreach (Player player in game.players)
		{
			if (player.isExcluded == false)
			{
				activePlayers.Add(player);
			}
		}

		return activePlayers;
	}


	private void LogAchieveLevelEvent (User user) 
	{
        var gamesPlayed = user.numberOfGamesPlayedForFacebook;
        Debug.Log("Log Achieved Level. Games played: " + gamesPlayed);

        var parameters = new Dictionary<string, object>();
		
		if (gamesPlayed == 4)
		{
			Debug.Log("Level Achieved: Rookie");
			parameters[AppEventParameterName.Level] = "Rookie";
			FB.LogAppEvent(AppEventName.AchievedLevel, 0.0f, parameters);
		}
		else if (gamesPlayed == 24)
		{
			Debug.Log("Level Achieved: Player");
			parameters[AppEventParameterName.Level] = "Player";
			FB.LogAppEvent(AppEventName.AchievedLevel, 0.0f, parameters);
		} /* else if ((gamesPlayed == 49 && invitedFriends >= 5) || (gamesPlayed >= 49 && invitedFriends == 5)) {
            Debug.Log("Level Achieved: Ambassador");
            parameters[AppEventParameterName.Level] = "Ambassador";
            FB.LogAppEvent(AppEventName.AchievedLevel, 0.0f, parameters);
        } */
	}
}
