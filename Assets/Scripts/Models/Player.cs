using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
	public Enums.PlayerType playerType;
	public Hand hand;
	public Hand playedHandInRound;
	public int roundScore;
	public int roundRank;
	public int score;
	public int rankingScore = 0;
	public int tablePosition; // 0=the logged in user, 1=player to the left, 2=across, 3=player to the right
	public bool isExcluded = false;
	public List<Enums.PointType> pointTypes = new List<Enums.PointType>();
	public User user;

	public Player () {}
	
	public Player (User user)
	{
		this.user = user;
        playerType = Enums.PlayerType.User;
        hand = new Hand();
	}


	public virtual void PlayedCards (List<Card> playedCards)
	{
        playedHandInRound = new Hand();
		playedHandInRound.AddCards(playedCards);

		hand.RemoveCards(playedCards);
	}


	public void AddPointToScore (int points)
	{
		score += points;
	}
}
