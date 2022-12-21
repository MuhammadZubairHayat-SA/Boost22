using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScoreboard 
{
	public Score[] scores;
	public Event gameEvent;	
	public int userPosition;
	public int participantCount;
}


public class DynamicUserData
{
	public int gamesLeftToday;
    public int extraGamesLeftToday;
    public int extraGames;
    public int loyaltyPoints;
    public float realMoneyAmount;
    public int realMoneyCurrencyType;
}


public class DashboardStats
{
	public int totalNumberOfPlayers;
	public int totalNumberOfGamesPlayed;
	public int numberOfUsersRegisteredToday;
	public int numberOfPlayersLastHour;
	public int numberOfPlayersReadyForNextRushEvent;
}

public class DashboardDetails
{
    public string statisticsDetails;
}


public class EventLeaderboard 
{
    public MinimalEventLeader[] leaders;
}

[System.Serializable]
public class MinimalEventLeader 
{
    public int userId;
    public string username;
    public int score;
}


public class Winners
{
    public Winner[] winners;
    public Winner userWinner; // the user's own winnings
}

[System.Serializable]
public class Winner
{
    public int userId;
    public string username;
    public float gameScore;
    public int ranking;
}