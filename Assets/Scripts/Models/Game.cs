using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game 
{
	public Enums.GameType gameType;
	public List<Player> players;
	public Player handStartingPlayer; 
	public Player roundStartingPlayer; 
	public Enums.GameStateType gameStateType;
	public List<Card> cardPool;
	public List<Enums.CardType> excludedCardTypes;
	public int currentPlayerIndex; // Change to use method for finding current player from Player.playedHandInRound
    public List<Card> highestCardsPlayed = new List<Card>();
	public float startTime;
	public float endTime;
	public bool firstHand = true;
	public bool firstRoundInHand = true;
	public bool resultsSentToServer = false;
	public bool gameIsRigged = false;
	public string gameIdentifier = System.Guid.NewGuid().ToString();

    public static Game Create (Enums.GameType type)
	{
		var game = new Game
		{
			gameType = type,
			gameStateType = Enums.GameStateType.HighAceMode,
			excludedCardTypes = new List<Enums.CardType>(),
			players = new List<Player> { new Player(user: UserManager.LoggedInUser) }
		};

		game.players.AddRange(UserManager.GetRobotPlayers(gameType: type));

		for (int i = 0; i < game.players.Count; i++) {
			game.players[i].tablePosition = i;
		}

		game.startTime = Time.time;

		return game;
	}


    public void NewRound()
    {
        highestCardsPlayed = new List<Card>();
        foreach (Player player in players)
        {
            player.playedHandInRound = null;
        }

    }

    public void NewHand()
    {
		firstRoundInHand = true;
        NewRound();
    }

    public Enums.PointType GetNextGamePosition(int playersAlreadyExcluded)
    {
        if (playersAlreadyExcluded > 3)
        {
            playersAlreadyExcluded = 3;
        }
        return Enums.GetGamePositions(this.gameType)[playersAlreadyExcluded];
    }

}
